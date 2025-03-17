using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class HomeStay
    {
        [Key]
        public int HomeStayID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }
        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        [EnumDataType(typeof(HomeStayStatus))]
        public HomeStayStatus Status { get; set; }

        public string Area { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        [ForeignKey("CommissionRateID")]
        public int? CommissionRateID { get; set; }
        public CommissionRate? CommissionRate { get; set; }

        public int? CancellationID { get; set; }
        public CancellationPolicy? CancelPolicy { get; set; }

        [EnumDataType(typeof(RentalType))]

        public RentalType TypeOfRental { get; set; } 

        public ICollection<Report> Reports { get; set; }

        public ICollection<HomeStayRentals> HomeStayRentals { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public ICollection<CultureExperience> CultureExperiences { get; set; }

        public ICollection<Services> Services { get; set; }

        public ICollection<Rating> Ratings { get; set; }
    }

    public enum HomeStayStatus
    {
        PendingApproval = 0, // Chờ xét duyệt
        Accepted = 1,        // Chấp nhận
        Rejected = 2,        // Từ chối
        Cancelled = 3        // Hủy
    }

    public enum RentalType
    {
        [Display(Name = "Nhà nghỉ")]
        GuestHouse = 1,

        [Display(Name = "Lều cắm trại")]
        CampingTent = 2,

        [Display(Name = "Resort")]
        Resort = 3,

        [Display(Name = "Căn hộ")]
        Apartment = 4,

        [Display(Name = "Khách sạn")]
        Hotel = 5
    }

