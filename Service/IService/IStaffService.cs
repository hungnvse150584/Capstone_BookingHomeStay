using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Staffs;
using Service.RequestAndResponse.Response.Staffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IStaffService
    {
        Task<BaseResponse<Staff>> CreateStaffAccount(CreateStaffRequest request);
        Task<BaseResponse<Staff>> UpdateStaffAccount(string userId,UpdateStaffRequest request);
        Task<BaseResponse<IEnumerable<GetAllStaff>>> GetAllStaffByHomeStay(int homeStayID);
        Task<BaseResponse<IEnumerable<GetAllStaff>>> GetAllStaffByOwner(string accountID);
        Task<BaseResponse<GetAllStaff>> GetStaffByID(string accountID);
    }
}
