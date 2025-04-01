using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Conversation
{
    public class OwnerConversationResponse
    {
        public int ConversationID { get; set; }
        public SimplifiedAccountResponse OtherUser { get; set; }
        public SimplifiedMessageResponse LastMessage { get; set; }
        public int UnreadMessageCount { get; set; }
    }
}
