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
            // Khi người dùng kết nối, yêu cầu họ gửi UserId
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Khi người dùng ngắt kết nối, xóa ánh xạ
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                _userConnections.TryRemove(userId, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUser(string userId)
        {
            // Lưu ánh xạ giữa UserId và ConnectionId
            _userConnections[userId] = Context.ConnectionId;
        }

        public async Task SendMessage(string senderId, string receiverId, string content)
        {
            var message = await _chatService.SendMessageAsync(senderId, receiverId, content);

            // Gửi tin nhắn đến người nhận
            if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, content, message.SentAt, message.MessageID, message.ConversationID);
            }

            // Gửi tin nhắn đến chính người gửi
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

            // Thông báo cho người gửi của các tin nhắn rằng tin nhắn của họ đã được đọc
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