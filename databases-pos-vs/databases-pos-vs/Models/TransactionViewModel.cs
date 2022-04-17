using System;
using System.ComponentModel.DataAnnotations;

namespace databseApp.Models
{
    public class TransactionViewModel
    {
        [Key]
        public int Transaction_id { get; set; }
        public int Customer_id { get; set; }
        public string Payment_Method { get; set; }
        public string Order_Date { get; set; }
        public string Shipping_Address { get; set; }
        public string Product_Cost { get; set; }
        public string Shipping_Cost { get; set; }
        public string Total_Cost { get; set; }

        public string email { get; set; }
        public string name { get; set; }

        //Data for report
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartPrice { get; set; }
        public string EndPrice { get; set; }


    }
}
