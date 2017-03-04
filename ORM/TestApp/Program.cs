using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM;
using TestApp.Model;
using TestApp.Controller;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionSettings.Instance.RegisterConnectionString("Test Connection", "CONNECTION STRING HERE");

            RunTestsAsync().Wait();
        }

        static async Task RunTestsAsync()
        {

            var controller = new ProductController();

            //Insert a Product
            var product = new Product() { ID = 1, Description = "Test Product A", Barcode = "" };
            await controller.InsertAsync(product);

            //Update a Product
            product.Description = "Change in Description";
            await controller.UpdateAsync(product);

            //Delete a Product
            await controller.DeleteAsync(product);
        }
    }
}
