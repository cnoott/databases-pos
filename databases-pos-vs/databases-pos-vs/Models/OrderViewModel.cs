
﻿using System;
using System.ComponentModel.DataAnnotations;

namespace databseApp.Models

{
    public class OrderViewModel
    {
        [Key]
        public int order_history_id { get; set; }
        public int inventory_id { get; set; }
        public string vendor_id { get; set; }
        public string quantity { get; set; }
        public string date_buy { get; set; }
        public string userVar { get; set; }

        //Data for report
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string minQuantity { get; set; }
        public string maxQuantity { get; set; }

    }
}
