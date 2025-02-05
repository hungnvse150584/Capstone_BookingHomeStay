using BusinessObject.Model;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.HomeStayType
{
    public class GetAllHomeStayType
    {
        public int HomeStayTypesID { get; set; }
        
        public string Name { get; set; }
        
        public string? Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public double UnitPrice { get; set; }

        public double RentPrice { get; set; }

        public bool Status { get; set; }

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public int MaxPeople { get; set; }

        public int? HomeStayID { get; set; }
        public HomeStayResponse? HomeStay { get; set; }

        public int? PropertyID { get; set; }
        public GetAllProperties? Property { get; set; }
    }
}
