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
            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                conn.Open();
                string sql = "Set @val = 30;SELECT * FROM VendorPurchases WHERE quantity = @val";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    System.Diagnostics.Debug.WriteLine(rdr[0] + " -- " + rdr[1]);
                }
                rdr.Close();
            }

            if (dtbl != null)
                return View(dtbl);
            else
                return RedirectToAction("Index", new { Controller = "Order", Action = "Index" });
        }
    }
}
    
