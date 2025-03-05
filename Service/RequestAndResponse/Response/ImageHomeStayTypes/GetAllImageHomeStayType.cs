using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.ImageHomeStayTypes
{
    public class GetAllImageHomeStayType
    {
        [Key]
        public int ImageHomeStayTypesID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? HomeStayTypesID { get; set; }
        [ForeignKey("HomeStayTypesID ")]
        public HomeStayRentals? HomeStayTypes { get; set; }
    }
}
