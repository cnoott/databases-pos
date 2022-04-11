using System;
using System.ComponentModel.DataAnnotations;


namespace databseApp.Models
{
    public class UserViewModel
    {
        [Key]
        public int UserID { get; set; }
        public string Password { get; set; }
        public string FirstName_ { get; set; }
        public string LastName_ { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Date { get; set; }
        public string Payment_Method { get; set; }
        public string Shipping_Address { get; set; }
    }
}
