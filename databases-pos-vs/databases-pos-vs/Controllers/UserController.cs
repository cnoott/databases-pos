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
using System.Data;
using MySql.Data.MySqlClient;
using System.Web;

namespace databseApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult Logout()
        {
            
            HttpContext.Response.Cookies.Delete("role");
            HttpContext.Response.Cookies.Delete("email");
            HttpContext.Response.Cookies.Delete("name");
            HttpContext.Response.Cookies.Delete("id");

            if (HttpContext.Request.Cookies["CartCookie"] != null) { 
                HttpContext.Response.Cookies.Delete("CartCookie");
                HttpContext.Response.Cookies.Delete("Qty");
                HttpContext.Response.Cookies.Delete("Sum");
            }

            return RedirectToAction("Index", new { Controller = "Home", Action = "Index" });
        }

        // GET: User Login
        public IActionResult UserIndex()
        {
                UserViewModel userViewModel = new UserViewModel();
                return View(userViewModel); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserIndex([Bind("FirstName_, LastName_, Email, Password, Role, UserID")] UserViewModel userViewModel)
        {
            DataTable dtbl = new DataTable();
            DataTable dtbl2 = new DataTable();
            MySqlDataAdapter daUsers;
            MySqlDataAdapter daName;


            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string sql = "SELECT * FROM Users WHERE email = '"+userViewModel.Email+"' AND password = '"+userViewModel.Password+"'";
                daUsers = new MySqlDataAdapter(sql, sqlConnection);
                daUsers.Fill(dtbl);

                if (dtbl.Rows.Count == 1)
                {
                    userViewModel.UserID = Convert.ToInt32(dtbl.Rows[0]["user_id"].ToString());
                    userViewModel.Role = dtbl.Rows[0]["role"].ToString(); 
                    if (userViewModel.Role == "customer")
                    {
                        string temp = "SELECT FirstName FROM Customers WHERE CustomerID = '"+userViewModel.UserID+"'";
                        daName = new MySqlDataAdapter(temp, sqlConnection);
                        daName.Fill(dtbl2);
                        userViewModel.FirstName_ = dtbl2.Rows[0]["FirstName"].ToString();
                    } 
                    else
                    {
                        string temp = "SELECT FirstName FROM Employees WHERE employee_id = '"+userViewModel.UserID+"'";
                        daName = new MySqlDataAdapter(temp, sqlConnection);
                        daName.Fill(dtbl2);
                        userViewModel.FirstName_ = dtbl2.Rows[0]["FirstName"].ToString();
                    }
                    sqlConnection.Close();
                    //save account in localstorage as cookie
                    HttpContext.Response.Cookies.Append("id", (userViewModel.UserID).ToString());
                    HttpContext.Response.Cookies.Append("email", userViewModel.Email);
                    HttpContext.Response.Cookies.Append("role", userViewModel.Role);
                    HttpContext.Response.Cookies.Append("name", userViewModel.FirstName_);

                    return RedirectToAction("Index", new { Controller = "Home", Action = "Index" });
                }
                sqlConnection.Close();
            }
            return View(userViewModel);
        }


        // GET:
        public IActionResult Create()
        {
            UserViewModel userViewModel = new UserViewModel();
            return View(userViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstName_, LastName_, Email, Password, Role, UserID, Address, Zipcode, City, State")] UserViewModel userViewModel)
        {
            MySqlDataAdapter daProducts;
            DataTable dtbl = new DataTable();
            if (ModelState.IsValid)
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    MySqlCommand sqlCmd = new MySqlCommand("CreateNewUser", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@FirstName_", userViewModel.FirstName_);
                    sqlCmd.Parameters.AddWithValue("@LastName_", userViewModel.LastName_);
                    sqlCmd.Parameters.AddWithValue("@Email", userViewModel.Email);
                    sqlCmd.Parameters.AddWithValue("@Password", userViewModel.Password);
                    sqlCmd.Parameters.AddWithValue("@Role", userViewModel.Role);
                    sqlCmd.Parameters.AddWithValue("@Address_", userViewModel.Address);
                    sqlCmd.Parameters.AddWithValue("@Zipcode_", userViewModel.Zipcode);
                    sqlCmd.Parameters.AddWithValue("@City_", userViewModel.City);
                    sqlCmd.Parameters.AddWithValue("@State_", userViewModel.State);
                    sqlCmd.ExecuteNonQuery();

                    string sql = "SELECT user_id FROM Users WHERE email = '"+userViewModel.Email+"' AND password = '"+userViewModel.Password+"'";                    
                    daProducts = new MySqlDataAdapter(sql, sqlConnection);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder(daProducts);
                    daProducts.Fill(dtbl);
                    userViewModel.UserID = Convert.ToInt32(dtbl.Rows[0]["user_id"].ToString());
                }
                HttpContext.Response.Cookies.Append("id", (userViewModel.UserID).ToString());
                HttpContext.Response.Cookies.Append("email", userViewModel.Email);
                HttpContext.Response.Cookies.Append("role", userViewModel.Role);
                HttpContext.Response.Cookies.Append("firstname", userViewModel.FirstName_);

                return RedirectToAction("Index", new { Controller = "Home", Action = "Index" });   

            }
            return View(userViewModel);
        }


        // GET: /User/TransactionHistoryReport (Work on later)
        public IActionResult TransactionHistoryReport()
        {
            UserViewModel userViewModel = new UserViewModel();
            return View(userViewModel);
        }


        public async Task<IActionResult> Details()
        {
            UserViewModel userViewModel = FetchUserByID();
             return View(userViewModel);
        }

        public IActionResult Edit(int id)
        {
            UserViewModel userViewModel = FetchUserByID();
            return View(userViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("UserID, Password, FirstName_, LastName_, Email, Address, Zipcode, City, State, Date")] UserViewModel userViewModel)
        {

            //function to check if email exists in order to avoid repetition!!!!!!!
            if (ModelState.IsValid)
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    
                    sqlConnection.Open();
                     MySqlCommand sqlCmd;

                    if (Request.Cookies["role"] == "customers")
                    {
                        sqlCmd = new MySqlCommand("EditCustomer", sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@id", userViewModel.UserID);
                        sqlCmd.Parameters.AddWithValue("@FirstName_", userViewModel.FirstName_);
                        sqlCmd.Parameters.AddWithValue("@LastName_", userViewModel.LastName_);
                        sqlCmd.Parameters.AddWithValue("@Email_", userViewModel.Email);
                        sqlCmd.Parameters.AddWithValue("@Password_", userViewModel.Password);
                        sqlCmd.Parameters.AddWithValue("@Role", userViewModel.Role);
                        sqlCmd.Parameters.AddWithValue("@Address_", userViewModel.Address);
                        sqlCmd.Parameters.AddWithValue("@Zipcode_", userViewModel.Zipcode);
                        sqlCmd.Parameters.AddWithValue("@City_", userViewModel.City);
                        sqlCmd.Parameters.AddWithValue("@State_", userViewModel.State);
                                            sqlCmd.ExecuteNonQuery();
                    sqlConnection.Close();
                    }
                    else
                    {
                        sqlCmd = new MySqlCommand("EditEmployee", sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@id", userViewModel.UserID);
                        sqlCmd.Parameters.AddWithValue("@FirstName_", userViewModel.FirstName_);
                        sqlCmd.Parameters.AddWithValue("@LastName_", userViewModel.LastName_);
                        sqlCmd.Parameters.AddWithValue("@Email_", userViewModel.Email);
                        sqlCmd.Parameters.AddWithValue("@Password_", userViewModel.Password);
                                            sqlCmd.ExecuteNonQuery();
                    sqlConnection.Close();
                    }

                }
                return RedirectToAction("Details");
            }
            return RedirectToAction("Details");
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
                if (userrole == "customer")
                    sql = string.Format("SELECT * FROM Users, Customers WHERE Users.user_id = '"+userid+"' AND Customers.CustomerID = '"+userid+"'");
                else
                    sql = string.Format("SELECT * FROM Users, Employees WHERE Users.user_id = '"+userid+"' AND Employees.employee_id = '"+userid+"'");
                daProducts = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daProducts);
                daProducts.Fill(dtbl);
                if (dtbl.Rows.Count == 1 && userrole == "customer")
                {
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

                }
                else if (dtbl.Rows.Count == 1 && userrole == "employee")
                {
                    userViewModel.UserID = userid;
                    userViewModel.Role = userrole; 
                    userViewModel.Password = dtbl.Rows[0]["password"].ToString();
                    userViewModel.Email = dtbl.Rows[0]["email"].ToString(); 
                    userViewModel.FirstName_ = dtbl.Rows[0]["FirstName"].ToString(); 
                    userViewModel.LastName_ = dtbl.Rows[0]["LastName"].ToString(); 
                    userViewModel.Date = (dtbl.Rows[0]["DateJoined"]).ToString();
                }
                return userViewModel;
            }
        }
    }
}