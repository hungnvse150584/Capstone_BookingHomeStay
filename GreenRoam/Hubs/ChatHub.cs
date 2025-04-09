// GreenRoam/Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using Repository.IRepositories;
using Service.IService;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GreenRoam.Hubs
{
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
            // Lấy userId từ query string
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                Console.WriteLine($"User {userId} connected with ConnectionId {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine("User connected without userId");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                _userConnections.TryRemove(userId, out _);
                Console.WriteLine($"User {userId} disconnected");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUser(string userId)
        {
            _userConnections[userId] = Context.ConnectionId;
            Console.WriteLine($"User {userId} registered with ConnectionId {Context.ConnectionId}");
        }

        public async Task SendMessage(string senderId, string receiverId, string content, string senderName, int homeStayId, List<IFormFile> images = null)
        {
            var message = await _chatService.SendMessageAsync(senderId, receiverId, content, senderName, homeStayId, images);

            // Gửi tin nhắn cho cả sender và receiver
            if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID, senderName, receiverId);
                Console.WriteLine($"Sent message to receiver {receiverId}");
            }
            if (_userConnections.TryGetValue(senderId, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID, senderName, receiverId);
                Console.WriteLine($"Sent message to sender {senderId}");
            }
        }

        public async Task MarkAsRead(int messageId)
        {
            await _chatService.MarkMessageAsReadAsync(messageId);
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message != null)
            {
                if (_userConnections.TryGetValue(message.SenderID, out var senderConnectionId))
                {
                    await Clients.Client(senderConnectionId).SendAsync("MessageRead", messageId);
                }
            }
        }

        public async Task MarkAllMessagesAsRead(int conversationId, string userId)
        {
            await _chatService.MarkAllMessagesAsReadAsync(conversationId, userId);

            var messages = await _messageRepository.GetMessagesByConversationAsync(conversationId);
            var unreadMessages = messages.Where(m => m.IsRead && m.SenderID != userId).ToList();

            foreach (var message in unreadMessages)
            {
                if (_userConnections.TryGetValue(message.SenderID, out var senderConnectionId))
                {
                    await Clients.Client(senderConnectionId).SendAsync("MessageRead", message.MessageID);
                }
            }
        }
        public string GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }
    }
}