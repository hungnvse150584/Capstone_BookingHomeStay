using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class RoomChangeHistory
    {
        [Key]
        public int RoomChangeHistoryID { get; set; }

        [ForeignKey("BookingDetailID")]
        public int BookingDetailID { get; set; }
        public BookingDetail BookingDetail { get; set; }

        public int? OldRoomID { get; set; }
        public int? NewRoomID { get; set; }

        public DateTime UsagedDate { get; set; }
        public DateTime ChangedDate { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account ChangedBy { get; set; }
    }
}
