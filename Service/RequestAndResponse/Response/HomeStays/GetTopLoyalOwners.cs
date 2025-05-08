using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.HomeStays
{
    public class GetTopLoyalOwners
    {
       public string accountID { get; set; }
       public string ownerName { get; set; }
       public int totalHomeStays { get; set; }
    }
}
