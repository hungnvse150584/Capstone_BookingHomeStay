using Microsoft.AspNetCore.SignalR;
using Repository.IRepositories;
using Service.IService;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IMessageRepository _messageRepository;
    private static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

    public ChatHub(IChatService chatService, IMessageRepository messageRepository)
    {
        _chatService = chatService;
        _messageRepository = messageRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name; // Lấy từ JWT
        if (string.IsNullOrEmpty(userId))
        {
            // Nếu không lấy được từ JWT, thử từ query string (dùng tạm thời để debug)
            userId = Context.GetHttpContext()?.Request.Query["userId"];
        }

        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections[userId] = Context.ConnectionId;
            // Thêm người dùng vào nhóm dựa trên userId
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            Console.WriteLine($"User {userId} connected with ConnectionId: {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (userId != null)
        {
            _userConnections.TryRemove(userId, out _);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            Console.WriteLine($"User {userId} disconnected.");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterUser(string userId)
    {
        if (!_userConnections.ContainsKey(userId))
        {
            _userConnections[userId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            Console.WriteLine($"User {userId} registered with ConnectionId: {Context.ConnectionId}");
        }
    }

    public async Task SendMessage(string senderId, string receiverId, string content, string senderName, int homeStayId, List<IFormFile> images = null)
    {
        var message = await _chatService.SendMessageAsync(senderId, receiverId, content, senderName, homeStayId, images);

        // Gửi đến người nhận nếu online
        if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID, senderName, receiverId);
        }

        // Gửi đến người gửi nếu online
        if (_userConnections.TryGetValue(senderId, out var senderConnectionId) && senderConnectionId != receiverConnectionId)
        {
            await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID, senderName, receiverId);
        }

        // Gửi đến nhóm để đảm bảo tất cả thiết bị của receiver nhận được (nếu có nhiều kết nối)
        await Clients.Group(receiverId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID, senderName, receiverId);
    }

    public async Task MarkAsRead(int messageId)
    {
        await _chatService.MarkMessageAsReadAsync(messageId);
        var message = await _messageRepository.GetMessageByIdAsync(messageId);
        if (message != null && _userConnections.TryGetValue(message.SenderID, out var senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("MessageRead", messageId);
        }
    }

    public async Task MarkAllMessagesAsRead(int conversationId, string userId)
    {
        await _chatService.MarkAllMessagesAsReadAsync(conversationId, userId);
        var messages = await _messageRepository.GetMessagesByConversationAsync(conversationId);
        var unreadMessages = messages.Where(m => !m.IsRead && m.SenderID != userId).ToList();

        foreach (var message in unreadMessages)
        {
            if (_userConnections.TryGetValue(message.SenderID, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendAsync("MessageRead", message.MessageID);
            }
        }
    }
}