// Service/ChatService.cs
using BusinessObject.Model;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.IRepositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class ChatService : IChatService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IHomeStayRepository _homeStayRepository;
        private readonly GreenRoamContext _context;

        public ChatService(IConversationRepository conversationRepository, IMessageRepository messageRepository)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
        }

        public async Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id)
        {
            var conversation = await _conversationRepository.GetConversationByUsersAsync(user1Id, user2Id);
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1ID = user1Id,
                    User2ID = user2Id,
                    CreatedAt = DateTime.UtcNow
                };
                await _conversationRepository.CreateConversationAsync(conversation);
            }
            return conversation;
        }

        public async Task<List<Conversation>> GetConversationsByUserAsync(string userId)
        {
            return await _conversationRepository.GetConversationsByUserAsync(userId);
        }

        public async Task<List<Message>> GetMessagesByConversationAsync(int conversationId)
        {
            return await _messageRepository.GetMessagesByConversationAsync(conversationId);
        }

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content)
        {
            var conversation = await GetOrCreateConversationAsync(senderId, receiverId);
            var message = new Message
            {
                ConversationID = conversation.ConversationID,
                SenderID = senderId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            return await _messageRepository.CreateMessageAsync(message);
        }

        public async Task MarkMessageAsReadAsync(int messageId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _messageRepository.UpdateMessageAsync(message);
            }
        }
        public async Task MarkAllMessagesAsReadAsync(int conversationId, string userId)
        {
            var messages = await _messageRepository.GetMessagesByConversationAsync(conversationId);
            var unreadMessages = messages.Where(m => !m.IsRead && m.SenderID != userId).ToList();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                await _messageRepository.UpdateMessageAsync(message);
            }
        }
        public async Task<string> GetOwnerIdByHomeStayIdAsync(int homeStayId)
        {
            var homeStay = await _homeStayRepository.GetByIdAsync(homeStayId);
            if (homeStay == null)
            {
                throw new Exception("HomeStay not found.");
            }

            // Kiểm tra xem AccountID có vai trò OWNER hay không
            var userRole = await _context.UserRoles
                .Where(ur => ur.UserId == homeStay.AccountID && ur.RoleId == "2ad2e3fe-065c-41bd-a6eb-7c83ba258929") // RoleId của OWNER
                .FirstOrDefaultAsync();

            if (userRole == null)
            {
                throw new Exception("The account associated with this HomeStay is not an OWNER.");
            }

            return homeStay.AccountID; // Sử dụng AccountID thay vì OwnerId
        }

        
        public async Task<List<Conversation>> GetConversationsForOwnerAsync(string ownerId)
        {
            var conversations = await _conversationRepository.GetConversationsByUserAsync(ownerId);
            return conversations;
        }

        public async Task<int> GetUnreadMessageCountAsync(int conversationId, string userId)
        {
            var messages = await _messageRepository.GetMessagesByConversationAsync(conversationId);
            return messages.Count(m => !m.IsRead && m.SenderID != userId);
        }
        public async Task<Conversation> GetOrCreateConversationWithHomeStayOwnerAsync(string customerId, int homeStayId)
        {
            var ownerId = await GetOwnerIdByHomeStayIdAsync(homeStayId);
            return await GetOrCreateConversationAsync(customerId, ownerId);
        }
    }
}