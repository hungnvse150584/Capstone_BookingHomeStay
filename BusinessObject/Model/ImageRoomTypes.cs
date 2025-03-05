using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class ImageRoomTypes
    {
        [Key]
        public int ImageRoomTypesID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? RoomTypesID { get; set; }
        [ForeignKey("RoomTypesID")]
        public RoomTypes? RoomTypes { get; set; }
    }

