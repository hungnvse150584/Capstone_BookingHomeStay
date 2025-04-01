using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<List<Message>> GetMessagesByConversationAsync(int conversationId);
        Task<Message> CreateMessageAsync(Message message);
        Task<Message> UpdateMessageAsync(Message message);
        Task<Message> GetMessageByIdAsync(int messageId);
    }
}
