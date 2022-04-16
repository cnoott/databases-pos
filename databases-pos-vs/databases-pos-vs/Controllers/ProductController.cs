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
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            this._configuration = configuration;

        }

        // GET: Product
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


        //GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ProductViewModel productViewModel = FetchProductByID(id);
            return View(productViewModel);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            MySqlDataAdapter daVendors;
            DataTable dtbl = new DataTable();
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string sql = "SELECT * FROM Vendors";
                daVendors = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daVendors);
                daVendors.Fill(dtbl);
            }
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.vendorsize = dtbl.Rows.Count;
         
            return View(productViewModel);
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create([Bind("ProductId,Size,Price,Name,Image_url,Vendor_id, VendorPrice, Quantity")] ProductViewModel productViewModel)

        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    MySqlCommand sqlCmd = new MySqlCommand("ProductsAddOrEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@Product_id", productViewModel.ProductId);
                    sqlCmd.Parameters.AddWithValue("@Size", productViewModel.Size);
                    sqlCmd.Parameters.AddWithValue("@Price", productViewModel.Price);
                    sqlCmd.Parameters.AddWithValue("@Name", productViewModel.Name);

                    sqlCmd.Parameters.AddWithValue("@Image_url", productViewModel.Image_url);

                    sqlCmd.Parameters.AddWithValue("@Category_id", productViewModel.Category_id);
                    sqlCmd.Parameters.AddWithValue("@Vendor_id_", productViewModel.Vendor_id);
                    sqlCmd.Parameters.AddWithValue("@Quantity_", productViewModel.Quantity);
                    sqlCmd.Parameters.AddWithValue("@Vendor_Price_", productViewModel.VendorPrice);


                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // GET: Product/Edit/
        public IActionResult Edit(int id)
        {
            ProductViewModel productViewModel = new ProductViewModel();
            if (id > 0)
                productViewModel = FetchProductByID(id);
            return View(productViewModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProductId,Size,Price,Name,Image_url")] ProductViewModel productViewModel)
        {


            if (ModelState.IsValid)
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    MySqlCommand sqlCmd = new MySqlCommand("ProductsAddOrEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@Product_id", productViewModel.ProductId);
                    sqlCmd.Parameters.AddWithValue("@Size", productViewModel.Size);
                    sqlCmd.Parameters.AddWithValue("@Price", productViewModel.Price);
                    sqlCmd.Parameters.AddWithValue("@Name", productViewModel.Name);
                    sqlCmd.Parameters.AddWithValue("@Category_id", productViewModel.Category_id);
                    sqlCmd.Parameters.AddWithValue("@Vendor_id", productViewModel.Vendor_id);
                    sqlCmd.Parameters.AddWithValue("@Image_url", productViewModel.Image_url);



                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);


        }

 


        public ProductViewModel FetchProductByID(int? id)
        {

            ProductViewModel productViewModel = new ProductViewModel();
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                MySqlDataAdapter daProducts;
                DataTable dtbl = new DataTable();

                sqlConnection.Open();
                var sql = string.Format("SELECT * FROM Products WHERE Products.product_id = {0}", id);
                daProducts = new MySqlDataAdapter(sql, sqlConnection);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daProducts);
                daProducts.Fill(dtbl);


                if (dtbl.Rows.Count == 1)
                {
                    productViewModel.ProductId = Convert.ToInt32(dtbl.Rows[0]["product_id"].ToString());
                    productViewModel.Size = dtbl.Rows[0]["size"].ToString();

                    productViewModel.Price = (float)Convert.ToDouble(dtbl.Rows[0]["price"].ToString()); //Cant convert this for some reason 
                    
                    productViewModel.Name = dtbl.Rows[0]["name"].ToString();
                    productViewModel.times_sold = Convert.ToInt32(dtbl.Rows[0]["times_sold"].ToString());


                }
                return productViewModel;
            }
        }

        // GET: Product/Delete/5
        public IActionResult Delete(int id)
        {
            ProductViewModel productViewModel = FetchProductByID(id);
            return View(productViewModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                MySqlCommand sqlCmd = new MySqlCommand("ProductDeleteByID", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@Product_id", id);
                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}