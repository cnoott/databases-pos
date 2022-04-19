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

        
        public IActionResult Report()
        {
            CustomerViewModel cusViewModel = new CustomerViewModel();
            return View(cusViewModel);
        }
        

        //POST: /Transaction/Report
        [HttpPost]
        public IActionResult Report(string query, [Bind("StartDate,EndDate,StartPrice,EndPrice,Shoe")] CustomerViewModel cusViewModel)
        {
                int userid = Int32.Parse(Request.Cookies["id"]);
                string sql;

            if (cusViewModel.Shoe == null || cusViewModel.Shoe == "") { 
                sql = string.Format("SELECT * FROM Transaction_Info WHERE customer_id = '"+userid+"' AND " + 
                    "( '"+cusViewModel.StartDate+"' <= order_date AND order_date <= '"+cusViewModel.EndDate+"')" + 
                    "ORDER BY transaction_id DESC");
            }   
            else
            {
                sql = string.Format("SELECT * FROM Transaction_Info, Transactions, Products WHERE customer_id = '"+userid+"' AND " + 
                    "( '"+cusViewModel.StartDate+"' <= order_date AND order_date <= '"+cusViewModel.EndDate+"')" + 
                    " AND Transaction_Info.transaction_id = Transactions.transaction_info_id " + 
                    " AND Transactions.product_id = Products.product_id " +
                    " AND Products.name = '"+cusViewModel.Shoe+"' " +
                    "ORDER BY Transaction_Info.transaction_id DESC");
            }
            return RedirectToAction(nameof(Data), new { query = sql });
        }


        public IActionResult Data(string query)
        {
            MySqlDataAdapter daTransactions;
            DataTable dtbl = new DataTable();

            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();              
                daTransactions = new MySqlDataAdapter(query, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daTransactions);
                daTransactions.Fill(dtbl);
            }
            return View(dtbl);
        }


        public IActionResult Purchase_Details(int id)
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
