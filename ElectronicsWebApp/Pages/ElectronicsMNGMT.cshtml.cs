using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using static ElectronicsWebApp.Pages.DeviceCategories;

namespace ElectronicsWebApp.Pages
{
    public class ElectronicsMNGMTModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string connString = "";

        public ElectronicsMNGMTModel(IConfiguration configuration)
        {
            _configuration = configuration;
            connString = _configuration.GetConnectionString("TuesdayConnString");
        }
        public string message = ""; // for displaying messages in HTML

        public MyDevices devices = new MyDevices();
        public List<DeviceCategories> categoriesList = new List<DeviceCategories>();
        public void OnPost()
        {
            // validations
            if (string.IsNullOrEmpty(Request.Form["DeviceName"]) ||
                string.IsNullOrEmpty(Request.Form["Manufacturer"]))
            {
                message = "Device name or Manufacturer is empty!";
                return;
            }

            // and get inputs form the form
            devices.DeviceName = Request.Form["DeviceName"];
            devices.Manufacturer = Request.Form["Manufacturer"];
            devices.Price = decimal.Parse(Request.Form["Price"]);
            devices.DeviceCounts = int.Parse(Request.Form["DeviceCounts"]);
            devices.DeviceCategory = int.Parse(Request.Form["DeviceCategory"]);
            devices.DevicePicture = Request.Form["DevicePicture"];

            //save data
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO DevicesStore(DeviceName, Manufacturer, Price, DeviceCounts, DeviceCategory, DevicePicture) VALUES(@DeviceName, @Manufacturer, @Price, @DeviceCounts, @DeviceCategory, @DevicePicture"))
                    {
                        cmd.Parameters.AddWithValue("@DeviceName", devices.DeviceName);
                        cmd.Parameters.AddWithValue("@Manufacturer", devices.Manufacturer);
                        cmd.Parameters.AddWithValue("@Price", devices.Price);
                        cmd.Parameters.AddWithValue("@DeviceCounts", devices.DeviceCounts);
                        cmd.Parameters.AddWithValue("@DeviceCategory", devices.DeviceCategory);
                        cmd.Parameters.AddWithValue("@DevicePicture", devices.DevicePicture);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

        }

        public void RetrieveCategories()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT CategoryId, CategoryName FROM DeviceCategory", conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DeviceCategories category = new DeviceCategories();

                                category.CategoryId = reader.GetInt32("CategoryId");
                                category.CategoryName = reader.GetString("CategoryName");

                                categoriesList.Add(category);
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
        public void OnGet()
        {
            RetrieveCategories();
        }
    }
    public class DeviceCategories
    {
        public int CategoryId { get; set; }
        public int CategoryName { get; set; }

        public class MyDevices
        {
            public int DeviceId { get; set; }
            public string DeviceName { get; set; }
            public string Manufacturer { get; set; }
            public decimal Price { get; set; }
            public int DeviceCounts { get; set; }
            public int DeviceCategory { get; set; }
            public string DevicePicture { get; set; }

        }
    }
}
