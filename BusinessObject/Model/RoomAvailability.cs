using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class RoomAvailability
    {
        [Key]
        public int RoomAvailabilityID { get; set; }

        public DateTime DateTime { get; set; }
        
        public int AvailableRooms { get; set; }

        public int UsedRooms { get; set; }

        public int RemainingRooms { get; set; }

        public bool Status { get; set; }

        public int? RoomTypesID { get; set; }
        [ForeignKey("RoomTypesID")]
        public RoomTypes? RoomTypes { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }

