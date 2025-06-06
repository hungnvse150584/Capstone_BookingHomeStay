﻿using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet;
using GreenRoam.Ultilities;
using Microsoft.Identity.Client;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Service
{
    public class BookingForService : IBookingForService
    {
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingServiceRepository _bookingServiceRepository;
        private readonly ICommissionRateRepository _commissionRateRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingServiceDetailRepository _bookingServiceDetailRepository;

        public BookingForService(IMapper mapper, IBookingRepository bookingRepository,
                             IServiceRepository serviceRepository,
                             IBookingServiceRepository bookingServiceRepository,
                             ICommissionRateRepository commissionRateRepository,
                             IBookingServiceDetailRepository bookingServiceDetailRepository
                             )
        {
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _bookingServiceRepository = bookingServiceRepository;
            _commissionRateRepository = commissionRateRepository;
            _bookingServiceDetailRepository = bookingServiceDetailRepository;
            _bookingRepository = bookingRepository;
        }

        //Tạo mã code 8 ký tự ko trùng lập
        private string GenerateShortBookingCode(int length = 8)
        {
            var guidBytes = Guid.NewGuid().ToByteArray();
            var base64 = Convert.ToBase64String(guidBytes);

            var safeCode = base64
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "")
                .ToUpper();

            return safeCode.Substring(0, length);
        }

        private async Task<string> GenerateUniqueBookingServiceCodeAsync()
        {
            string code;
            do
            {
                code = GenerateShortBookingCode();
            }
            while (await _bookingServiceRepository.ExistsBookingServiceCodeAsync(code));

            return code;
        }

        public async Task<BaseResponse<IEnumerable<GetAllBookingServices>>> GetAllBookingService(string? search, DateTime? date = null, BookingServicesStatus? status = null, PaymentServicesStatus? paymentStatus = null)
        {
            IEnumerable<BookingServices> booking = await _bookingServiceRepository.GetAllBookingServicesAsync(search, date, status, paymentStatus);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetAllBookingServices>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetAllBookingServices>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetAllBookingServices>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllBookingServices>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }


        public async Task<BaseResponse<BookingServices>> CreateServiceBooking(CreateBookingServices bookingServiceRequest, PaymentServicesMethod paymentServicesMethod)
        {
            var nowVN = DateTimeHelper.NowVietnamTime();
            TimeZoneInfo vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var bookingExist = await _bookingRepository.GetBookingByIdAsync(bookingServiceRequest.BookingID);
            if (bookingExist == null)
            {
                return new BaseResponse<BookingServices>("Cannot Find any Booking!",
                    StatusCodeEnum.NotFound_404, null);
            }

            switch (bookingExist.Status)
            {
                case BookingStatus.Cancelled:
                    return new BaseResponse<BookingServices>("This booking was canceled, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                case BookingStatus.Completed:
                    return new BaseResponse<BookingServices>("This booking was completed, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                default:
                    break;
            }

            switch (bookingExist.paymentStatus)
            {
                case PaymentStatus.Refunded:
                    return new BaseResponse<BookingServices>("This booking was refunded, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                default:
                    break;
            }

            var UnpaidServices = await _bookingServiceRepository.GetUnpaidServicesByAccountId(bookingServiceRequest.AccountID);
            if (UnpaidServices != null)
            {
                return new BaseResponse<BookingServices>("There still have a booking services need to pay, complete before start a new BookingServices",
                    StatusCodeEnum.Conflict_409, UnpaidServices);
            }

            var bookingServices = new BookingServices
            {
                BookingServiceCode = await GenerateUniqueBookingServiceCodeAsync(),
                BookingServicesDate = nowVN,
                AccountID = bookingServiceRequest.AccountID,
                BookingID = bookingServiceRequest.BookingID,
                Status = BookingServicesStatus.Pending,
                HomeStayID = bookingServiceRequest.HomeStayID,
                PaymentServiceStatus = PaymentServicesStatus.Pending,
                isPaidWithBooking = false,
                PaymentServicesMethod = paymentServicesMethod == PaymentServicesMethod.Cod ? PaymentServicesMethod.Cod : PaymentServicesMethod.VnPay,
                BookingServicesDetails = new List<BookingServicesDetail>()
            };

            var duplicateService = bookingServiceRequest.BookingServicesDetails
                                                        .GroupBy(x => x.ServicesID)
                                                        .FirstOrDefault(g => g.Count() > 1);

            if (duplicateService != null)
            {
                return new BaseResponse<BookingServices>(
                    $"Duplicate service detected with ServiceID: {duplicateService.Key}. Please ensure each service is only added once.",
                    StatusCodeEnum.Conflict_409, null);
            }

            foreach (var serviceDetailRequest in bookingServiceRequest.BookingServicesDetails)
            {
                var service = await _serviceRepository.GetByIdAsync(serviceDetailRequest.ServicesID);
                if (service == null)
                {
                    return new BaseResponse<BookingServices>("Service not found, please check the service ID.",
                        StatusCodeEnum.NotFound_404, null);
                }

                if (serviceDetailRequest.Quantity <= 0)
                {
                    return new BaseResponse<BookingServices>("Quantity must be > 0, please check again.",
                        StatusCodeEnum.NotFound_404, null);
                }

                double totalServiceAmount = 0;

                if (service.ServiceType == ServiceType.Hour)
                {
                    if (serviceDetailRequest.RentHour is null || serviceDetailRequest.RentHour <= 0)
                        return new BaseResponse<BookingServices>("RentHour must be > 0 for Hour service type.", StatusCodeEnum.BadRequest_400, null);

                    totalServiceAmount = service.servicesPrice * serviceDetailRequest.Quantity * serviceDetailRequest.RentHour.Value;
                }

                else if (service.ServiceType == ServiceType.Day)
                {
                    if (serviceDetailRequest.dayRent <= 0 || serviceDetailRequest.dayRent == null)
                    {
                        return new BaseResponse<BookingServices>("Please input dayRent > 0", StatusCodeEnum.BadRequest_400, null);
                    }

                    totalServiceAmount = service.servicesPrice * serviceDetailRequest.Quantity * serviceDetailRequest.dayRent.Value;
                }
                else
                {
                    totalServiceAmount = service.servicesPrice * serviceDetailRequest.Quantity;
                }

                var bookingServiceDetail = new BookingServicesDetail
                {
                    Quantity = serviceDetailRequest.Quantity,
                    unitPrice = service.servicesPrice,
                    TotalAmount = totalServiceAmount,
                    ServicesID = serviceDetailRequest.ServicesID,
                    RentHour = service.ServiceType == ServiceType.Hour ? serviceDetailRequest.RentHour : null,
                    dayRent = service.ServiceType == ServiceType.Day ? serviceDetailRequest.dayRent : null,

                };
                bookingServices.BookingServicesDetails.Add(bookingServiceDetail);
            }
            var commissionrate = await _commissionRateRepository.GetCommissionByHomeStayAsync(bookingExist.HomeStayID);
            if (commissionrate == null)
            {
                return new BaseResponse<BookingServices>("Cannot find the HomeStay Commission, please try again!",
                            StatusCodeEnum.Conflict_409, null);
            }
            if (commissionrate.PlatformShare <= 0 || commissionrate.PlatformShare > 1)
            {
                return new BaseResponse<BookingServices>("Invalid PlatformShare value, please check commission settings!",
                            StatusCodeEnum.Conflict_409, null);
            }
            var totalAmount = bookingServices.BookingServicesDetails.Sum(detail => detail.TotalAmount);
            var deposit = commissionrate.PlatformShare * totalAmount;
            var remaining = totalAmount - deposit;
            bookingServices.Total = totalAmount;
            bookingServices.bookingServiceDeposit = deposit;
            bookingServices.remainingBalance = remaining;
            await _bookingServiceRepository.AddBookingServicesAsync(bookingServices);
            return new BaseResponse<BookingServices>("Booking Services Successfully!!!", StatusCodeEnum.Created_201, bookingServices);
        }

        public async Task<BaseResponse<UpdateBookingService>> UpdateBookingServices(int bookingServiceID, UpdateBookingService request)
        {
            var bookingExist = await _bookingRepository.GetBookingByIdAsync(request.BookingID);
            if (bookingExist == null)
            {
                return new BaseResponse<UpdateBookingService>("Cannot Find any Booking!", StatusCodeEnum.NotFound_404, null);
            }

            switch (bookingExist.Status)
            {
                case BookingStatus.Cancelled:
                    return new BaseResponse<UpdateBookingService>("This booking was canceled, cannot update more services!", StatusCodeEnum.Conflict_409, null);
                case BookingStatus.Completed:
                    return new BaseResponse<UpdateBookingService>("This booking was completed, cannot update more services!", StatusCodeEnum.Conflict_409, null);
            }

            switch (bookingExist.paymentStatus)
            {
                case PaymentStatus.Refunded:
                    return new BaseResponse<UpdateBookingService>("This booking was refunded, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                default:
                    break;
            }

            var existingBookingService = await _bookingServiceRepository.GetBookingServicesByIdAsync(bookingServiceID);
            if (existingBookingService == null)
            {
                return new BaseResponse<UpdateBookingService>("Cannot find the specified booking service!", StatusCodeEnum.NotFound_404, null);
            }

            if (existingBookingService.BookingID != bookingExist.BookingID)
            {
                return new BaseResponse<UpdateBookingService>("The booking service does not belong to this booking!", StatusCodeEnum.Conflict_409, null);
            }

            if (existingBookingService.Status != BookingServicesStatus.Pending)
            {
                return new BaseResponse<UpdateBookingService>("This booking service is not eligible for updates!", StatusCodeEnum.Conflict_409, null);
            }

            switch (existingBookingService.Status)
            {
                case BookingServicesStatus.Cancelled:
                    return new BaseResponse<UpdateBookingService>("This bookingservice was canceled, cannot update more services!", StatusCodeEnum.Conflict_409, null);
                case BookingServicesStatus.Completed:
                    return new BaseResponse<UpdateBookingService>("This bookingservice was completed, cannot update more services!", StatusCodeEnum.Conflict_409, null);
            }

            switch (existingBookingService.PaymentServiceStatus)
            {
                case PaymentServicesStatus.FullyPaid:
                    return new BaseResponse<UpdateBookingService>("This bookingservice was paid, cannot update more services!", StatusCodeEnum.Conflict_409, null);
                case PaymentServicesStatus.Deposited:
                    return new BaseResponse<UpdateBookingService>("This bookingservice was paid the deposit, cannot update more services!", StatusCodeEnum.Conflict_409, null);
                case PaymentServicesStatus.Refunded:
                    return new BaseResponse<UpdateBookingService>("This bookingservice was refunded, cannot update more services!", StatusCodeEnum.Conflict_409, null);
            }

            if (request.BookingServicesDetails == null || !request.BookingServicesDetails.Any())
            {
                return new BaseResponse<UpdateBookingService>("No service details provided for update!", StatusCodeEnum.BadRequest_400, null);
            }

            var duplicateService = request.BookingServicesDetails
                                    .GroupBy(d => d.ServicesID)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key)
                                    .FirstOrDefault();

            if (duplicateService != 0) // Nếu có service trùng nhau
            {
                return new BaseResponse<UpdateBookingService>(
                    $"Service with ID {duplicateService} is duplicated. Please choose different services or adjust the quantity.",
                    StatusCodeEnum.Conflict_409, null);
            }

            var updatedDetailIds = request.BookingServicesDetails
                                        .Select(d => d.ServiceDetailID)
                                        .Where(id => id.HasValue)
                                        .Select(id => id.Value)
                                        .ToList();

            var detailsToRemove = await _bookingServiceDetailRepository.GetBookingServiceDetailsToRemoveAsync(bookingServiceID, updatedDetailIds);

            if (detailsToRemove.Any())
            {
                await _bookingServiceDetailRepository.DeleteBookingServiceDetailAsync(detailsToRemove);
            }

            foreach (var updatedServiceDetails in request.BookingServicesDetails)
            {
                var service = await _serviceRepository.GetByIdAsync(updatedServiceDetails.ServicesID);
                if (service == null)
                {
                    return new BaseResponse<UpdateBookingService>($"Cannot find service with ID {updatedServiceDetails.ServicesID}", StatusCodeEnum.BadRequest_400, null);
                }

                if (updatedServiceDetails.Quantity <= 0)
                {
                    return new BaseResponse<UpdateBookingService>($"Quantity must be > 0, Please Check again!", StatusCodeEnum.BadRequest_400, null);
                }

                if (service.servicesPrice <= 0)
                {
                    return new BaseResponse<UpdateBookingService>($"Service with ID {service.ServicesID} is missing price info.", StatusCodeEnum.Conflict_409, null);
                }

                double totalServiceAmount = 0;
                if (service.ServiceType == ServiceType.Hour)
                {
                    if (updatedServiceDetails.RentHour is null || updatedServiceDetails.RentHour <= 0)
                        return new BaseResponse<UpdateBookingService>("RentHour must be > 0 for Hour service.", StatusCodeEnum.BadRequest_400, null);

                    totalServiceAmount = service.servicesPrice * updatedServiceDetails.Quantity * updatedServiceDetails.RentHour.Value;
                }
                else if (service.ServiceType == ServiceType.Day)
                {
                    if (updatedServiceDetails.dayRent == null || updatedServiceDetails.dayRent <= 0)
                        return new BaseResponse<UpdateBookingService>("Please input dayRent > 0", StatusCodeEnum.BadRequest_400, null);

                    totalServiceAmount = service.servicesPrice * updatedServiceDetails.Quantity * updatedServiceDetails.dayRent.Value;
                }
                else
                {
                    // Dịch vụ mặc định theo số lượng
                    totalServiceAmount = service.servicesPrice * updatedServiceDetails.Quantity;
                }

                // Check if we are updating an existing service detail
                if (updatedServiceDetails.ServiceDetailID.HasValue)
                {
                    var existingDetail = existingBookingService.BookingServicesDetails
                        .FirstOrDefault(d => d.BookingServicesDetailID == updatedServiceDetails.ServiceDetailID.Value);

                    if (existingDetail == null)
                    {
                        return new BaseResponse<UpdateBookingService>($"Cannot find existing service detail with ID {updatedServiceDetails.ServiceDetailID}", StatusCodeEnum.BadRequest_400, null);
                    }

                    if (existingDetail != null)
                    {
                        existingDetail.Quantity = updatedServiceDetails.Quantity;
                        existingDetail.unitPrice = service.servicesPrice;
                        existingDetail.TotalAmount = totalServiceAmount;
                        existingDetail.RentHour = service.ServiceType == ServiceType.Hour ? updatedServiceDetails.RentHour : null;
                        existingDetail.dayRent = service.ServiceType == ServiceType.Day ? updatedServiceDetails.dayRent : null;
                        existingDetail.ServicesID = updatedServiceDetails.ServicesID;
                    }
                }
                else // Adding a new service detail
                {
                    bool isServiceExists = existingBookingService.BookingServicesDetails
                    .Any(d => d.ServicesID == updatedServiceDetails.ServicesID);

                    if (isServiceExists)
                    {
                        return new BaseResponse<UpdateBookingService>(
                            "This Service is already choosen, Please chooose another Services or change the quantity of that Service ",
                            StatusCodeEnum.Conflict_409, null);
                    }

                    existingBookingService.BookingServicesDetails.Add(new BookingServicesDetail
                    {
                        ServicesID = updatedServiceDetails.ServicesID,
                        unitPrice = service.servicesPrice,
                        Quantity = updatedServiceDetails.Quantity,
                        RentHour = service.ServiceType == ServiceType.Hour ? updatedServiceDetails.RentHour : null,
                        dayRent = service.ServiceType == ServiceType.Day ? updatedServiceDetails.dayRent : null,
                        TotalAmount = totalServiceAmount,
                    });
                }
            }
            var commissionrate = await _commissionRateRepository.GetCommissionByHomeStayAsync(bookingExist.HomeStayID);
            if (commissionrate == null)
            {
                return new BaseResponse<UpdateBookingService>("Cannot find the HomeStay Commission, please try again!",
                            StatusCodeEnum.Conflict_409, null);
            }
            if (commissionrate.PlatformShare <= 0 || commissionrate.PlatformShare > 1)
            {
                return new BaseResponse<UpdateBookingService>("Invalid PlatformShare value, please check commission settings!",
                            StatusCodeEnum.Conflict_409, null);
            }

            var totalAmount = existingBookingService.BookingServicesDetails.Sum(detail => detail.TotalAmount);
            var deposit = commissionrate.PlatformShare * totalAmount;
            var remaining = totalAmount - deposit;
            existingBookingService.Total = totalAmount;
            existingBookingService.bookingServiceDeposit = deposit;
            existingBookingService.remainingBalance = remaining;
            existingBookingService.isPaidWithBooking = existingBookingService.isPaidWithBooking;

            await _bookingServiceRepository.UpdateBookingServicesAsync(existingBookingService);

            return new BaseResponse<UpdateBookingService>("Booking updated successfully!", StatusCodeEnum.OK_200, request);
        }

        public async Task<BaseResponse<BookingServices>> GetBookingServicesById(int bookingID)
        {
            var booking = await _bookingServiceRepository.GetBookingServicesByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<BookingServices>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }

            return new BaseResponse<BookingServices>("Get all bookings as base success",
                StatusCodeEnum.OK_200, booking);
        }

        public async Task<BaseResponse<BookingServices>> GetBookingServiceById(int? bookingID)
        {
            var booking = await _bookingServiceRepository.GetBookingServiceByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<BookingServices>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }

            return new BaseResponse<BookingServices>("Get all bookings as base success",
                StatusCodeEnum.OK_200, booking);
        }

        public async Task<BaseResponse<IEnumerable<GetBookingServiceByAccount>>> GetBookingServiceByAccountId(string accountId)
        {
            IEnumerable<BookingServices> booking = await _bookingServiceRepository.GetBookingServiceByAccountId(accountId);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingServiceByAccount>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetBookingServiceByAccount>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingServiceByAccount>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetBookingServiceByAccount>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        public async Task<BaseResponse<IEnumerable<GetBookingServiceByHomeStay>>> GetBookingServicesByHomeStayId(int homeStayID)
        {
            IEnumerable<BookingServices> booking = await _bookingServiceRepository.GetBookingServicesByHomeStayId(homeStayID);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingServiceByHomeStay>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetBookingServiceByHomeStay>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingServiceByHomeStay>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetBookingServiceByHomeStay>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        public async Task<BaseResponse<List<GetServiceStats>>> GetServiceUsageStatsAsync(int homestayId)
        {
            var rawStats = await _bookingServiceDetailRepository.GetServiceUsageStatsAsync(homestayId);

            var stats = rawStats.Select(tuple => new GetServiceStats
            {
                ServiceName = tuple.serviceName,
                count = tuple.count
            }).ToList();

            if (stats == null || !stats.Any())
            {
                return new BaseResponse<List<GetServiceStats>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<List<GetServiceStats>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, stats);
        }
    }
}
