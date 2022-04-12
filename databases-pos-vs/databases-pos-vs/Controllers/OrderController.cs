using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace databases_pos_vs.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration)
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
                string query = "SELECT * From VendorPurchases " +
                    "INNER JOIN Inventories ON VendorPurchases.inventory_id = Inventories.inventory_id " +
                    "INNER JOIN Products ON Inventories.inventory_id = Products.product_id";
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
                string query = "SELECT transaction_info_id, Products.product_id, name, size, price, quantity FROM Transaction_Info, Transactions, Products WHERE Transaction_Info.transaction_id = '" + id + "' AND transaction_info_id = '" + id + "' AND Transactions.product_id = Products.product_id";
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
    
