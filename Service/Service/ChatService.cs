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

            public ChatService(
                IConversationRepository conversationRepository,
                IMessageRepository messageRepository,
                IHomeStayRepository homeStayRepository)
            {
                _conversationRepository = conversationRepository;
                _messageRepository = messageRepository;
                _homeStayRepository = homeStayRepository;
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
                var unreadMessages = messages.Where(m => !m.IsRead).ToList(); 

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

                return homeStay.AccountID; // Trả về AccountID mà không kiểm tra vai trò OWNER
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