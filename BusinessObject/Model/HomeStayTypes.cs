using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class HomeStayTypes
    {
        [Key]
        public int HomeStayTypesID { get; set; }

        [Required]
        public string Name { get; set; } 

        [Required]
        public string? Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        [Required]
        public double RentPrice { get; set; }

        public bool Status { get; set; }
       
        public int MaxAdults { get; set; }
        
        public int MaxChildren { get; set; }
        
        public int MaxPeople { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID ")]
        public HomeStay? HomeStay { get; set; }

        public int? PropertyID { get; set; }
        [ForeignKey("PropertyID ")]
        public Property? Property { get; set; }

        public ICollection<ImageHomeStayTypes> ImageHomeStayTypes { get; set; }

        public ICollection<Room> Rooms { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }

    }

