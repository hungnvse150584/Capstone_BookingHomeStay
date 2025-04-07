using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IConversationRepository : IBaseRepository<Conversation>
    {
        Task<Conversation> GetConversationByUsersAsync(string user1Id, string user2Id, int homeStayId);
        Task<List<Conversation>> GetConversationsByUserAsync(string userId);
        Task<Conversation> CreateConversationAsync(Conversation conversation);
        Task<List<Conversation>> GetConversationsByHomeStayIdAsync(int homeStayId);

    }
}
