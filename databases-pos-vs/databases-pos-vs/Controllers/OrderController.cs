
﻿using databseApp.Models;
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

        //GET: /Order/Report
        public IActionResult Report()
        {
            OrderViewModel orderViewModel = new OrderViewModel();
            return View(orderViewModel);
        }


        [HttpPost]
        public IActionResult Report(string query1, [Bind("StartDate,EndDate,minQuantity,maxQuantity")] OrderViewModel orderViewModel)
        {
            MySqlDataAdapter daTransactions;
            DataTable dtbl = new DataTable();

            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string sql = string.Format("SELECT * From VendorPurchases " +
                    "INNER JOIN Inventories ON VendorPurchases.inventory_id = Inventories.inventory_id " +
                    "INNER JOIN Products ON Inventories.inventory_id = Products.product_id " +
                    "WHERE (date_buy >= \"{0}\" AND date_buy < \"{1}\") AND (quantity >= \"{2}\"AND quantity < \"{3}\")",
                    orderViewModel.StartDate, orderViewModel.EndDate, orderViewModel.minQuantity, orderViewModel.maxQuantity);



                return RedirectToAction(nameof(Data), new { query = sql });
            }
        }

        //Get /Transaction/ViewTable
        // instead of returning the datatable, returen the query string and then perform the querry in the VieDTable()
        public IActionResult Data(string query)
        {
            MySqlDataAdapter daTransactions;
            int userid = Int32.Parse(Request.Cookies["id"]);
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
