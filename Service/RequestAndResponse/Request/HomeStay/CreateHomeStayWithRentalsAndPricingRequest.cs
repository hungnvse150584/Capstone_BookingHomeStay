using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Service.RequestAndResponse.Request.Pricing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.RequestAndResponse.Request.HomeStay
{
    public class CreateHomeStayWithRentalsAndPricingRequest
    {
        [Required(ErrorMessage = "HomeStay must have name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "HomeStay must have Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "HomeStay must have Address")]
        public string Address { get; set; }

        [Required]
        public double Longtitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        public RentalType RentalType { get; set; }

        public string? Area { get; set; }

        [Required(ErrorMessage = "Must Include AccountID")]
        public string AccountID { get; set; }

        public List<IFormFile> Images { get; set; }

        public string RentalName { get; set; }

        public string? RentalDescription { get; set; }

        public int numberBedRoom { get; set; }

        public int numberBathRoom { get; set; }

        public int numberKitchen { get; set; }

        public int numberWifi { get; set; }

        public bool? Status { get; set; } = true;

        public bool? RentWhole { get; set; } = true;

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public int MaxPeople { get; set; }

        public List<IFormFile> RentalImages { get; set; }

        public string? PricingJson { get; set; }

        public List<PricingForHomeStayRental>? Pricing { get; set; }
    }
}