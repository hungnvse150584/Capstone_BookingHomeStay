using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.ImageHomeStay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.HomeStays
{
    public class SingleHomeStayResponse
    {
        public int HomeStayID { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public GetAccountUser Account { get; set; }

        public IEnumerable<ImageHomeStayResponse> ImageHomeStays { get; set; }
    }
}
