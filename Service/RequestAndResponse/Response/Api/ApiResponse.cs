using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Api
{
    public class ApiResponse
    {
        public class ApiResponses<T>
        {
            public IEnumerable<T> Data { get; set; }
            public string HomestayName { get; set; }
        }
    }
}
