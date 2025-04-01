using BusinessObject.Model;
using System;

namespace Service.RequestAndResponse.Response.Conversation
{
    public class SimplifiedConversationResponse
    {
        public int ConversationID { get; set; }

        public SimplifiedAccountResponse OtherUser { get; set; }

        public SimplifiedMessageResponse LastMessage { get; set; }
    }

    public class SimplifiedAccountResponse
    {
        public string AccountID { get; set; }
        public string Name { get; set; }
    }

    public class SimplifiedMessageResponse
    {
        public int MessageID { get; set; }
        public string SenderID { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}