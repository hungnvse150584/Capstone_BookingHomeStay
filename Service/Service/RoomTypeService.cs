﻿using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IMapper _mapper;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IHomeStayRentalRepository _homeStayRentalRepository; // Thêm repository

        public RoomTypeService(
            IMapper mapper,
            IRoomTypeRepository roomTypeRepository,
            IHomeStayRentalRepository homeStayRentalRepository) // Thêm vào constructor
        {
            _mapper = mapper;
            _roomTypeRepository = roomTypeRepository;
            _homeStayRentalRepository = homeStayRentalRepository;
        }

        public async Task<BaseResponse<RoomTypes>> CreateRoomType(CreateRoomTypeRequest request, int homeStayRentalId)
        {
            try
            {
                // Kiểm tra HomeStayRentalID có tồn tại và RentWhole = false
                var homeStayRental = await _homeStayRentalRepository.GetByIdAsync(homeStayRentalId);
                if (homeStayRental == null)
                {
                    return new BaseResponse<RoomTypes>(
                        "HomeStayRental not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                if (homeStayRental.RentWhole)
                {
                    return new BaseResponse<RoomTypes>(
                        "Cannot create RoomType because RentWhole is true!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                var roomType = _mapper.Map<RoomTypes>(request);
                roomType.CreateAt = DateTime.UtcNow;
                roomType.UpdateAt = DateTime.UtcNow;
                roomType.HomeStayRentalID = homeStayRentalId;

                await _roomTypeRepository.AddAsync(roomType);
                await _roomTypeRepository.SaveChangesAsync();

                return new BaseResponse<RoomTypes>(
                    "RoomType created successfully!",
                    StatusCodeEnum.Created_201,
                    roomType);
            }
            catch (Exception ex)
            {
                return new BaseResponse<RoomTypes>(
                    $"An error occurred while creating RoomType: {ex.Message}. InnerException: {ex.InnerException?.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        public async Task<BaseResponse<IEnumerable<GetAllRoomType>>> GetAllRoomTypes()
        {
            IEnumerable<RoomTypes> roomType = await _roomTypeRepository.GetAllAsync();
            if (roomType == null)
            {
                return new BaseResponse<IEnumerable<GetAllRoomType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var roomTypes = _mapper.Map<IEnumerable<GetAllRoomType>>(roomType);
            if (roomTypes == null)
            {
                return new BaseResponse<IEnumerable<GetAllRoomType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllRoomType>>("Get all RoomType as base success",
                StatusCodeEnum.OK_200, roomTypes);
        }
    }
}
