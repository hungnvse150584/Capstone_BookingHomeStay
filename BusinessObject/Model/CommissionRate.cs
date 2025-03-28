﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;
    public class CommissionRate
    {
        [Key]
        public int CommissionRateID { get; set; }

        public double HostShare { get; set; }

        public double PlatformShare { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public ICollection<HomeStay> HomeStays { get; set; }
    }

