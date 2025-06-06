﻿using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.ImageHomeStayTypes
{
    public class UpdateImageHomeStayTypesRequest
    {
        //[Key]
        //public int ImageHomeStayTypesID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? HomeStayTypesID { get; set; }
        //[ForeignKey("HomeStayTypesID ")]
        //public HomeStayTypes? HomeStayTypes { get; set; }
    }
}
