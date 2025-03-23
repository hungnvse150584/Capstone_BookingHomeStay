using Microsoft.AspNetCore.Http;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Request.RoomType;
using System.Text.Json;

namespace Service.RequestAndResponse.Request.HomeStayType
{
    public class CreateHomeStayTypeRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? HomeStayID { get; set; }
        public int numberBedRoom { get; set; }
        public int numberBathRoom { get; set; }
        public int numberKitchen { get; set; }
        public int numberWifi { get; set; }
        public bool? Status { get; set; } = true;
        public bool? RentWhole { get; set; } = true;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
        public List<IFormFile> Images { get; set; }

        public string? PricingJson { get; set; }

        public List<PricingForHomeStayRental> Pricing { get; set; }


    }
}