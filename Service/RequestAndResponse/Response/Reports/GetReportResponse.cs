using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Reports
{
    public class GetReportResponse
    {
        [Key]
        public int ReportID { get; set; }
    }
}
