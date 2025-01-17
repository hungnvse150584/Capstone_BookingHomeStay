﻿using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingAsync(string? search, DateTime? date = null, BookingStatus? status = null);
        Task<IEnumerable<Booking>> GetBookingsByAccountId(string accountId);
        Task<Booking?> GetBookingStatusByAccountId(string accountId);
        Task<Booking?> ChangeBookingStatus(int bookingId, BookingStatus status);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<Booking?> UpdateBookingWithReportAsync(int bookingId, Booking booking);
    }
}
