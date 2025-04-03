using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class Conversation
    {
        [Key]
        public int ConversationID { get; set; }

        [ForeignKey("User1")]
        public string User1ID { get; set; }
        public Account User1 { get; set; }

        [ForeignKey("User2")]
        public string User2ID { get; set; }
        public Account User2 { get; set; }

        public int? HomeStayID { get; set; }
        public HomeStay? HomeStay { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
    }
}
