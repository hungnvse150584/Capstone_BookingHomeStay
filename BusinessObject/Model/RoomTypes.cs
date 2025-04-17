using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class RoomTypes
    {
        [Key]
        public int RoomTypesID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public DateTime? DeleteAt { get; set; }

        [Required]
        public int numberBedRoom { get; set; }

        [Required]
        public int numberBathRoom { get; set; }

        [Required]
        public int numberWifi { get; set; }

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public int MaxPeople { get; set; }

        public bool Status { get; set; }

        public int? HomeStayRentalID { get; set; }
        [ForeignKey("HomeStayRentalID")]
        public HomeStayRentals? HomeStayRentals { get; set; }

        public ICollection<ImageRoomTypes> ImageRoomTypes { get; set; }

        public ICollection<Room> Rooms { get; set; }

        public ICollection<Pricing> Prices { get; set; }

        public ICollection<HistoryPricing> PricingHistories { get; set; }
    }

