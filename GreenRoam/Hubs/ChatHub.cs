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
            // Lấy userId từ token hoặc query string
            var userId = Context.User?.Identity?.Name; // Nếu dùng JWT
                                                       // Hoặc từ query string: var userId = Context.GetHttpContext().Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                _userConnections.TryRemove(userId, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUser(string userId)
        {
            _userConnections[userId] = Context.ConnectionId;
        }

        public async Task SendMessage(string senderId, string receiverId, string content, string senderName, int homeStayId,int conversationId, List<IFormFile> images = null)
        {
            var message = await _chatService.SendMessageAsync(senderId, receiverId, content, senderName,homeStayId,conversationId ,images);

            if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID);
            }

            if (_userConnections.TryGetValue(senderId, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID);
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
    }
}