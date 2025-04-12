using AutoMapper;
using BusinessObject.IdentityModel;
using BusinessObject.Model;
using Microsoft.AspNetCore.Identity;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Staffs;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Staffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class StaffService : IStaffService
    {
        private readonly IMapper _mapper;
        private readonly IStaffRepository _staffRepository;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        public StaffService(IMapper mapper, IStaffRepository staffRepository, 
                            UserManager<Account> userManager,
                            RoleManager<IdentityRole> roleManager,
                            ITokenService tokenService)
        {
            _mapper = mapper;
            _staffRepository = staffRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<BaseResponse<Staff>> CreateStaffAccount(CreateStaffRequest request)
        {
            // 1. Tạo tài khoản Staff
            var accountApp = new Account
            {
                UserName = request.Username,
                Name = request.Name,
                Email = request.Email,
                Address = request.Address,
                Phone = request.Phone,
                Status = true,
                EmailConfirmed = true
            };

            // 2. Kiểm tra email đã tồn tại chưa
            var existUser = await _userManager.FindByEmailAsync(request.Email);
            if (existUser != null)
            {
                return new BaseResponse<Staff>("Email already exists!", StatusCodeEnum.Conflict_409, null);
            }

            // 3. Tạo tài khoản
            var createdUser = await _userManager.CreateAsync(accountApp, request.Password);
            if (!createdUser.Succeeded)
            {
                return new BaseResponse<Staff>("Cannot create account!", StatusCodeEnum.Conflict_409, null);
            }

            // 4. Thêm role STAFF
            var roleResult = await _userManager.AddToRoleAsync(accountApp, "STAFF");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(accountApp);
                return new BaseResponse<Staff>("Cannot assign role STAFF!", StatusCodeEnum.Conflict_409, null);
            }

            // 5. Tạo Staff, liên kết với Owner (AccountID là owner), và Homestay
            var staff = new Staff
            {
                StaffIdAccount = accountApp.Id,       // tài khoản staff vừa tạo
                Username = request.Username,
                Email = accountApp.Email,
                Password = null,
                Address = accountApp.Address,
                Phone = accountApp.Phone,
                HomeStayID = request.HomeStayID,      // homestay staff quản lý
                StaffName = accountApp.Name,
                AccountID = request.AccountID         // ID của chủ homestay (owner)
            };

            var result = await _staffRepository.AddAsync(staff);
            if (result == null)
            {
                await _userManager.DeleteAsync(accountApp);
                return new BaseResponse<Staff>("Cannot create staff record!", StatusCodeEnum.Conflict_409, null);
            }

            return new BaseResponse<Staff>("Create Staff Account Successfully!", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<IEnumerable<GetAllStaff>>> GetAllStaffByHomeStay(int homeStayID)
        {
            IEnumerable<Staff> Staff = await _staffRepository.GetAllStaffByHomeStay(homeStayID);
            if (Staff == null)
            {
                return new BaseResponse<IEnumerable<GetAllStaff>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var Staffs = _mapper.Map<IEnumerable<GetAllStaff>>(Staff);
            if (Staffs == null)
            {
                return new BaseResponse<IEnumerable<GetAllStaff>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllStaff>>("Get all HomeStay as base success",
                StatusCodeEnum.OK_200, Staffs);
        }

        public async Task<BaseResponse<IEnumerable<GetAllStaff>>> GetAllStaffByOwner(string accountID)
        {
            IEnumerable<Staff> Staff = await _staffRepository.GetAllStaffByOwner(accountID);
            if (Staff == null)
            {
                return new BaseResponse<IEnumerable<GetAllStaff>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var Staffs = _mapper.Map<IEnumerable<GetAllStaff>>(Staff);
            if (Staffs == null)
            {
                return new BaseResponse<IEnumerable<GetAllStaff>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllStaff>>("Get all HomeStay as base success",
                StatusCodeEnum.OK_200, Staffs);
        }

        public async Task<BaseResponse<GetAllStaff>> GetStaffByID(string accountID)
        {
            Staff staff = await _staffRepository.GetStaffByID(accountID);
            var result = _mapper.Map<GetAllStaff>(staff);
            return new BaseResponse<GetAllStaff>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<Staff>> UpdateStaffAccount(string userId, UpdateStaffRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponse<Staff>("Cannot find staff record!", StatusCodeEnum.Conflict_409, null);
            }

            var staff = await _staffRepository.GetStaffByID(userId);
            if (staff == null)
            {
                return new BaseResponse<Staff>("Cannot find staff record!", StatusCodeEnum.Conflict_409, null);
            }

            if (!string.IsNullOrEmpty(request.Username))
            {
                user.UserName = request.Username;
                staff.Username = request.Username;
            }
            
            user.Email = request.Email ?? user.Email;
            user.Name = request.Name ?? user.Name;
            user.Address = request.Address ?? user.Address;
            user.Phone = request.Phone ?? user.Phone;
            
            staff.Email = request.Email ?? staff.Email;
            staff.StaffName = request.Name ?? staff.StaffName;
            staff.Phone = request.Phone ?? staff.Phone;
            staff.Address = request.Address ?? staff.Address;
            staff.HomeStayID = request.HomeStayID ?? staff.HomeStayID;
            
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return new BaseResponse<Staff>("Cannot update staff account!", StatusCodeEnum.Conflict_409, null);
            }
            
            var updateStaff = await _staffRepository.UpdateAsync(staff);

            if (updateStaff == null)
            {
                return new BaseResponse<Staff>("Cannot update staff account!", StatusCodeEnum.Conflict_409, null);
            }

            return new BaseResponse<Staff>("Update Staff Account Successfully!", StatusCodeEnum.OK_200, null);
        }
    }
}
