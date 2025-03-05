using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Account : IdentityUser
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string? TaxCode { get; set; }

        public string BankAccountNumber { get; set; }
        
        public bool Status { get; set; }

        public DateTime DateOfBirth { get; set; }

        public ICollection<HomeStay> HomeStays { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<Report> Reports { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public ICollection<BookingServices> BookingServices { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<CultureExperience> CultureExperiences { get; set; }
    }

