using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class HomeStayRentals
    {
        [Key]
        public int HomeStayRentalID { get; set; }

        [Required]
        public string Name { get; set; } 

        [Required]
        public string? Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        [Required]
        public int numberBedRoom { get; set; }

        [Required]
        public int numberBathRoom { get; set; }

        [Required]
        public int numberKitchen { get; set; }

        [Required]
        public int numberWifi { get; set; }

        public bool Status { get; set; }

        public bool RentWhole { get; set; }

        public int MaxAdults { get; set; }
        
        public int MaxChildren { get; set; }
        
        public int MaxPeople { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID")]
        public HomeStay? HomeStay { get; set; }

        public ICollection<RoomTypes> RoomTypes { get; set; }

        public ICollection<ImageHomeStayRentals> ImageHomeStayRentals { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }

        public ICollection<Pricing> Prices { get; set; }

    }

