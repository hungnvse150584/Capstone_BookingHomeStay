using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class ConversationDAO : BaseDAO<Conversation>
    {
        private readonly GreenRoamContext _context;

        public ConversationDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Conversation> GetConversationByUsersAsync(string user1Id, string user2Id)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => (c.User1ID == user1Id && c.User2ID == user2Id) ||
                                          (c.User1ID == user2Id && c.User2ID == user1Id));
        }

        public async Task<List<Conversation>> GetConversationsByUserAsync(string userId)
        {
            return await _context.Conversations
                .Where(c => c.User1ID == userId || c.User2ID == userId)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .ToListAsync();
        }

        public async Task<Conversation> CreateConversationAsync(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }
        public async Task<List<Conversation>> GetConversationsByHomeStayIdAsync(int homeStayId)
        {
            return await _context.Conversations
                .Where(c => c.HomeStayID == homeStayId)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .ToListAsync();
        }
    }
}
