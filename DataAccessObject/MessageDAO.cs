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
    public class MessageDAO : BaseDAO<Message>
    {
        private readonly GreenRoamContext _context;
        public MessageDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Message>> GetMessagesByConversationAsync(int conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationID == conversationId)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message> UpdateMessageAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageID == messageId);
        }
    }
}
