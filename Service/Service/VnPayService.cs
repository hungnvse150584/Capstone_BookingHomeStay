using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.IService;
using Service.RequestAndResponse.Request.VnPayModel;
using Service.RequestAndResponse.Response.VNPayModel;
using Service.RequestAndResponse.VNPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreatePaymentUrl(HttpContext context, VnPayRequestModel requestModel)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (requestModel.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không 
                                                                                        //mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND 
                                                                                        //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY 
                                                                                        //là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", requestModel.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:Currency"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
            //
            string orderInfo = string.Empty;
            string txnRef = string.Empty;
            // Determine the booking scenario
            bool hasBookingID = requestModel.BookingID > 0;
            bool hasBookingServiceID = requestModel.BookingServiceID > 0;

            if (hasBookingID && hasBookingServiceID)
            {
                // Both HomeStay and Service booked
                orderInfo = $"BookingID:{requestModel.BookingID}, ServiceID:{requestModel.BookingServiceID}";
                txnRef = $"H-{requestModel.BookingID}-S-{requestModel.BookingServiceID}-{tick}";
            }
            else if (hasBookingID)
            {
                // Only HomeStay booked
                orderInfo = $"BookingID:{requestModel.BookingID}";
                txnRef = $"H-{requestModel.BookingID}-{tick}";
            }
            else if (hasBookingServiceID)
            {
                // Only Service booked
                orderInfo = $"ServiceID:{requestModel.BookingServiceID}";
                txnRef = $"S-{requestModel.BookingServiceID}-{tick}";
            }

            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", txnRef);
            var paymentUrl = vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"],
                _configuration["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));

            var vnp_SecureHash = collections.FirstOrDefault(x => x.Key == "vnp_SecureHash").Value;
            if (string.IsNullOrEmpty(vnp_SecureHash))
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                };
            }

            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfor = vnpay.GetResponseData("vnp_OrderInfor");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfor,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode.ToString()
            };
        }
    }
}
