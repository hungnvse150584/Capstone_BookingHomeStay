// Service/ChatService.cs
using BusinessObject.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccessObject;
using Microsoft.AspNetCore.Http;
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
        private readonly Cloudinary _cloudinary;

        public ChatService(
                IConversationRepository conversationRepository,
                IMessageRepository messageRepository,
                IHomeStayRepository homeStayRepository,
                Cloudinary cloudinary)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _homeStayRepository = homeStayRepository;
            _cloudinary = cloudinary;
        }

        public async Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id, int homeStayId)
        {
            var orderedUserIds = new[] { user1Id, user2Id }.OrderBy(id => id).ToArray();
            string orderedUser1Id = orderedUserIds[0];
            string orderedUser2Id = orderedUserIds[1];

            var conversation = await _conversationRepository.GetConversationByUsersAsync(user1Id, user2Id, homeStayId);
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1ID = user1Id,
                    User2ID = user2Id,
                    CreatedAt = DateTime.UtcNow,
                    HomeStayID = homeStayId
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

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content, string senderName, int homeStayId, List<IFormFile> images = null)
        {
            var conversation = await GetOrCreateConversationAsync(senderId, receiverId, homeStayId);

            string finalContent = content ?? string.Empty;
            if (images != null && images.Any())
            {
                var imageUrls = await UploadImagesToCloudinary(images);
                if (imageUrls.Any())
                {
                    finalContent = finalContent + (string.IsNullOrEmpty(finalContent) ? "" : "\n") + string.Join("\n", imageUrls);
                }
            }

            var message = new Message
            {
                ConversationID = conversation.ConversationID,
                SenderID = senderId,
                receiverID = receiverId,
                senderName = senderName,
                Content = finalContent,
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
            var conversation = await _conversationRepository.GetConversationByUsersAsync(customerId, ownerId, homeStayId);
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1ID = customerId,
                    User2ID = ownerId,
                    CreatedAt = DateTime.UtcNow,
                    HomeStayID = homeStayId
                };
                await _conversationRepository.CreateConversationAsync(conversation);
            }
            return conversation;
        }

        public async Task<List<Conversation>> GetConversationsByHomeStayIdAsync(int homeStayId)
        {
            return await _conversationRepository.GetConversationsByHomeStayIdAsync(homeStayId);
        }
        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return new List<string>(); // Trả về danh sách rỗng nếu không có file
            }

            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue; // Bỏ qua file không hợp lệ
                }

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "ChatImages" // Lưu hình ảnh vào thư mục ChatImages trên Cloudinary
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    urls.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    throw new Exception($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}");
                }
            }

            return urls;
        }
        public async Task<List<Conversation>> GetConversationsByCustomerIdAsync(string customerId)
        {

            var conversations = await _conversationRepository.GetConversationsByUserAsync(customerId);

            var filteredConversations = conversations
                .Where(c => c.HomeStayID.HasValue)
                .ToList();

            return filteredConversations;
        }
    }
}