﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class Pricing
    {
        [Key]
        public int PricingID { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double RentPrice { get; set; }

        public double Percentage { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public int? HomeStayRentalID { get; set; }
        [ForeignKey("HomeStayRentalID")]
        public HomeStayRentals? HomeStayRentals { get; set; }

        public int? RoomTypesID { get; set; }
        [ForeignKey("RoomTypesID")]
        public RoomTypes RoomTypes { get; set; }

        [EnumDataType(typeof(DayType))]
        public DayType DayType { get; set; }

        public ICollection<HistoryPricing> PricingHistories { get; set; }
    }

    public enum DayType
    {
        Weekday,// =0
        Weekend,//=1
        Holiday //=2
    }
}
