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

        [Required]
        public int numberBed { get; set; }

        [Required]
        public int numberBathRoom { get; set; }

        [Required]
        public int numberWifi { get; set; }

        public bool isActive { get; set; }

        public DateTime? DeleteAt { get; set; }

        public int? RoomTypesID { get; set; }
        [ForeignKey("RoomTypesID")]
        public RoomTypes? RoomTypes { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }

        public ICollection<ImageRoom> ImageRooms { get; set; }
    }
}
