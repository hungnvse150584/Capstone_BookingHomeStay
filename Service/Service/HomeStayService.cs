using AutoMapper;
using Azure;
using BusinessObject.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Provinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service.Service
{
    public class HomeStayService : IHomeStayService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayRepository _homeStayRepository;
        private readonly IStreetRepository _streetRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly IProvinceRepository _provinceRepository;


        public HomeStayService(IMapper mapper, IHomeStayRepository homeStayRepository, IStreetRepository streetRepository, IWardRepository wardRepository, IDistrictRepository districtRepository, IProvinceRepository provinceRepository)
        {
            _mapper = mapper;
            _homeStayRepository = homeStayRepository;
            _streetRepository = streetRepository;
            _wardRepository = wardRepository;
            _districtRepository = districtRepository;
            _provinceRepository = provinceRepository;
        }

        public async Task<BaseResponse<HomeStayResponse>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status)
        {
            var homestay = await _homeStayRepository.ChangeHomeStayStatus(homestayId, status);
            var homestayResponse = _mapper.Map<HomeStayResponse>(homestay);
            return new BaseResponse<HomeStayResponse>("Change status ok", StatusCodeEnum.OK_200, homestayResponse);
        }

        public async Task<BaseResponse<IEnumerable<HomeStayResponse>>> GetAllHomeStayRegisterFromBase()
        {
            IEnumerable<HomeStay> homeStay = await _homeStayRepository.GetAllRegisterHomeStayAsync();
            if (homeStay == null)
            {
                return new BaseResponse<IEnumerable<HomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStays = _mapper.Map<IEnumerable<HomeStayResponse>>(homeStay);
            if (homeStays == null)
            {
                return new BaseResponse<IEnumerable<HomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<HomeStayResponse>>("Get all HomeStay as base success",
                StatusCodeEnum.OK_200, homeStays);
        }

        public async Task<BaseResponse<HomeStayResponse>> GetHomeStayDetailByIdFromBase(int id)
        {
            HomeStay homeStay = await _homeStayRepository.GetHomeStayDetailByIdAsync(id);
            var result = _mapper.Map<HomeStayResponse>(homeStay);
            return new BaseResponse<HomeStayResponse>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<HomeStayResponse>> GetOwnerHomeStayByIdFromBase(string accountId)
        {
            HomeStay homeStay = await _homeStayRepository.GetOwnerHomeStayByIdAsync(accountId);
            var result = _mapper.Map<HomeStayResponse>(homeStay);
            return new BaseResponse<HomeStayResponse>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<List<HomeStay>>> RegisterHomeStay(CreateHomeStayRequest request)
        {
            var streetName = await _streetRepository.GetStreetNameById(request.Location.StreetID);
            var wardName = await _wardRepository.GetWardNameById(request.Location.WardID);
            var districtName = await _districtRepository.GetDistrictNameById(request.Location.DistrictID);
            var provinceName = await _provinceRepository.GetProvinceNameById(request.Location.ProvinceID);

            var fullAddress = $"{request.Location.numberHouse}, {streetName}, {wardName}, {districtName}, {provinceName}, {request.Location.postalCode}, {request.Location.Cooordinate}";

            var homestayRegister = new List<HomeStay>
            {
                new HomeStay
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    Status = HomeStayStatus.PendingApproval,
                    Area = request.Area,
                    Location = new Location
                    {
                        numberHouse = request.Location.numberHouse,
                        postalCode = request.Location.postalCode,
                        Cooordinate = request.Location.Cooordinate,
                        StreetID = request.Location.StreetID,
                        WardID = request.Location.WardID,
                        DistrictID = request.Location.DistrictID,
                        ProvinceID = request.Location.ProvinceID,
                        FullAddress = fullAddress
                    },
                    AccountID = request.AccountID,

                }
            };
                
            await _homeStayRepository.AddRange(homestayRegister);
            return new BaseResponse<List<HomeStay>>("Add province as base success", StatusCodeEnum.Created_201, homestayRegister);
        }

        public async Task<BaseResponse<HomeStay>> UpdateHomeStay(int homestayId, UpdateHomeStayRequest request)
        {
            var streetName = await _streetRepository.GetStreetNameById(request.Location.StreetID);
            var wardName = await _wardRepository.GetWardNameById(request.Location.WardID);
            var districtName = await _districtRepository.GetDistrictNameById(request.Location.DistrictID);
            var provinceName = await _provinceRepository.GetProvinceNameById(request.Location.ProvinceID);

            var fullAddress = $"{request.Location.numberHouse}, {streetName}, {wardName}, {districtName}, {provinceName}, {request.Location.postalCode}, {request.Location.Cooordinate}";

            var homeStayExist = await _homeStayRepository.GetHomeStayDetailByIdAsync(homestayId);
          if (homeStayExist == null) 
          {
                return new BaseResponse<HomeStay>("Cannot find HomeStay", StatusCodeEnum.BadGateway_502, null);
          }
            var updatedHomeStay = _mapper.Map(request, homeStayExist);
            updatedHomeStay.CreateAt = homeStayExist.CreateAt; // Keep the original CreateAt
            updatedHomeStay.Status = homeStayExist.Status;     // Keep the original Status
            updatedHomeStay.UpdateAt = DateTime.Now;
            updatedHomeStay.Location.FullAddress = fullAddress;
            await _homeStayRepository.UpdateAsync(updatedHomeStay);

            return new BaseResponse<HomeStay>("Update HomeStay successfully", StatusCodeEnum.OK_200, updatedHomeStay);
        }
    }
}
