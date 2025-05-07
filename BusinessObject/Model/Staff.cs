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

        public string? Username { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account? Owner { get; set; }

        public int? HomeStayID { get; set; }
        public HomeStay? HomeStay { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
