﻿using BusinessObject.Model;
using Service.RequestAndResponse.Response.CultureExperiences;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.Ratings;
using Service.RequestAndResponse.Response.Reports;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.HomeStays
{
    public class GetAllHomeStayWithOwnerName
    {
        public int HomeStayID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public HomeStayStatus Status { get; set; }
        public string Area { get; set; }
        public string AccountID { get; set; }
        public int? CommissionRateID { get; set; }
        public object CommissionRate { get; set; }
        public int? CancellationID { get; set; }
        public object CancelPolicy { get; set; }
        public int TypeOfRental { get; set; }
        public IEnumerable<GetReportResponse> Reports { get; set; }
        public IEnumerable<ImageHomeStayResponse> ImageHomeStays { get; set; }
        public IEnumerable<GetAllCultureExperiencesResponse> CultureExperiences { get; set; }
        public IEnumerable<GetServiceForHomeStay> Services { get; set; }
        public IEnumerable<GetAllRatingResponse> Ratings { get; set; }
        public string OwnerName { get; set; }

    }
}
