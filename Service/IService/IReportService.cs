using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Report;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.Reports;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IReportService
    {
        Task<BaseResponse<IEnumerable<GetAllReport>>> GetAllReport();
        Task<BaseResponse<GetAllReport>> GetReportById(int id);
        Task<BaseResponse<CreateReport>> CreateReport(CreateReport createReport);
        Task<BaseResponse<UpdateReport>> UpdateReport(int id, UpdateReport updateReport);
        Task<BaseResponse<string>> DeleteReport(int id);
        Task<BaseResponse<IEnumerable<GetAllReport>>> SearchReport(string search, int pageIndex, int pageSize);
    }
}
