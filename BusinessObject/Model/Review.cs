using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        public string? Content { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID ")]
        public HomeStay? HomeStay { get; set; }
    }

