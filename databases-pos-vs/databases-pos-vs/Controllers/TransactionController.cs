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
    public class TransactionController : Controller
    {
        private readonly IConfiguration _configuration;

        public TransactionController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Transaction
        public IActionResult Index()
        {
            MySqlDataAdapter daTransactions;
            DataTable dtbl = new DataTable();

            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string sql = "SELECT * FROM Transaction_Info";
                daTransactions = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daTransactions);
                daTransactions.Fill(dtbl);


            }
            return View(dtbl);
 
        }

        //GET: /Transaction/Report
        public IActionResult Report()
        {
            TransactionViewModel transactionViewModel = new TransactionViewModel();
            return View(transactionViewModel);
        }


        //POST: /Transaction/Report
        [HttpPost]
        public IActionResult Report(string query, [Bind("StartDate,EndDate,StartPrice,EndPrice")] TransactionViewModel transactionViewModel)
        {

                string sql = string.Format("SELECT * FROM Transaction_Info WHERE (order_date >= order_date < \"{1}\") AND (total_cost >= \"{2}\"AND total_cost < \"{3}\")",
                    transactionViewModel.StartDate, transactionViewModel.EndDate, transactionViewModel.StartPrice, transactionViewModel.EndPrice);

          
            return RedirectToAction(nameof(ViewTable), new { query = sql });
        }

        //Get /Transaction/ViewTable
        // instead of returning the datatable, returen the query string and then perform the querry in the VieDTable()
        public IActionResult ViewTable(string query)
        {
            MySqlDataAdapter daTransactions;
            DataTable dtbl = new DataTable();
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {

                sqlConnection.Open();
              
                daTransactions = new MySqlDataAdapter(query, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daTransactions);
                daTransactions.Fill(dtbl);

                System.Diagnostics.Debug.WriteLine(query);

            }
            return View(dtbl);
        }
 
        public async Task<IActionResult> Details(int id)
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
