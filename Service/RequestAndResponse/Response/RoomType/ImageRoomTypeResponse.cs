using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.RoomType
{
    public class ImageRoomTypeResponse
    {
        //[Required]
        //public IFormFile File { get; set; }
        public int ImageRoomTypesID { get; set; }
        public string Image { get; set; }

    }
}
