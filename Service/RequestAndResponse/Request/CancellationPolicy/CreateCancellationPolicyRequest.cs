using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.CancellationPolicy
{
    public class CreateCancellationPolicyRequest
    {
        [Required]
        public int DayBeforeCancel { get; set; }

        [Required]
        public double RefundPercentage { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public int HomeStayID { get; set; }
    }
}
