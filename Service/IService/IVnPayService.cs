using Microsoft.AspNetCore.Http;
using Service.RequestAndResponse.Request.VnPayModel;
using Service.RequestAndResponse.Response.VNPayModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IVnPayService
    {
        public string CreatePaymentUrl(HttpContext context, VnPayRequestModel requestModel);
        public VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
