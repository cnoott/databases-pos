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

namespace databseApp.Controllers
{
    public class VendorController : Controller
    {
        private readonly IConfiguration _configuration;

        public VendorController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        /*
        private readonly databases_pos_vsContext _context;

        public VendorController()
        {
          
        }*/

        // GET: Vendor
        public IActionResult Index()
        {
            MySqlDataAdapter vendors;
            DataTable dtbl = new DataTable();
     
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string sql = "SELECT * FROM Vendors";
                vendors = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(vendors);
                vendors.Fill(dtbl);
            }

            return View(dtbl);
        }

        public IActionResult Create()
        {
            VendorViewModel vendorVM = new VendorViewModel();
            return View(vendorVM);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("VendorName, Location, Manager")] VendorViewModel vendorViewModel)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    MySqlCommand sqlCmd = new MySqlCommand("AddVendor", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@name_", vendorViewModel.VendorName);
                    sqlCmd.Parameters.AddWithValue("@manager_", vendorViewModel.Location);
                    sqlCmd.Parameters.AddWithValue("@location_", vendorViewModel.Manager);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vendorViewModel);
        }
     
        public IActionResult Edit(int id)
        {
            VendorViewModel vendor = FetchVendor(id);
            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("VendorID, VendorName, Location, Manager")] VendorViewModel vendorViewModel)
        {

            if (ModelState.IsValid)
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    
                    sqlConnection.Open();
                    MySqlCommand sqlCmd;

                    sqlCmd = new MySqlCommand("EditVendor", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@id_", vendorViewModel.VendorID);
                    sqlCmd.Parameters.AddWithValue("@name_", vendorViewModel.VendorName);
                    sqlCmd.Parameters.AddWithValue("@manager_", vendorViewModel.Manager);
                    sqlCmd.Parameters.AddWithValue("@location_", vendorViewModel.Location);

                    sqlCmd.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            return RedirectToAction("Index");
        } 


        public VendorViewModel FetchVendor(int id)
        {
            VendorViewModel vendorViewModel = new VendorViewModel();
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                MySqlDataAdapter daProducts;
                DataTable dtbl = new DataTable();

                sqlConnection.Open();
                string sql = string.Format("SELECT * FROM Vendors WHERE '"+id+"' = vendorID");

                daProducts = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daProducts);
                daProducts.Fill(dtbl);
                vendorViewModel.VendorID = Convert.ToInt32(dtbl.Rows[0]["vendorID"].ToString());
                vendorViewModel.VendorName = dtbl.Rows[0]["vendor_name"].ToString(); 
                vendorViewModel.Location = dtbl.Rows[0]["location"].ToString(); 
                vendorViewModel.Manager = dtbl.Rows[0]["manager"].ToString(); 
            }
            return vendorViewModel;
        }
    }
}
