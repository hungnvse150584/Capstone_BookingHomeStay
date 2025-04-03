// Service/IService/IChatService.cs
using BusinessObject.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IChatService
    {
        Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id);
        Task<List<Conversation>> GetConversationsByUserAsync(string userId);
        Task<List<Message>> GetMessagesByConversationAsync(int conversationId);
        Task<Message> SendMessageAsync(string senderId, string receiverId, string content, string senderName);
        Task MarkMessageAsReadAsync(int messageId);
        Task MarkAllMessagesAsReadAsync(int conversationId, string userId);
        Task<string> GetOwnerIdByHomeStayIdAsync(int homeStayId);
        Task<List<Conversation>> GetConversationsForOwnerAsync(string ownerId);
        Task<int> GetUnreadMessageCountAsync(int conversationId, string userId);
        Task<Conversation> GetOrCreateConversationWithHomeStayOwnerAsync(string customerId, int homeStayId);
    }
}