using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.BookingDetail
{
    public class BookingDetailRequest
    {
        public int? homeStayTypeID { get; set; }

        public int? roomTypeID { get; set; }

        public int? roomID { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CheckInDate { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CheckOutDate { get; set; }
    }
}
