using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using databaseApp.Models;
using databases_pos_vs.Data;

namespace databases_pos_vs.Controllers
{
    public class VendorController : Controller
    {
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

        

     
    }
}
