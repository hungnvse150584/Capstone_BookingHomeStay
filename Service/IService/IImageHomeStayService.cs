using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageHomeStay;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IImageHomeStayService
    {
        Task<BaseResponse<UploadImageRequest>> UploadImageAsync(UploadImageRequest request);

    }
}
