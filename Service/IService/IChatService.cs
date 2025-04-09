// Service/IService/IChatService.cs
using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IChatService
    {
        Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id, int homeStayId);
        Task<List<Conversation>> GetConversationsByUserAsync(string userId);
        Task<List<Message>> GetMessagesByConversationAsync(int conversationId);
        Task<Message> SendMessageAsync(string senderId, string receiverId, string content, string senderName, int homeStayId,  List<IFormFile> images = null);
        Task MarkMessageAsReadAsync(int messageId);
        Task MarkAllMessagesAsReadAsync(int conversationId, string userId);
        Task<string> GetOwnerIdByHomeStayIdAsync(int homeStayId);
        Task<List<Conversation>> GetConversationsForOwnerAsync(string ownerId);
        Task<int> GetUnreadMessageCountAsync(int conversationId, string userId);
        Task<Conversation> GetOrCreateConversationWithHomeStayOwnerAsync(string customerId, int homeStayId);
        Task<List<Conversation>> GetConversationsByHomeStayIdAsync(int homeStayId);
        Task<List<Conversation>> GetConversationsByCustomerIdAsync(string customerId);
        Task<List<string>> GetInitialSuggestionsAsync(int homeStayId);
        Task<List<string>> GetDetailedSuggestionsAsync(string customerMessage, int homeStayId);
    }
}