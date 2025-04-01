using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Conversation
{
    public class ConversationResponse
    {
        public int ConversationID { get; set; }
        public Account OtherUser { get; set; }
        public Message LastMessage { get; set; }
    }
}
