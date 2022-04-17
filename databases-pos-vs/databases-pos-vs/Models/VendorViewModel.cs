using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace databseApp.Models
{
    public class VendorViewModel
    {
        [Key]
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string Location { get; set; }
        public string Manager { get; set; }
        public int [] sales { get; set; }

    }
}

