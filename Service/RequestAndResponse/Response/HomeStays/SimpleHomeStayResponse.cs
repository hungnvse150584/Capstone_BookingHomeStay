using BusinessObject.Model;
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
        public HomeStayStatus Status { get; set; } // Nếu HomeStayStatus là enum, bạn có thể để kiểu enum
        public string Area { get; set; }
        public string AccountID { get; set; }
        public int? CommissionRateID { get; set; }
        public object CommissionRate { get; set; }
        public int? CancellationID { get; set; }
        public object CancelPolicy { get; set; }
        public int TypeOfRental { get; set; }
        public IEnumerable<object> Reports { get; set; }
        public IEnumerable<object> HomeStayRentals { get; set; }
        public IEnumerable<object> ImageHomeStays { get; set; }
        public IEnumerable<object> Bookings { get; set; }
        public IEnumerable<object> CultureExperiences { get; set; }
        public IEnumerable<object> Services { get; set; }
        public IEnumerable<object> Ratings { get; set; }
    }
}
