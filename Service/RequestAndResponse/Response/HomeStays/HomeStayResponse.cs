using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.Locations;

namespace Service.RequestAndResponse.Response.HomeStays
{
    public class HomeStayResponse
    {
        
        public int HomeStayID { get; set; }

        
        public string Name { get; set; }
      
        public string Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        
        public HomeStayStatus Status { get; set; }

        public string Area { get; set; }

        
        public string AccountID { get; set; }
        public Account Account { get; set; }

        
        public int LocationID { get; set; }
        public GetLocation Location { get; set; }
    }
}
