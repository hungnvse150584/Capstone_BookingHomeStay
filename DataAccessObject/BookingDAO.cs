using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class BookingDAO : BaseDAO<Booking>
    {
        private readonly GreenRoamContext _context;
        public BookingDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null)
        {
            IQueryable<Booking> bookings = _context.Bookings
                .Include(b => b.Account)
                .Include(b => b.BookingDetails)
                .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.BookingServicesDetails);

            if (date.HasValue)
            {
                bookings = bookings.Where(o => o.BookingDate.Date == date.Value.Date);
            }

            // Apply status filter if provided
            if (status.HasValue)
            {
                bookings = bookings.Where(o => o.Status == status.Value);
            }

            if (paymentStatus.HasValue)
            {
                bookings = bookings.Where(o => o.paymentStatus == paymentStatus.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                bookings = bookings.Where(o => o.Account.Phone.ToString().Contains(search.ToLower()) || o.Account.Email.ToString().Contains(search.ToLower()));

            }

            // Execute the query and return the results as a list
            return await bookings.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByAccountId(string accountId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.HomeStayRentals)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Rooms)
                .Include(b => b.HomeStay)
                .Where(b => b.AccountID == accountId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByHomeStayId(int homeStayID)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.HomeStayRentals)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Rooms)
                .Include(b => b.Account)
                .Where(b => b.HomeStayID == homeStayID)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingStatusByAccountId(string accountId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(b => b.AccountID == accountId && b.Status == BookingStatus.Pending);
        }



        public async Task<Booking?> ChangeBookingStatus(int bookingId, BookingStatus status, PaymentStatus paymentStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                booking.paymentStatus = paymentStatus;
                await _context.SaveChangesAsync();
            }

            return await _context.Bookings.FindAsync(bookingId);
        }

        public async Task AddBookingAsync(Booking order)
        {
            await _context.Bookings.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _context.Bookings
                .Include(o => o.Account)
                .Include(o => o.BookingDetails)
                .Where(o => o.BookingDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _context.Bookings
                .Include(o => o.Account)
                .Include(o => o.BookingDetails)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(o => o.Account)
                .Include(o => o.BookingDetails)
                .ThenInclude(bd => bd.HomeStayRentals)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Rooms)
                .Include(o => o.BookingServices)
                .ThenInclude(o => o.BookingServicesDetails)
                .ThenInclude(o => o.Services)
                .Include(o => o.HomeStay)
                .FirstOrDefaultAsync(o => o.BookingID == bookingId);
        }

        public async Task<Booking?> GetBookingsByIdAsync(int? bookingId)
        {
            return await _context.Bookings
                .Include(o => o.Account)
                .Include(o => o.BookingDetails)
                .ThenInclude(bd => bd.HomeStayRentals)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Rooms)
                .Include(o => o.BookingServices)
                .ThenInclude(o => o.BookingServicesDetails)
                .ThenInclude(o => o.Services)
                .Include(o => o.HomeStay)
                .FirstOrDefaultAsync(o => o.BookingID == bookingId);
        }

        public async Task<Booking?> UpdateBookingWithReportAsync(int bookingId, Booking booking)
        {

            var existBooking = await _context.Bookings.FindAsync(bookingId);
            if (existBooking != null)
            {
                existBooking.ReportID = booking.ReportID;
            }
            await _context.SaveChangesAsync();
            return existBooking;
        }


        //For Admin and Owner DashBoard
        //Admin DashBoard
        public async Task<(int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport)> GetStaticBookings()
        {
            // Lấy ngày hiện tại
            DateTime today = DateTime.Today;

            // Xác định ngày đầu tuần (ngày thứ Hai là ngày đầu tuần)
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            // Xác định ngày cuối tuần (ngày Chủ nhật là ngày cuối tuần)
            DateTime endOfWeek = startOfWeek.AddDays(6);

            int bookingsReturnOrCancell = await _context.Bookings
                 .Where(o => o.BookingDetails
                     .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                            || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                 .Where(o => o.Status == BookingStatus.Cancelled
                          || o.paymentStatus == PaymentStatus.Refunded)
                 .CountAsync();

            int bookings = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .CountAsync();

            int bookingsComplete = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.Status == BookingStatus.Completed)
                                .CountAsync();

            int bookingsCancell = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded)
                                .CountAsync();

            int bookingsReturnRefund = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.paymentStatus == PaymentStatus.Refunded)
                                .CountAsync();

            int bookingsReport = await _context.Bookings
                               .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.ReportID != null)
                                .CountAsync();
            return (bookingsReturnOrCancell, bookings, bookingsComplete, bookingsCancell, bookingsReturnRefund, bookingsReport);
        }

        public async Task<List<(string homeStayName, int QuantityOfBooking)>> GetTopHomeStayBookingInMonthAsync()
        {
            // Lấy ngày hiện tại
            DateTime today = DateTime.Today;

            // Xác định ngày đầu tháng và ngày cuối tháng
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Lấy danh sách các Booking trong khoảng thời gian từ startOfMonth đến endOfMonth
            IEnumerable<Booking> bookingsInMonth = await _context.Bookings
                .Where(o => o.BookingDetails
                .Any(d => (d.CheckInDate >= startOfMonth && d.CheckInDate <= endOfMonth)
                       || (d.CheckOutDate >= startOfMonth && d.CheckOutDate <= endOfMonth)))
                .Include(o => o.BookingDetails) // Include BookingRoomDetails
                .ThenInclude(d => d.HomeStayRentals) // Include HomeStayPlaces from BookingRoomDetails
                .Include(o => o.BookingDetails)
                .ThenInclude(d => d.Rooms) // Include Room (nếu đặt phòng)
                .ToListAsync();

            // Tính tổng số lượng bookings đã đặt của từng homestay
            var homeStayQuantities = bookingsInMonth
                .SelectMany(b => b.BookingDetails)
                .GroupBy(bd => bd.HomeStayRentals != null ? bd.HomeStayRentals.HomeStayID : bd.Rooms.RoomTypes.HomeStayRentals.HomeStayID)
                .Select(g => new
                {
                    HomeStayID = g.Key,
                    BookingCount = g.Count()
                })
                .ToList();

            //Lấy tên HomeStay từ homeStayQuantities
            var homeStayNames = await _context.HomeStays
                .Where(hs => homeStayQuantities.Select(h => h.HomeStayID).Contains(hs.HomeStayID))
                .ToListAsync();

            //Lấy top theo homeStay
            var result = homeStayQuantities
                .Select(h =>
                {
                    var homeStayName = homeStayNames
                        .FirstOrDefault(hs => hs.HomeStayID == h.HomeStayID)?.Name; // Tìm tên HomeStay
                    return (homeStayName, h.BookingCount);
                })
                .OrderByDescending(h => h.BookingCount)
                .Take(4)
                .ToList();

            // Chuyển đổi kết quả sang List<(string homeStayName, int QuantityOfBooking)>
            List<(string homeStayName, int QuantityOfBooking)> topHomeStay = result
                .Select(p => (p.homeStayName, p.BookingCount))
                .ToList();

            return topHomeStay;
        }
        public async Task<List<(object span, int totalBookings, double totalBookingsAmount)>> GetTotalBookingsTotalBookingsAmountForHomeStay
        (int homeStayID, DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException($"startDate <= endDate");
            }
            List<(object span, int totalBookings, double totalBookingsAmount)> result = new List<(object, int, double)>();

            var homestay = await _context.HomeStays
            .Include(h => h.CancelPolicy)
            .FirstOrDefaultAsync(h => h.HomeStayID == homeStayID);

            if (homestay == null)
            {
                throw new ArgumentException("Invalid HomeStayID");
            }
            switch (timeSpanType?.ToLower())
            {
                case "day":
                    for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {
                        DateTime currentDayStart = date.Date;
                        DateTime currentDayEnd = date.Date.AddDays(1).AddTicks(-1);
                        double totalBookingsAmount = 0;
                        int totalBookings = 0;

                        var bookingsDay = _context.Bookings
                            .Where(o => o.HomeStayID == homestay.HomeStayID)
                            .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= currentDayStart && d.CheckInDate <= currentDayEnd)
                                       || (d.CheckOutDate >= currentDayStart && d.CheckOutDate <= currentDayEnd)));
                        // Tính tổng bookings và tiền cho từng homestay trong ngày
                        totalBookings = await bookingsDay
                        .Where(o => o.Status == BookingStatus.Completed ||
                                    (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded) ||
                                    (o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded &&
                                    o.paymentStatus != PaymentStatus.Pending))
                        .CountAsync();

                        double completedBookingsAmount = await bookingsDay
                             .Where(o => o.Status == BookingStatus.Completed)
                             .SumAsync(o => o.Total);

                        // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                        double refundedBookingsAmount = await bookingsDay
                            .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                            .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                        double nonRefundedBookingsAmount = await bookingsDay
                            .Where(o => o.Status == BookingStatus.Cancelled
                                        && o.paymentStatus != PaymentStatus.Refunded
                                        && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                            .SumAsync(o => o.Total);
                        double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                        totalBookingsAmount = totalEachHomeStay;

                        result.Add((date.Date, totalBookings, totalBookingsAmount));
                    }
                    break;
                case "week":
                    DateTime currentWeekStart = startDate.Date.AddDays(-(int)startDate.DayOfWeek + (int)DayOfWeek.Monday);
                    if (currentWeekStart > startDate.Date)
                    {
                        currentWeekStart = startDate.Date.AddDays(-(int)startDate.DayOfWeek - 6);
                    }
                    while (currentWeekStart <= endDate.Date)
                    {
                        DateTime currentWeekEnd = currentWeekStart.AddDays(6);

                        if (currentWeekEnd > endDate.Date)
                        {
                            currentWeekEnd = endDate.Date.AddDays(-(int)endDate.DayOfWeek + 7);
                        }
                        double totalBookingsAmount = 0;
                        int totalBookings = 0;

                        var bookingWeek = _context.Bookings
                          .Where(o => o.HomeStayID == homestay.HomeStayID)
                          .Where(o => o.BookingDetails
                          .Any(d => (d.CheckInDate >= currentWeekStart && d.CheckInDate <= currentWeekEnd)
                                 || (d.CheckOutDate >= currentWeekStart && d.CheckOutDate <= currentWeekEnd)));

                        totalBookings = await bookingWeek
                       .Where(o => o.Status == BookingStatus.Completed ||
                                       (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded) ||
                                       (o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded &&
                                       o.paymentStatus != PaymentStatus.Pending))
                      .CountAsync();

                        double completedBookingsAmount = await bookingWeek
                            .Where(o => o.Status == BookingStatus.Completed)
                            .SumAsync(o => o.Total);

                        // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                        double refundedBookingsAmount = await bookingWeek
                            .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                            .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                        double nonRefundedBookingsAmount = await bookingWeek
                            .Where(o => o.Status == BookingStatus.Cancelled
                                        && o.paymentStatus != PaymentStatus.Refunded
                                        && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                            .SumAsync(o => o.Total);
                        double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                        totalBookingsAmount = totalEachHomeStay;

                        string weekRange = $"{currentWeekStart.ToString("MM/dd/yyyy")} - {currentWeekEnd.ToString("MM/dd/yyyy")}";
                        result.Add((weekRange, totalBookings, totalBookingsAmount));

                        // Move to the start of the next week
                        currentWeekStart = currentWeekEnd.AddDays(1);
                    }
                    break;
                case "month":
                    DateTime currentMonthStart = new DateTime(startDate.Year, startDate.Month, 1);

                    while (currentMonthStart <= endDate.Date)
                    {
                        DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

                        double totalBookingsAmount = 0;
                        int totalBookings = 0;

                        var bookingMonth = _context.Bookings
                                .Where(o => o.HomeStayID == homestay.HomeStayID)
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= currentMonthStart && d.CheckInDate <= currentMonthEnd)
                                      || (d.CheckOutDate >= currentMonthStart && d.CheckOutDate <= currentMonthEnd)));

                        totalBookings = await bookingMonth
                        .Where(o => o.Status == BookingStatus.Completed ||
                                     (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded) ||
                                     (o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded &&
                                     o.paymentStatus != PaymentStatus.Pending))
                        .CountAsync();

                        double completedBookingsAmount = await bookingMonth
                            .Where(o => o.Status == BookingStatus.Completed)
                            .SumAsync(o => o.Total);

                        // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                        double refundedBookingsAmount = await bookingMonth
                            .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                            .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                        double nonRefundedBookingsAmount = await bookingMonth
                            .Where(o => o.Status == BookingStatus.Cancelled
                                        && o.paymentStatus != PaymentStatus.Refunded
                                        && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                            .SumAsync(o => o.Total);
                        double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                        totalBookingsAmount = totalEachHomeStay;

                        string monthName = currentMonthStart.ToString("MM/yyyy");

                        result.Add((monthName, totalBookings, totalBookingsAmount));

                        // Move to the start of the next month
                        currentMonthStart = currentMonthStart.AddMonths(1);
                    }
                    break;
                default:
                    for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {
                        DateTime currentDayStart = date.Date;
                        DateTime currentDayEnd = date.Date.AddDays(1).AddTicks(-1);
                        double totalBookingsAmount = 0;
                        int totalBookings = 0;

                        var bookingsDay = _context.Bookings
                            .Where(o => o.HomeStayID == homestay.HomeStayID)
                            .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= currentDayStart && d.CheckInDate <= currentDayEnd)
                                       || (d.CheckOutDate >= currentDayStart && d.CheckOutDate <= currentDayEnd)));
                        // Tính tổng bookings và tiền cho từng homestay trong ngày
                        totalBookings = await bookingsDay
                        .Where(o => o.Status == BookingStatus.Completed ||
                                    (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded) ||
                                    (o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded &&
                                    o.paymentStatus != PaymentStatus.Pending))
                        .CountAsync();

                        double completedBookingsAmount = await bookingsDay
                             .Where(o => o.Status == BookingStatus.Completed)
                             .SumAsync(o => o.Total);

                        // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                        double refundedBookingsAmount = await bookingsDay
                            .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                            .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                        double nonRefundedBookingsAmount = await bookingsDay
                            .Where(o => o.Status == BookingStatus.Cancelled
                                        && o.paymentStatus != PaymentStatus.Refunded
                                        && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                            .SumAsync(o => o.Total);
                        double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                        totalBookingsAmount = totalEachHomeStay;

                        result.Add((date.Date, totalBookings, totalBookingsAmount));
                    }
                    break;
            }
            return result;
        }

        public async Task<List<(string accountID, string CustomerName, int BookingCount)>> GetTopLoyalCustomersAsync(int homeStayId, int top = 5)
        {
            var topCustomers = await _context.Bookings
            .Where(b => b.HomeStayID == homeStayId && b.Status == BookingStatus.Completed)
            .GroupBy(b => new { b.AccountID, b.Account.Name })
            .Select(g => new
            {
                AccountId = g.Key.AccountID,
                CustomerName = g.Key.Name,
                BookingCount = g.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(top)
            .ToListAsync();

            return topCustomers
                  .Select(x => (x.AccountId, x.CustomerName, x.BookingCount))
                  .ToList();
        }

        public async Task<List<(object span, int totalBookings, double totalBookingsAmount)>> GetTotalBookingsTotalBookingsAmountAsync
        (DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException($"startDate <= endDate");
            }
            List<(object span, int totalBookings, double totalBookingsAmount)> result = new List<(object, int, double)>();

            var homestays = await _context.HomeStays.ToListAsync();

            switch (timeSpanType?.ToLower())
            {
                case "day":
                    // Show results for each day in the specified range
                    for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {

                        DateTime currentDayStart = date.Date;
                        DateTime currentDayEnd = date.Date.AddDays(1).AddTicks(-1);
                        double totalBookingsAmount = 0;
                        int totalBookings = 0;


                        foreach (var homestay in homestays)
                        {
                            var bookingsDay = _context.Bookings
                            .Where(o => o.HomeStayID == homestay.HomeStayID)
                            .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= currentDayStart && d.CheckInDate <= currentDayEnd)
                                       || (d.CheckOutDate >= currentDayStart && d.CheckOutDate <= currentDayEnd)));
                            // Tính tổng bookings và tiền cho từng homestay trong ngày
                            totalBookings += await bookingsDay
                            .Where(o => o.Status == BookingStatus.Completed ||
                                        (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded) ||
                                        (o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded &&
                                        o.paymentStatus != PaymentStatus.Pending))
                            .CountAsync();

                            double completedBookingsAmount = await bookingsDay
                                 .Where(o => o.Status == BookingStatus.Completed)
                                 .SumAsync(o => o.Total);

                            // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                            double refundedBookingsAmount = await bookingsDay
                                .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                                .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                            double nonRefundedBookingsAmount = await bookingsDay
                                .Where(o => o.Status == BookingStatus.Cancelled
                                            && o.paymentStatus != PaymentStatus.Refunded
                                            && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                                .SumAsync(o => o.Total);
                            double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                            totalBookingsAmount += totalEachHomeStay;

                        }
                        result.Add((date.Date, totalBookings, totalBookingsAmount));
                    }
                    break;
                case "week":
                    // Show results for each week in the specified range
                    DateTime currentWeekStart = startDate.Date.AddDays(-(int)startDate.DayOfWeek + (int)DayOfWeek.Monday);
                    if (currentWeekStart > startDate.Date)
                    {
                        currentWeekStart = startDate.Date.AddDays(-(int)startDate.DayOfWeek - 6);
                    }
                    while (currentWeekStart <= endDate.Date)
                    {
                        DateTime currentWeekEnd = currentWeekStart.AddDays(6);

                        if (currentWeekEnd > endDate.Date)
                        {
                            currentWeekEnd = endDate.Date.AddDays(-(int)endDate.DayOfWeek + 7);
                        }
                        double totalBookingsAmount = 0;
                        int totalBookings = 0;
                        foreach (var homestay in homestays)
                        {
                            var bookingWeek = _context.Bookings
                          .Where(o => o.HomeStayID == homestay.HomeStayID)
                          .Where(o => o.BookingDetails
                          .Any(d => (d.CheckInDate >= currentWeekStart && d.CheckInDate <= currentWeekEnd)
                                 || (d.CheckOutDate >= currentWeekStart && d.CheckOutDate <= currentWeekEnd)));

                            totalBookings += await bookingWeek
                           .Where(o => o.Status == BookingStatus.Completed ||
                                           (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded) ||
                                           (o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded &&
                                           o.paymentStatus != PaymentStatus.Pending))
                          .CountAsync();

                            double completedBookingsAmount = await bookingWeek
                                .Where(o => o.Status == BookingStatus.Completed)
                                .SumAsync(o => o.Total);

                            // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                            double refundedBookingsAmount = await bookingWeek
                                .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                                .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                            double nonRefundedBookingsAmount = await bookingWeek
                                .Where(o => o.Status == BookingStatus.Cancelled
                                            && o.paymentStatus != PaymentStatus.Refunded
                                            && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                                .SumAsync(o => o.Total);
                            double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                            totalBookingsAmount += totalEachHomeStay;
                        }
                        // Format the week string as "MM/dd/yyyy - MM/dd/yyyy"
                        string weekRange = $"{currentWeekStart.ToString("MM/dd/yyyy")} - {currentWeekEnd.ToString("MM/dd/yyyy")}";
                        result.Add((weekRange, totalBookings, totalBookingsAmount));

                        // Move to the start of the next week
                        currentWeekStart = currentWeekEnd.AddDays(1);
                    }
                    break;
                case "month":
                    // Show results for each month in the specified range
                    DateTime currentMonthStart = new DateTime(startDate.Year, startDate.Month, 1);

                    while (currentMonthStart <= endDate.Date)
                    {
                        DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

                        double totalBookingsAmount = 0;
                        int totalBookings = 0;
                        foreach (var homestay in homestays)
                        {
                            var bookingMonth = _context.Bookings
                                .Where(o => o.HomeStayID == homestay.HomeStayID)
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= currentMonthStart && d.CheckInDate <= currentMonthEnd)
                                      || (d.CheckOutDate >= currentMonthStart && d.CheckOutDate <= currentMonthEnd)));

                            totalBookings += await bookingMonth
                            .Where(o => o.Status == BookingStatus.Completed ||
                                         (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded) ||
                                         (o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded &&
                                         o.paymentStatus != PaymentStatus.Pending))
                            .CountAsync();

                            double completedBookingsAmount = await bookingMonth
                                .Where(o => o.Status == BookingStatus.Completed)
                                .SumAsync(o => o.Total);

                            // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                            double refundedBookingsAmount = await bookingMonth
                                .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                                .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                            double nonRefundedBookingsAmount = await bookingMonth
                                .Where(o => o.Status == BookingStatus.Cancelled
                                            && o.paymentStatus != PaymentStatus.Refunded
                                            && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                                .SumAsync(o => o.Total);
                            double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                            totalBookingsAmount += totalEachHomeStay;
                        }

                        // Format the month string as "MM/yyyy"
                        string monthName = currentMonthStart.ToString("MM/yyyy");

                        result.Add((monthName, totalBookings, totalBookingsAmount));

                        // Move to the start of the next month
                        currentMonthStart = currentMonthStart.AddMonths(1);
                    }
                    break;
                default:
                    // Default to "ngày" if timeSpanType is unrecognized
                    for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {
                        DateTime currentDayStart = date.Date;
                        DateTime currentDayEnd = date.Date.AddDays(1).AddTicks(-1);

                        double totalBookingsAmount = 0;
                        int totalBookings = 0;

                        foreach (var homestay in homestays)
                        {
                            var bookingsDay = _context.Bookings
                            .Where(o => o.HomeStayID == homestay.HomeStayID)
                            .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= currentDayStart && d.CheckInDate <= currentDayEnd)
                                       || (d.CheckOutDate >= currentDayStart && d.CheckOutDate <= currentDayEnd)));

                            // Tính tổng bookings và tiền cho từng homestay trong ngày
                            totalBookings += await bookingsDay
                            .Where(o => o.Status == BookingStatus.Completed ||
                            (o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded))
                            .CountAsync();

                            double completedBookingsAmount = await bookingsDay
                                 .Where(o => o.Status == BookingStatus.Completed)
                                 .SumAsync(o => o.Total);

                            // Tính tổng tiền từ các booking bị hủy và có hoàn trả
                            double refundedBookingsAmount = await bookingsDay
                                .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus == PaymentStatus.Refunded)
                                .SumAsync(o => o.Total * (1 - (o.HomeStay.CancelPolicy.RefundPercentage / 100.0)));

                            double nonRefundedBookingsAmount = await bookingsDay
                                .Where(o => o.Status == BookingStatus.Cancelled
                                            && o.paymentStatus != PaymentStatus.Refunded
                                            && o.paymentStatus != PaymentStatus.Pending) // bỏ pending
                                .SumAsync(o => o.Total);
                            double totalEachHomeStay = completedBookingsAmount + refundedBookingsAmount + nonRefundedBookingsAmount;
                            totalBookingsAmount += totalEachHomeStay;
                        }
                        result.Add((date.Date, totalBookings, totalBookingsAmount));
                    }
                    break;
            }
            return result;
        }
        // DataAccessObject/BookingDAO.cs
        public async Task<IEnumerable<Booking>> GetBookingsForCheckInReminderAsync()
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(1); // 1 ngày tới
            var endDate = today.AddDays(2);  // 2 ngày tới

            return await _context.Bookings
                .Include(b => b.Account)
                .Include(b => b.HomeStay)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.HomeStayRentals)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Rooms)
                .ThenInclude(r => r.RoomTypes)
                .ThenInclude(rt => rt.HomeStayRentals)
                .Where(b => b.BookingDetails.Any(bd => bd.CheckInDate.Date >= startDate && bd.CheckInDate.Date <= endDate))
                .Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Completed) // Chỉ gửi email cho booking chưa bị hủy
                .ToListAsync();
        }
    }
}
