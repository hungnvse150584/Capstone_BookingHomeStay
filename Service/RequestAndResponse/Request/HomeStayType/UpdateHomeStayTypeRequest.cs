using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.HomeStayType
{
    public class UpdateHomeStayTypeRequest
    {
        [Key]
        public int HomeStayTypesID { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public int? HomeStayID { get; set; }

        public int? PropertyID { get; set; }

        public double UnitPrice { get; set; }

        public double RentPrice { get; set; }

        public bool Status { get; set; }

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public int MaxPeople { get; set; }
    }
}
