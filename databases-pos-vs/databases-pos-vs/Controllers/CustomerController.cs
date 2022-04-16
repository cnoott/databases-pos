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

namespace databseApp.Controllers
{
    public class CustomerController : Controller
    {
        
        private readonly IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }





        public IActionResult Data()
        {
            MySqlDataAdapter daTransactions;
            DataTable dtbl = new DataTable();
            int userid = Int32.Parse(Request.Cookies["id"]);
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();

                string query = "SELECT * From Transaction_Info WHERE customer_id = '"+userid+"' ORDER BY transaction_id DESC";

              
                daTransactions = new MySqlDataAdapter(query, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daTransactions);
                daTransactions.Fill(dtbl);
            }
            return View(dtbl);
        }


        public async Task<IActionResult> Purchase_Details(int id)
        {
            MySqlDataAdapter daTransactions;
            DataTable dtbl = new DataTable();
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {

                sqlConnection.Open();
                string query = "SELECT transaction_info_id, Products.product_id, name, size, price, quantity FROM Transaction_Info, Transactions, Products WHERE Transaction_Info.transaction_id = '"+id+"' AND transaction_info_id = '"+id+"' AND Transactions.product_id = Products.product_id";
                daTransactions = new MySqlDataAdapter(query, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daTransactions);
                daTransactions.Fill(dtbl);
            }

            if (dtbl != null)
                return View(dtbl);
            else
                return RedirectToAction("Index", new { Controller = "Home", Action = "Index" });
        }
    }
}
