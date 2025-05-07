using BusinessObject.Model;
using Service.RequestAndResponse.Response.CancellationPolicyRequest;
using Service.RequestAndResponse.Response.CultureExperiences;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.Ratings;
using Service.RequestAndResponse.Response.Reports;
using Service.RequestAndResponse.Response.Services;
using System;

namespace Service.RequestAndResponse.Response.HomeStays
{
    public class SimpleHomeStayResponse
    {
        public int HomeStayID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public HomeStayStatus Status { get; set; }
        public string Area { get; set; }
        public string AccountID { get; set; }
        public int? CommissionRateID { get; set; }
        public object CommissionRate { get; set; }
        //public int? CancellationID { get; set; }
        public GetAllCancellationPolicy CancelPolicy { get; set; }
        public int TypeOfRental { get; set; }
        public IEnumerable<GetReportResponse> Reports { get; set; }
        public IEnumerable<ImageHomeStayResponse> ImageHomeStays { get; set; }
        public IEnumerable<GetAllCultureExperiencesResponse> CultureExperiences { get; set; }
        //public IEnumerable<GetAllCancellationPolicy> CancellationPolicies { get; set; }

        public IEnumerable<GetServiceForHomeStay> Services { get; set; }
        //public IEnumerable<GetAllRatingResponse> Ratings { get; set; }
        public double? SumRate { get; set; }
        public int TotalRatings { get; set; } 
        public IEnumerable<CreateRatingResponse> LatestRatings { get; set; }
        public decimal? CheapestPrice { get; set; }
        public string? StaffID { get; set; } 
        public string? StaffName { get; set; }
        public string? OwnerID { get; set; } 
       
    }
}
