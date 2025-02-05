using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Properties
{
    public class CreatePropertyRequest
    {
        public int numberBedRoom { get; set; }

        public int numberBathRoom { get; set; }

        public int numberWifi { get; set; }

        public string AccountID { get; set; }
    }
}
