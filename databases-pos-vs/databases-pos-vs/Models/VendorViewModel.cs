using System;
using System.ComponentModel.DataAnnotations;

namespace databaseApp.Models
{
    public class VendorViewModel
    {
        [Key]
        public int VendorID { get; set; }
        public int InventoryID { get; set; }
        
        public string Location { get; set; }
        public int SupervisorID { get; set; }


    }
}

