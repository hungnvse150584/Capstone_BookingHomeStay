using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.HomeStays
{
    public class GetTrendingHomeStay
    {
        public SingleHomeStayResponse HomeStays { get; set; }

        public double AvgRating { get; set; }

        public int RatingCount { get; set; }
    }
}
