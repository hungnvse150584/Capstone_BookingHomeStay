using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class ImageRoom
    {
        [Key]
        public int ImageRoomID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? RoomID { get; set; }
        [ForeignKey("RoomID")]
        public Room? Room { get; set; }
    }
}
