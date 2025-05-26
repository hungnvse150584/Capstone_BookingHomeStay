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

        public async Task<IEnumerable<Booking>> GetExpiredBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.BookingServices)
                .Where(b => (b.ExpiredTime < DateTime.Now && b.Status == BookingStatus.Pending) ||
                       (b.BookingDetails.Any(d => d.CheckOutDate <= DateTime.Now) && b.Status == BookingStatus.Confirmed))
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetCheckOutBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.BookingServices)
                .Include(b => b.Notifications)
                .Include(b => b.HomeStay)
                .Include(b => b.BookingDetails)
                .Where(b => b.BookingDetails.Any(bd => bd.CheckOutDate <= DateTime.Now) && b.Status == BookingStatus.InProgress)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByHomeStayId(int homeStayID)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.HomeStayRentals)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Rooms)
                .ThenInclude(bd => bd.RoomTypes)
                .ThenInclude(bd => bd.Prices)
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
                .ThenInclude(bd => bd.RoomTypes)
                .ThenInclude(bd => bd.Prices)
                .Include(o => o.BookingServices)
                .ThenInclude(o => o.BookingServicesDetails)
                .ThenInclude(o => o.Services)
                .Include(o => o.HomeStay)
                .Include(o => o.Transactions)
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
                .ThenInclude(bd => bd.RoomTypes)
                .ThenInclude(bd => bd.Prices)
                .Include(o => o.BookingServices)
                .ThenInclude(o => o.BookingServicesDetails)
                .ThenInclude(o => o.Services)
                .Include(o => o.HomeStay)
                .Include(o => o.Transactions)
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
        public async Task<(int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport, int bookingConfirmed)> GetStaticBookings()
        {
            /*// Lấy ngày hiện tại
            DateTime today = DateTime.Today;

            // Xác định ngày đầu tuần (ngày thứ Hai là ngày đầu tuần)
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            // Xác định ngày cuối tuần (ngày Chủ nhật là ngày cuối tuần)
            DateTime endOfWeek = startOfWeek.AddDays(6);*/

            int bookingsReturnOrCancell = await _context.Bookings
                 .Where(o => o.BookingDetails
                     .Any(d => d.CheckInDate != null
                            || d.CheckOutDate != null))
                 .Where(o => o.Status == BookingStatus.Cancelled
                          || o.paymentStatus == PaymentStatus.Refunded)
                 .CountAsync();

            int bookings = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => d.CheckInDate != null
                                       || d.CheckOutDate != null))
                                .CountAsync();

            int bookingsComplete = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => d.CheckInDate != null
                                       || d.CheckOutDate != null))
                                .Where(o => o.Status == BookingStatus.Completed)
                                .CountAsync();

            int bookingsCancell = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => d.CheckInDate != null
                                       || d.CheckOutDate != null))
                                .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded)
                                .CountAsync();

            int bookingsReturnRefund = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => d.CheckInDate != null
                                       || d.CheckOutDate != null))
                                .Where(o => o.paymentStatus == PaymentStatus.Refunded)
                                .CountAsync();

            int bookingsReport = await _context.Bookings
                               .Where(o => o.BookingDetails
                                .Any(d => d.CheckInDate != null
                                       || d.CheckOutDate != null))
                                .Where(o => o.ReportID != null)
                                .CountAsync();

            int bookingConfirmed = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => d.CheckInDate != null
                                       || d.CheckOutDate != null))
                                .Where(o => o.Status == BookingStatus.Confirmed && (o.paymentStatus == PaymentStatus.Deposited || o.paymentStatus == PaymentStatus.FullyPaid))
                                .CountAsync();
            return (bookingsReturnOrCancell, bookings, bookingsComplete, bookingsCancell, bookingsReturnRefund, bookingsReport, bookingConfirmed);
        }

        public async Task<(int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport, int bookingConfirmed)> GetStaticBookingsForHomeStay(int homestayId)
        {
            /* DateTime today = DateTime.Today;
             DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
             DateTime endOfWeek = startOfWeek.AddDays(6);*/

            var baseQuery = _context.Bookings
                .Where(o => o.HomeStayID == homestayId)
                .Where(o => o.BookingDetails.Any(d => d.CheckInDate != null
                            || d.CheckOutDate != null)
                );

            int bookingsReturnOrCancell = await baseQuery
                .Where(o => o.Status == BookingStatus.Cancelled || o.paymentStatus == PaymentStatus.Refunded)
                .CountAsync();

            int bookings = await baseQuery.CountAsync();

            int bookingsComplete = await baseQuery
                .Where(o => o.Status == BookingStatus.Completed)
                .CountAsync();

            int bookingsCancell = await baseQuery
                .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded)
                .CountAsync();

            int bookingsReturnRefund = await baseQuery
                .Where(o => o.paymentStatus == PaymentStatus.Refunded)
                .CountAsync();

            int bookingsReport = await baseQuery
                .Where(o => o.ReportID != null)
                .CountAsync();

            int bookingConfirmed = await baseQuery
                 .Where(o => o.Status == BookingStatus.Confirmed &&
                 (o.paymentStatus == PaymentStatus.Deposited ||
                  o.paymentStatus == PaymentStatus.FullyPaid))
                 .CountAsync();

            return (bookingsReturnOrCancell, bookings, bookingsComplete, bookingsCancell, bookingsReturnRefund, bookingsReport, bookingConfirmed);
        }

        public async Task<List<(HomeStay homeStay, int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport, int bookingConfirmed)>> GetStaticBookingsForAllHomestays()
        {
            /* DateTime today = DateTime.Today;
             DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
             DateTime endOfWeek = startOfWeek.AddDays(6);*/

            var homestays = await _context.HomeStays.ToListAsync();

            var results = new List<(HomeStay homeStay, int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport, int bookingConfirmed)>();

            foreach (var homestay in homestays)
            {
                var baseQuery = _context.Bookings
                .Where(o => o.HomeStayID == homestay.HomeStayID)
                .Where(o => o.BookingDetails.Any(d => d.CheckInDate != null || d.CheckOutDate != null));

                int bookingsReturnOrCancell = await baseQuery
                    .Where(o => o.Status == BookingStatus.Cancelled || o.paymentStatus == PaymentStatus.Refunded)
                    .CountAsync();

                int bookings = await baseQuery.CountAsync();

                int bookingsComplete = await baseQuery
                    .Where(o => o.Status == BookingStatus.Completed)
                    .CountAsync();

                int bookingsCancell = await baseQuery
                    .Where(o => o.Status == BookingStatus.Cancelled && o.paymentStatus != PaymentStatus.Refunded)
                    .CountAsync();

                int bookingsReturnRefund = await baseQuery
                    .Where(o => o.paymentStatus == PaymentStatus.Refunded)
                    .CountAsync();

                int bookingsReport = await baseQuery
                    .Where(o => o.ReportID != null)
                    .CountAsync();

                int bookingConfirmed = await baseQuery
                 .Where(o => o.Status == BookingStatus.Confirmed &&
                 (o.paymentStatus == PaymentStatus.Deposited ||
                  o.paymentStatus == PaymentStatus.FullyPaid))
                 .CountAsync();

                results.Add((homestay, bookingsReturnOrCancell, bookings, bookingsComplete, bookingsCancell, bookingsReturnRefund, bookingsReport, bookingConfirmed));
            }

            return results;
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

            var commissionRate = await _context.CommissionRates
            .FirstOrDefaultAsync(c => c.HomeStayID == homeStayID);

            if (commissionRate == null)
            {
                throw new Exception("Commission rate not found for the homestay.");
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

                        var transactionsDayCompleted = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                        var transactionsDayRefunded = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                        var transactionsDayCancelled = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                        // totalBookings là tổng count của Completed + Refunded
                        totalBookings = transactionsDayCompleted.Count + transactionsDayRefunded.Count;

                        // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                        totalBookingsAmount = transactionsDayCompleted.Sum(t => t.OwnerAmount)
                           + transactionsDayCancelled.Sum(t => t.OwnerAmount)
                           + transactionsDayRefunded.Sum(t => t.OwnerAmount);

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

                        var transactionsWeekCompleted = await _context.Transactions
                             .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                             .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                             .Where(t => t.PayDate >= currentWeekStart && t.PayDate <= currentWeekEnd)
                             .ToListAsync();

                        var transactionsWeekRefunded = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                            .Where(t => t.PayDate >= currentWeekStart && t.PayDate <= currentWeekEnd)
                            .ToListAsync();

                        var transactionsWeekCancelled = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                            .Where(t => t.PayDate >= currentWeekStart && t.PayDate <= currentWeekEnd)
                            .ToListAsync();

                        // totalBookings là tổng count của Completed + Refunded
                        totalBookings = transactionsWeekCompleted.Count + transactionsWeekRefunded.Count;

                        // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                        totalBookingsAmount = transactionsWeekCompleted.Sum(t => t.OwnerAmount)
                           + transactionsWeekCancelled.Sum(t => t.OwnerAmount)
                           + transactionsWeekRefunded.Sum(t => t.OwnerAmount);

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

                        var transactionsMonthCompleted = await _context.Transactions
                             .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                             .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                             .Where(t => t.PayDate >= currentMonthStart && t.PayDate <= currentMonthEnd)
                             .ToListAsync();

                        var transactionsMonthRefunded = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                            .Where(t => t.PayDate >= currentMonthStart && t.PayDate <= currentMonthEnd)
                            .ToListAsync();

                        var transactionsMonthCancelled = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                            .Where(t => t.PayDate >= currentMonthStart && t.PayDate <= currentMonthEnd)
                            .ToListAsync();

                        // totalBookings là tổng count của Completed + Refunded
                        totalBookings = transactionsMonthCompleted.Count + transactionsMonthRefunded.Count;

                        // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                        totalBookingsAmount = transactionsMonthCompleted.Sum(t => t.OwnerAmount)
                           + transactionsMonthCancelled.Sum(t => t.OwnerAmount)
                           + transactionsMonthRefunded.Sum(t => t.OwnerAmount);

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

                        var transactionsDayCompleted = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                        var transactionsDayRefunded = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                        var transactionsDayCancelled = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                        // totalBookings là tổng count của Completed + Refunded
                        totalBookings = transactionsDayCompleted.Count + transactionsDayRefunded.Count;

                        // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                        totalBookingsAmount = transactionsDayCompleted.Sum(t => t.OwnerAmount)
                           + transactionsDayCancelled.Sum(t => t.OwnerAmount)
                           + transactionsDayRefunded.Sum(t => t.OwnerAmount);

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

        public async Task<List<(object span, int totalBookings, double totalBookingsAmount)>> GetTotalBookingsTotalBookingsAmountAsync(
        DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException($"startDate <= endDate");
            }
            List<(object span, int totalBookings, double totalBookingsAmount)> result = new List<(object, int, double)>();

            var homestays = await _context.HomeStays
                .Include(h => h.CommissionRate)
                .Include(h => h.CancelPolicy)
                .Where(h => h.Status == HomeStayStatus.Accepted)
                .ToListAsync();

            switch (timeSpanType?.ToLower())
            {
                case "day":
                    for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {
                        DateTime currentDayStart = date.Date;
                        DateTime currentDayEnd = date.Date.AddDays(1).AddTicks(-1);
                        double totalBookingsAmount = 0;
                        int totalBookings = 0;

                        foreach (var homestay in homestays)
                        {
                            var transactionsDayCompleted = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                            var transactionsDayRefunded = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                                .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                                .ToListAsync();

                            var transactionsDayCancelled = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                                .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                                .ToListAsync();

                            // totalBookings là tổng count của Completed + Refunded
                            totalBookings += transactionsDayCompleted.Count + transactionsDayRefunded.Count;

                            // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                            totalBookingsAmount += transactionsDayCompleted.Sum(t => t.AdminAmount)
                               + transactionsDayCancelled.Sum(t => t.AdminAmount)
                               + transactionsDayRefunded.Sum(t => t.AdminAmount);
                        }

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

                        foreach (var homestay in homestays)
                        {
                            var commissionRate = homestay.CommissionRate;
                            var transactionsWeekCompleted = await _context.Transactions
                             .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                             .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                             .Where(t => t.PayDate >= currentWeekStart && t.PayDate <= currentWeekEnd)
                             .ToListAsync();

                            var transactionsWeekRefunded = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                                .Where(t => t.PayDate >= currentWeekStart && t.PayDate <= currentWeekEnd)
                                .ToListAsync();

                            var transactionsWeekCancelled = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                                .Where(t => t.PayDate >= currentWeekStart && t.PayDate <= currentWeekEnd)
                                .ToListAsync();

                            // totalBookings là tổng count của Completed + Refunded
                            totalBookings += transactionsWeekCompleted.Count + transactionsWeekRefunded.Count;

                            // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                            totalBookingsAmount += transactionsWeekCompleted.Sum(t => t.AdminAmount)
                               + transactionsWeekCancelled.Sum(t => t.AdminAmount)
                               + transactionsWeekRefunded.Sum(t => t.AdminAmount);
                        }

                        string weekRange = $"{currentWeekStart:MM/dd/yyyy} - {currentWeekEnd:MM/dd/yyyy}";
                        result.Add((weekRange, totalBookings, totalBookingsAmount));

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

                        foreach (var homestay in homestays)
                        {
                            var commissionRate = homestay.CommissionRate;
                            var transactionsMonthCompleted = await _context.Transactions
                             .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                             .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                             .Where(t => t.PayDate >= currentMonthStart && t.PayDate <= currentMonthEnd)
                             .ToListAsync();

                            var transactionsMonthRefunded = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                                .Where(t => t.PayDate >= currentMonthStart && t.PayDate <= currentMonthEnd)
                                .ToListAsync();

                            var transactionsMonthCancelled = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                                .Where(t => t.PayDate >= currentMonthStart && t.PayDate <= currentMonthEnd)
                                .ToListAsync();

                            // totalBookings là tổng count của Completed + Refunded
                            totalBookings += transactionsMonthCompleted.Count + transactionsMonthRefunded.Count;

                            // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                            totalBookingsAmount += transactionsMonthCompleted.Sum(t => t.AdminAmount)
                               + transactionsMonthCancelled.Sum(t => t.AdminAmount)
                               + transactionsMonthRefunded.Sum(t => t.AdminAmount);
                        }

                        string monthName = currentMonthStart.ToString("MM/yyyy");
                        result.Add((monthName, totalBookings, totalBookingsAmount));

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

                        foreach (var homestay in homestays)
                        {
                            var commissionRate = homestay.CommissionRate;
                            var transactionsDayCompleted = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                            .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                            .ToListAsync();

                            var transactionsDayRefunded = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                                .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                                .ToListAsync();

                            var transactionsDayCancelled = await _context.Transactions
                                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                                .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                                .Where(t => t.PayDate >= currentDayStart && t.PayDate <= currentDayEnd)
                                .ToListAsync();

                            // totalBookings là tổng count của Completed + Refunded
                            totalBookings += transactionsDayCompleted.Count + transactionsDayRefunded.Count;

                            // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                            totalBookingsAmount += transactionsDayCompleted.Sum(t => t.AdminAmount)
                               + transactionsDayCancelled.Sum(t => t.AdminAmount)
                               + transactionsDayRefunded.Sum(t => t.AdminAmount);
                        }

                        result.Add((date.Date, totalBookings, totalBookingsAmount));
                    }
                    break;
            }

            return result;
        }


        public async Task<(int totalBookings, double totalBookingsAmount)> GetTotalBookingsAndAmountForHomeStay(int homeStayID)
        {
            var homestay = await _context.HomeStays
                .Include(h => h.CancelPolicy)
                .FirstOrDefaultAsync(h => h.HomeStayID == homeStayID);

            if (homestay == null)
            {
                throw new ArgumentException("Invalid HomeStayID");
            }

            var commissionRate = await _context.CommissionRates
                .FirstOrDefaultAsync(c => c.HomeStayID == homeStayID);

            if (commissionRate == null)
            {
                throw new Exception("Commission rate not found for the homestay.");
            }
            int totalBookings = 0;
            double totalBookingsAmount = 0;

            var transactionsDayCompleted = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                            .ToListAsync();

            var transactionsDayRefunded = await _context.Transactions
                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                .ToListAsync();

            var transactionsDayCancelled = await _context.Transactions
                .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                .ToListAsync();

            // totalBookings là tổng count của Completed + Refunded
            totalBookings = transactionsDayCompleted.Count + transactionsDayRefunded.Count;

            // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
            totalBookingsAmount = transactionsDayCompleted.Sum(t => t.AdminAmount)
               + transactionsDayCancelled.Sum(t => t.AdminAmount)
               + transactionsDayRefunded.Sum(t => t.AdminAmount);

            return (totalBookings, totalBookingsAmount);
        }

        public async Task<(int totalBookings, double totalBookingsAmount)> GetTotalBookingsAndAmount()
        {
            var homestays = await _context.HomeStays
               .Include(h => h.CommissionRate)
               .Include(h => h.CancelPolicy)
               .Where(h => h.Status == HomeStayStatus.Accepted)
               .ToListAsync();

            double totalBookingsAmount = 0;
            int totalBookings = 0;

            foreach (var homestay in homestays)
            {
                var transactionsDayCompleted = await _context.Transactions
                            .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                            .Where(t => t.StatusTransaction == StatusOfTransaction.Completed || t.StatusTransaction == StatusOfTransaction.Pending)
                            .ToListAsync();

                var transactionsDayRefunded = await _context.Transactions
                    .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                    .Where(t => t.StatusTransaction == StatusOfTransaction.Refunded)
                    .ToListAsync();

                var transactionsDayCancelled = await _context.Transactions
                    .Where(t => t.HomeStay.HomeStayID == homestay.HomeStayID)
                    .Where(t => t.StatusTransaction == StatusOfTransaction.Cancelled)
                    .ToListAsync();

                // totalBookings là tổng count của Completed + Refunded
                totalBookings += transactionsDayCompleted.Count + transactionsDayRefunded.Count;

                // totalBookingsAmount là tổng OwnerAmount của Completed + Cancelled + Refunded
                totalBookingsAmount += transactionsDayCompleted.Sum(t => t.AdminAmount)
                   + transactionsDayCancelled.Sum(t => t.AdminAmount)
                   + transactionsDayRefunded.Sum(t => t.AdminAmount);
            }
            return (totalBookings, totalBookingsAmount);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(int roomId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                throw new ArgumentException("startDate must be less than or equal to endDate");
            }

            // Chuẩn hóa múi giờ
            // Giả sử dữ liệu trong DB là UTC+7 (giờ Việt Nam), đảm bảo endDate cũng ở UTC+7
            DateTime? adjustedStartDate = startDate.HasValue
                ? startDate.Value.Kind == DateTimeKind.Utc
                    ? startDate.Value.AddHours(7)
                    : startDate.Value
                : null;
            DateTime? adjustedEndDate = endDate.HasValue
                ? DateTime.SpecifyKind(endDate.Value.AddHours(7), DateTimeKind.Unspecified) // Luôn chuyển về UTC+7
                : null;

            // Log để kiểm tra
            Console.WriteLine($"Adjusted startDate: {adjustedStartDate}, Adjusted endDate: {adjustedEndDate}");

            var filteredDetailsQuery = _context.BookingDetails
                .Where(bd => bd.RoomID == roomId &&
                             (!adjustedStartDate.HasValue || bd.CheckInDate >= adjustedStartDate.Value) &&
                             (!adjustedEndDate.HasValue || bd.CheckOutDate <= adjustedEndDate.Value));

            var filteredDetails = await filteredDetailsQuery
                .Select(bd => new { bd.BookingID, bd.BookingDetailID, bd.CheckInDate, bd.CheckOutDate })
                .ToListAsync();
            Console.WriteLine($"Filtered BookingDetails for roomId={roomId}, startDate={startDate}, endDate={endDate}:");
            foreach (var detail in filteredDetails)
            {
                Console.WriteLine($"  BookingID: {detail.BookingID}, DetailID: {detail.BookingDetailID}, CheckIn: {detail.CheckInDate}, CheckOut: {detail.CheckOutDate}, CheckOut Kind: {detail.CheckOutDate.Kind}");
            }

            var filteredBookingIds = filteredDetails.Select(d => d.BookingID).Distinct().ToList();

            var query = _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Rooms)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.HomeStayRentals)
                .Include(b => b.Account)
                .Where(b => b.Status != BookingStatus.Pending && filteredBookingIds.Contains(b.BookingID));

            var bookings = await query.ToListAsync();

            Console.WriteLine($"Total bookings found for roomId={roomId}, startDate={startDate}, endDate={endDate}: {bookings.Count}");
            foreach (var booking in bookings)
            {
                Console.WriteLine($"BookingID: {booking.BookingID}");
                foreach (var detail in booking.BookingDetails.Where(bd => bd.RoomID == roomId))
                {
                    Console.WriteLine($"  DetailID: {detail.BookingDetailID}, RoomID: {detail.RoomID}, CheckIn: {detail.CheckInDate}, CheckOut: {detail.CheckOutDate}");
                }
            }

            return bookings;
        }
        public async Task<List<(Account Account, int TotalBooking)>> GetCustomersByHomeStayAsync(int homeStayId)
        {
            var result = await _context.Bookings
            .Where(b => b.HomeStayID == homeStayId)
            .GroupBy(b => b.AccountID)
            .Select(g => new
            {
                AccountId = g.Key,
                TotalBooking = g.Count(),
            })
            .Join(_context.Accounts,
                  booking => booking.AccountId,
                  account => account.Id,
                  (booking, account) => new { Account = account, booking.TotalBooking })
            .ToListAsync();

            // Lấy thông tin `CheckIn` mới nhất từ `BookingRoomDetails`
            var latestCheckInDates = await _context.BookingDetails
                .Where(bd => bd.Booking.HomeStayID == homeStayId)
                .GroupBy(bd => bd.Booking.AccountID)
                .Select(g => new
                {
                    AccountId = g.Key,
                    LatestCheckInDate = g.Max(bd => bd.CheckInDate)
                })
                .ToListAsync();

            // Gộp dữ liệu từ kết quả trên
            var mergedResult = result.Join(latestCheckInDates,
                                           r => r.Account.Id,
                                           l => l.AccountId,
                                           (r, l) => new
                                           {
                                               r.Account,
                                               r.TotalBooking,
                                               l.LatestCheckInDate
                                           })
                                           .Where(x => x.LatestCheckInDate != null) // Chỉ lấy khách hàng có ngày CheckIn
                                           .ToList();

            return mergedResult.Select(x => (x.Account, x.TotalBooking)).ToList();
        }

        public async Task<List<(string date, double totalBookingsAmount)>> GetCurrentWeekRevenueForHomeStayAsync(int homestayId)
        {
            var today = DateTime.Today;

            // Tính ngày đầu tuần (Thứ 2) và cuối tuần (Chủ nhật)
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6);

            var homestay = await _context.HomeStays
                .Where(h => h.HomeStayID == homestayId)
                .Include(h => h.CommissionRate)
                .FirstOrDefaultAsync();

            if (homestay == null || homestay.CommissionRate == null)
                return new List<(string, double)>();

            var hostShare = homestay.CommissionRate.HostShare;

            var transactions = await _context.Transactions
                .Where(t => t.HomeStay.HomeStayID == homestayId)
                .Where(t => (t.TransactionKind == TransactionKind.Deposited || t.TransactionKind == TransactionKind.FullPayment || t.TransactionKind == TransactionKind.Refund))
                .Where(t => t.PayDate >= startOfWeek && t.PayDate <= endOfWeek)
                .ToListAsync();

            // Gom nhóm theo ngày và tính doanh thu theo từng trạng thái
            var revenueByDay = transactions
                        .GroupBy(b => b.PayDate.Date)
                        .Select(g => (
                            date: g.Key.ToString("yyyy-MM-dd"),
                             totalBookingsAmount:
                        g.Where(t => t.TransactionKind == TransactionKind.Deposited || t.TransactionKind == TransactionKind.FullPayment).Sum(t => t.Amount)
                        - g.Where(t => t.TransactionKind == TransactionKind.Refund).Sum(t => t.Amount)
                        - g.Where(t => t.TransactionKind == TransactionKind.Deposited || t.TransactionKind == TransactionKind.FullPayment).Sum(t => t.AdminAmount)
                ))
                .ToList();

            // Đảm bảo có đủ 7 ngày trong tuần, kể cả khi không có booking
            var fullWeekRevenue = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = startOfWeek.AddDays(offset).ToString("yyyy-MM-dd");
                    var matched = revenueByDay.FirstOrDefault(d => d.date == date);
                    return matched != default ? matched : (date, 0.0);
                })
                .ToList();

            return fullWeekRevenue;
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
        public async Task<Booking?> GetBookingByAccountAndHomeStayAsync(string accountId, int homeStayId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .Where(b => b.AccountID == accountId && b.HomeStayID == homeStayId)
                .Where(b => b.Status == BookingStatus.Completed) // Booking phải ở trạng thái Completed
                .Where(b => b.RatingID == null) // Booking chưa có Rating
                .OrderByDescending(b => b.BookingDate) // Lấy booking gần nhất
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsBookingCodeAsync(string code)
        {
            return await _context.Bookings.AnyAsync(b => b.BookingCode == code);
        }
    }
}
