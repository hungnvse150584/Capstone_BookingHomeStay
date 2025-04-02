using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }

        [ForeignKey("ConversationID")]
        public int ConversationID { get; set; }
        public Conversation Conversation { get; set; }

        [Required]
        public string SenderID { get; set; } // Ai gửi tin nhắn
        public Account Sender { get; set; }

        [Required]
        public string receiverID { get; set; }
        public string senderName { get; set; }

        [Required]
        public string Content { get; set; } // Nội dung tin nhắn

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false; // Đã đọc chưa
    }
}
