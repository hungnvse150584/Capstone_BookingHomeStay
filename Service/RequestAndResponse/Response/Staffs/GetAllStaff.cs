using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.HomeStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Staffs
{
    public class GetAllStaff
    {
        public int StaffID { get; set; }

        public string StaffIdAccount { get; set; }

        public string StaffName { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public GetAccountUser? Owner { get; set; }

        public HomeStayResponse HomeStay { get; set; }
    }
}
