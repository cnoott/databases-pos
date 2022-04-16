using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace databseApp.Models
{
    public class ProductViewModel
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Size { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Should be greated than or equal to 1")]
        public float Price { get; set; }
        [Required]
        public string Name { get; set; }

        public int Category_id { get; set; }
        [Required]
        public int Vendor_id { get; set; }
        public int times_sold { get; set; }
        public string Image_url { get; set; }
        public int vendorsize { get; set; }
        [Range(6, int.MaxValue, ErrorMessage = "Should be greated than or equal to 5")]
        public int Quantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Should be greated than or equal to 1")]
        public float VendorPrice { get; set; }

        //for create
        public string[] VendorIds { get; set; }
        public string[] VendorNames { get; set; }

    }
}