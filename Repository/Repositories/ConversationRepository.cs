using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
    {
        private readonly ConversationDAO _converDao;

        public ConversationRepository(ConversationDAO converDao) : base(converDao)
        {
            _converDao = converDao;
        }

        public async Task<Conversation> CreateConversationAsync(Conversation conversation)
        {
            return await _converDao.CreateConversationAsync(conversation);
        }

        public async Task<Conversation> GetConversationByUsersAsync(string user1Id, string user2Id)
        {
            return await _converDao.GetConversationByUsersAsync(user1Id, user2Id);
        }

        public async Task<List<Conversation>> GetConversationsByUserAsync(string userId)
        {
            return await _converDao.GetConversationsByUserAsync(userId);
        }
        public async Task<List<Conversation>> GetConversationsByHomeStayIdAsync(int homeStayId)
        {
            return await _converDao.GetConversationsByHomeStayIdAsync(homeStayId);
        }
    }
}
