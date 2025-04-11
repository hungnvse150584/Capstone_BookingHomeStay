using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }

        public string StaffIdAccount { get; set; }

        public string StaffName { get; set;}

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account? Owner { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID")]
        public HomeStay? HomeStay { get; set; }
    }
}
