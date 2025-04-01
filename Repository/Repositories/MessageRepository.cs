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
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        private readonly MessageDAO _messDao;

        public MessageRepository(MessageDAO messDao) : base(messDao)
        {
            _messDao = messDao;
        }

        public async Task<List<Message>> GetMessagesByConversationAsync(int conversationId)
        {
            return await _messDao.GetMessagesByConversationAsync(conversationId);
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            return await _messDao.CreateMessageAsync(message);
        }

        public async Task<Message> UpdateMessageAsync(Message message)
        {
            return await _messDao.UpdateMessageAsync(message);
        }
        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _messDao.GetMessageByIdAsync(messageId);
        }
    }
}
