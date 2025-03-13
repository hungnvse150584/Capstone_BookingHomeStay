using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public string ReportText { get; set; }

        public string ResponseText { get; set; }

        public bool Status { get; set; }

        public string? Image { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID")]
        public HomeStay? HomeStay { get; set; }

        public int? BookingID { get; set; }
        [ForeignKey("BookingID")]
        public Booking? Booking { get; set; }
    }

