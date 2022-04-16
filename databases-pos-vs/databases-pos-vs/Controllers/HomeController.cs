using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using databseApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Text;

namespace databases_pos_vs.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            this._configuration = configuration;

        }

        public IActionResult Index()
        {
            MySqlDataAdapter daProducts;
            DataTable dtbl = new DataTable();

            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string sql = "SELECT * FROM Products";
                daProducts = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daProducts);
                daProducts.Fill(dtbl);

            }
            return View(dtbl);
           
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AddToCart(int id, float Price)
        {
            string idString = id.ToString();

            if (!HttpContext.Request.Cookies.ContainsKey("CartCookie"))
                HttpContext.Response.Cookies.Append("CartCookie", idString);

            else
            {
                string newCart = HttpContext.Request.Cookies["CartCookie"] + "," + idString;
                HttpContext.Response.Cookies.Append("CartCookie", newCart);
            }

            if (HttpContext.Request.Cookies.ContainsKey("Sum"))
            {
                float newSum = float.Parse(HttpContext.Request.Cookies["Sum"]) + Price;
                HttpContext.Response.Cookies.Append("Sum", newSum.ToString());
            }
            else
            {
                HttpContext.Response.Cookies.Append("Sum", Price.ToString());
            }


            if (HttpContext.Request.Cookies.ContainsKey("Qty"))
            {
                float newSum = Int32.Parse(HttpContext.Request.Cookies["Qty"]) + 1;
                HttpContext.Response.Cookies.Append("Qty", newSum.ToString());
            }
            else
            {
                HttpContext.Response.Cookies.Append("Qty", "1");
            }




            return RedirectToAction(nameof(Index));

        }

        public IActionResult Cart()
        {
            
            if (HttpContext.Request.Cookies["CartCookie"] == null)
                return RedirectToAction(nameof(Index));

            MySqlDataAdapter daProducts;
            DataTable dtbl = new DataTable();

            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string cartString = HttpContext.Request.Cookies["CartCookie"];
                string sql = String.Format("SELECT * FROM Products WHERE Products.product_id IN ({0})", cartString);
                daProducts = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daProducts);
                daProducts.Fill(dtbl);
            
            }
            return View(dtbl);
               
        }


        public IActionResult Checkout()
        {
            UserViewModel userViewModel = FetchUserByID();  
            return View(userViewModel);
        }
        //checkout controller
        //1. SQL query for creating new Transaction_Info entry
        //2. For every product in checkout, create new transactions entry with Transaction_Info Id and corresponding Product_id


        //TODO:
        //Jason: checkout form
        //Chichen: grab last insert id into a variable
        //Liam: grab product ids and make Transaction_info query


        
        [HttpPost]
        public IActionResult Checkout([Bind("Payment_Method, Shipping_Address")] TransactionViewModel transactionViewModel)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();



                //

                string userId = HttpContext.Request.Cookies["id"];
                string productCost = HttpContext.Request.Cookies["Sum"];
                string totalCost = (Int32.Parse(productCost) + 12).ToString();

                string today = DateTime.Today.ToString();

                //string query = "INSERT INTO Transaction_Info(customer_id, payment_method, order_date, shipping_address, product_cost, shipping_cost, total_cost)";
                //string query = String.Format("INSERT INTO Transaction_Info(customer_id, payment_method, order_date, shipping_address, product_cost, shipping_cost, total_cost) " +
                //    "VALUES({0}, \"{1}\", \"{2}\", \"{3}\", \"{4}\", {5}, \"{6})\"", userId, transactionViewModel.Payment_Method, today, transactionViewModel.Shipping_Address,
                //    productCost, "12", totalCost );

                MySqlCommand sqlCmd1 = new MySqlCommand("CreateNewTransaction", sqlConnection);
                sqlCmd1.CommandType = CommandType.StoredProcedure;
                sqlCmd1.Parameters.AddWithValue("@Customer_id", Int32.Parse(userId));
                sqlCmd1.Parameters.AddWithValue("@Payment_method", transactionViewModel.Payment_Method);
                sqlCmd1.Parameters.AddWithValue("@Order_date", today);
                sqlCmd1.Parameters.AddWithValue("@Shipping_address", transactionViewModel.Shipping_Address);
                sqlCmd1.Parameters.AddWithValue("@Product_cost", totalCost);
                sqlCmd1.Parameters.AddWithValue("@Total_cost", Double.Parse(totalCost));


                sqlCmd1.ExecuteNonQuery();

                


                MySqlCommand sqlCmd = new MySqlCommand();

                sqlCmd.Connection = sqlConnection;
                sqlCmd.CommandText = "Select_most_recent_trxInfo";
                sqlCmd.CommandType = CommandType.StoredProcedure;

                sqlCmd.Parameters.AddWithValue("@input", 11);
                sqlCmd.Parameters["@input"].Direction = ParameterDirection.Input;

                sqlCmd.Parameters.Add("@info", MySqlDbType.Int32);
                sqlCmd.Parameters["@info"].Direction = ParameterDirection.Output;

                sqlCmd.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine("Tranx number: " + sqlCmd.Parameters["@info"].Value);
                Object obj = sqlCmd.Parameters["@info"].Value;
                int result = Convert.ToInt32(obj);
                //



                //string transactionInfoId = "9";

                string productIdsString = HttpContext.Request.Cookies["CartCookie"];

                string[] productIds = productIdsString.Split(",");
                //string qty = HttpContext.Request.Cookies["Qty"];
                string qty = "1";

                foreach (var id in productIds)
                {
                    //INSERT INTO Transactions(FK_transactioninfoID, productId, quantity) VALUES(transactionInfoId, Quantity);
                    string transQuery = String.Format("SET @var=22;INSERT INTO Transactions(transaction_info_id, product_id, quantity, inventory_id) VALUES({0}, {1}, \"{2}\",{3} )", result, id, qty, id);
                    MySqlCommand transCmd = new MySqlCommand(transQuery, sqlConnection);
                    MySqlDataReader rdrr = transCmd.ExecuteReader();
                   
                    rdrr.Close();
                }

                DataTable edtbl = new DataTable();
                MySqlDataAdapter daEmail;
                string emailQuery = "SELECT email FROM Users WHERE user_id = '"+userId+"'";
                daEmail = new MySqlDataAdapter(emailQuery, sqlConnection);
                daEmail.Fill(edtbl);

                int r = 0;
                string sql = "SELECT @var";
                MySqlCommand cmd = new MySqlCommand(sql, sqlConnection);
                object result1 = cmd.ExecuteScalar();
                if (result1 != null)
                {
                    r = Convert.ToInt32(result1);
                    System.Diagnostics.Debug.WriteLine("Number of countries in the world database is: " + r);
                }

                if(r == 33) {
                    string to = edtbl.Rows[0]["email"].ToString(); //To address    
                    string from = "mastershoe111@gmail.com"; //From address    
                    MailMessage message = new MailMessage(from, to);

                    string mailbody = "You are receiving this email because we have confirmed your order! \n Expect your package in 1000 years";
                    message.Subject = "JaLiChen Order";
                    message.Body = mailbody;
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
                    System.Net.NetworkCredential basicCredential1 = new
                    System.Net.NetworkCredential("mastershoe111@gmail.com", "shoemaster@1");
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = basicCredential1;
                    try
                    {
                        client.Send(message);
                    }

                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                

                sqlConnection.Close();

                HttpContext.Response.Cookies.Delete("CartCookie");
                HttpContext.Response.Cookies.Delete("Qty");
                HttpContext.Response.Cookies.Delete("Sum");

            }
            return RedirectToAction(nameof(Index));
        }



        public UserViewModel FetchUserByID()
        {
            UserViewModel userViewModel = new UserViewModel();
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                MySqlDataAdapter daProducts;
                DataTable dtbl = new DataTable();
                int userid = Int32.Parse(Request.Cookies["id"]);
                string userrole = Request.Cookies["role"];
                string sql;

                sqlConnection.Open();

                    sql = string.Format("SELECT * FROM Users, Customers WHERE Users.user_id = '"+userid+"' AND Customers.CustomerID = '"+userid+"'");
               
                daProducts = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daProducts);
                daProducts.Fill(dtbl);

                    userViewModel.UserID = userid;
                    userViewModel.Role = userrole; 
                    userViewModel.Password = dtbl.Rows[0]["password"].ToString();
                    userViewModel.FirstName_ = dtbl.Rows[0]["FirstName"].ToString(); 
                    userViewModel.LastName_ = dtbl.Rows[0]["LastName"].ToString(); 
                    userViewModel.Email = dtbl.Rows[0]["email"].ToString(); 
                    userViewModel.Address = dtbl.Rows[0]["Address"].ToString(); 
                    userViewModel.City = dtbl.Rows[0]["City"].ToString(); 
                    userViewModel.State = dtbl.Rows[0]["State"].ToString(); 
                    userViewModel.Zipcode = dtbl.Rows[0]["Zipcode"].ToString(); 

 
                return userViewModel;
            }
        }
    }
}
