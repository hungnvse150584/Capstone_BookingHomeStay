using AutoMapper;
using Azure;
using BusinessObject.Model;
using CloudinaryDotNet;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingDetail;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Service
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICancellationPolicyRepository _cancelltaionRepository;

        public BookingService(IMapper mapper, IBookingRepository bookingRepository,
                              IHomeStayRentalRepository homeStayTypeRepository,
                              IServiceRepository serviceRepository,
                              IBookingServiceRepository bookingServiceRepository,
                              IRoomTypeRepository roomTypeRepository,
                              IRoomRepository roomRepository,
                              IPricingRepository pricingRepository,
                              ICommissionRateRepository commissionRateRepository, 
                              IBookingDetailRepository bookingDetailRepository, 
                              IBookingServiceDetailRepository bookingServiceDetailRepository, 
                              ICancellationPolicyRepository cancelltaionRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _cancelltaionRepository = cancelltaionRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllBookings>>> GetAllBooking(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null)
        {
            IEnumerable<Booking> booking = await _bookingRepository.GetAllBookingAsync(search, date, status, paymentStatus);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetAllBookings>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetAllBookings>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetAllBookings>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllBookings>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        public async Task<BaseResponse<GetCancellationBooking>> GetCancellationBooking(int bookingID)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<GetCancellationBooking>("Get Booking Fail", StatusCodeEnum.BadGateway_502, null);
            }

            if (booking.Status == BookingStatus.Cancelled && booking.paymentStatus == PaymentStatus.Refunded)
            {
                var cancellationRate = await _cancelltaionRepository.GetCancellationPolicyByHomeStayAsync(booking.HomeStayID);
                if (cancellationRate == null)
                {
                    return new BaseResponse<GetCancellationBooking>("Get CancellationPolicy Fail", StatusCodeEnum.BadGateway_502, null);
                }

                var bookingResponse = _mapper.Map<GetCancellationBooking>(booking);

                var checkInDate = booking.BookingDetails.First().CheckInDate;

                var daysBeforeCheckIn = (checkInDate - DateTime.Now).TotalDays;

                bookingResponse.CancelFee = daysBeforeCheckIn >= cancellationRate.DayBeforeCancel
                ? booking.Total * (1 - cancellationRate.RefundPercentage)
                : booking.Total; // Không hoàn tiền nếu hủy sát ngày

                return new BaseResponse<GetCancellationBooking>("Get Success Cancellation Booking", StatusCodeEnum.OK_200, bookingResponse);
            }
            else
            {
                return new BaseResponse<GetCancellationBooking>("Booking is not cancelled or refunded", StatusCodeEnum.BadRequest_400, null);
            }
        }

        public async Task<BaseResponse<Booking>> GetBookingsById(int bookingID)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<Booking>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }

            return new BaseResponse<Booking>("Get all bookings as base success",
                StatusCodeEnum.OK_200, booking);
        }

        // For Admin and Owner DashBoard
        public async Task<BaseResponse<GetStaticBookings>> GetStaticBookings()
        {
            var booking = await _bookingRepository.GetStaticBookings();
            var response = new GetStaticBookings
            {
                bookings = booking.bookings,
                bookingsReturnOrCancell = booking.bookingsReturnOrCancell,
                bookingsCancell = booking.bookingsCancell,
                bookingsComplete = booking.bookingsComplete,
                bookingsReport = booking.bookingsReport,
                bookingsReturnRefund = booking.bookingsReturnRefund
            };
            if (response == null)
            {
                return new BaseResponse<GetStaticBookings>("Get All Fail", StatusCodeEnum.BadGateway_502, response);
            }
            return new BaseResponse<GetStaticBookings>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<GetTopHomeStayBookingInMonth>> GetTopHomeStayBookingInMonth()
        {
            var bookings = await _bookingRepository.GetTopHomeStayBookingInMonthAsync();
            var response = new GetTopHomeStayBookingInMonth
            {
                topHomeStays = bookings.Select(o => (o.homeStayName, o.QuantityOfBooking)).ToList()
            };
            if (response == null)
            {
                return new BaseResponse<GetTopHomeStayBookingInMonth>("Get All Fail", StatusCodeEnum.BadGateway_502, response);
            }
            return new BaseResponse<GetTopHomeStayBookingInMonth>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>> GetTotalBookingsTotalBookingsAmount(DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            if (startDate == default(DateTime).Date || endDate == default(DateTime).Date)
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Please input time", StatusCodeEnum.NotImplemented_501, null);
            }

            if (startDate >= endDate)
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Please input endDate > startDate", StatusCodeEnum.NotAcceptable_406, null);
            }

            var total = await _bookingRepository.GetTotalBookingsTotalBookingsAmount(startDate, endDate, timeSpanType);
            var response = total.Select(p => new GetTotalBookingsTotalBookingsAmount
            {
                span = p.span,
                totalBookings = p.totalBookings,
                totalBookingsAmount = p.totalBookingsAmount
            }).ToList();
            if (response == null || !response.Any())
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Get Total Fail", StatusCodeEnum.BadRequest_400, null);
            }
            return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<IEnumerable<GetBookingByHomeStay>>> GetBookingsByHomeStayId(int homeStayID)
        {
            IEnumerable<Booking> booking = await _bookingRepository.GetBookingsByHomeStayId(homeStayID);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByHomeStay>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetBookingByHomeStay>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByHomeStay>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetBookingByHomeStay>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        public async Task<BaseResponse<IEnumerable<GetBookingByAccount>>> GetBookingsByAccountId(string accountId)
        {
            IEnumerable<Booking> booking = await _bookingRepository.GetBookingsByAccountId(accountId);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByAccount>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetBookingByAccount>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByAccount>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetBookingByAccount>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        





        /*public async Task<BaseResponse<Booking>> CanncelledBooking(int bookingID, int bookingServiceID)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<Booking>("Cannot find your Booking!",
                        StatusCodeEnum.NotFound_404, null);
            }




        }*/
    }
}
