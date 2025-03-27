using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Response.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.HomeStayType
{
    public class UpdateHomeStayTypeRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? HomeStayID { get; set; }
        public int numberBedRoom { get; set; }
        public int numberBathRoom { get; set; }
        public int numberKitchen { get; set; }
        public int numberWifi { get; set; }
        public bool Status { get; set; } = true;
        public bool RentWhole { get; set; } = true;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public ICollection<GetAllUpdatePricicing> Pricing { get; set; }
    }
}
