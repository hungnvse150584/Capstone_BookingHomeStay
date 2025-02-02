using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Properties
{
    public class GetAllProperties
    {
        public int PropertyID { get; set; }

        public int numberBedRoom { get; set; }

        public int numberBathRoom { get; set; }

        public int numberWifi { get; set; }

        public string AccountID { get; set; }
        public Account Account { get; set; }
    }
}
