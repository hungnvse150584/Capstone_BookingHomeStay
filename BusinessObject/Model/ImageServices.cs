    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace BusinessObject.Model;

        public class ImageServices
        {
            [Key]
            public int ImageServicesID { get; set; }

            [Required]
            public string? Image { get; set; }

            public int? ServicesID { get; set; }
            [ForeignKey("ServicesID ")]
            public Services? Services { get; set; }
        }

