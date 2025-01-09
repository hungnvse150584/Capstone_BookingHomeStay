﻿using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        private readonly BookingDAO _bookingDao;
        public BookingRepository(BookingDAO bookingDao) : base(bookingDao)
        {
            _bookingDao = bookingDao;
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _bookingDao.AddBookingAsync(booking);
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _bookingDao.UpdateAsync(booking);
        }

        public async Task<Booking?> ChangeBookingStatus(int bookingId, BookingStatus status)
        {
            return await _bookingDao.ChangeBookingStatus(bookingId, status);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync(string? search, DateTime? date = null, BookingStatus? status = null)
        {
           return await _bookingDao.GetAllBookingAsync(search, date);
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingDao.GetBookingByIdAsync(bookingId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByAccountId(string accountId)
        {
            return await _bookingDao.GetBookingsByAccountId(accountId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _bookingDao.GetBookingsByDateAsync(date);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _bookingDao.GetBookingsByStatusAsync(status);
        }

        public async Task<Booking?> UpdateBookingWithReportAsync(int bookingId, Booking booking)
        {
           return await _bookingDao.UpdateBookingWithReportAsync(bookingId, booking);
        }
    }
}
