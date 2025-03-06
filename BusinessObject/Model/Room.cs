using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }

        [Required]
        public string roomNumber { get; set; }

        public bool Status { get; set; }

        public int? RoomAvailabilityID { get; set; }
        [ForeignKey("RoomAvailabilityID")]
        public RoomAvailability? RoomAvailability { get; set; }
    }
}
