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

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        [EnumDataType(typeof(HomeStayStatus))]
        public HomeStayStatus Status { get; set; }

        public string Area { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        [ForeignKey("LocationID")]
        public int LocationID { get; set; }
        public Location Location { get; set; }

        public ICollection<Report> Reports { get; set; }

        public ICollection<HomeStayTypes> HomeStayTypes { get; set; }

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

