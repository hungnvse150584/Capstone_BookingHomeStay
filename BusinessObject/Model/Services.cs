﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Services
    {
        [Key]
        public int ServicesID { get; set; }

        [Required]
        public string servicesName { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public DateTime? DeleteAt { get; set; }

        public int? Quantity { get; set; }

        [Required]
        public double servicesPrice { get; set; }

        public bool Status { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID")]
        public HomeStay? HomeStay { get; set; }

        [EnumDataType(typeof(ServiceType))]
        public ServiceType ServiceType { get; set; }

        public ICollection<ImageServices> ImageServices { get; set; }

        public ICollection<BookingServicesDetail> BookingServicesDetails { get; set; }
    }
    public enum ServiceType
    {
        Quantity = 0,
        Hour = 1,
        Day = 2
    }


