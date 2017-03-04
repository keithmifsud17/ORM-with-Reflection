using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Model;

namespace TestApp.Controller
{
    class ProductController
    {
        #region SyncMethods
        public IEnumerable<Product> GetProducts()
        {
            var data = new DataHelper<Product>();
            return data.GetAll();
        }

        public Product GetProduct(int ID)
        {
            var data = new DataHelper<Product>();
            return data.Get(new { ID = ID }).SingleOrDefault();
        }

        public bool Insert(Product product)
        {
            var data = new DataHelper<Product>();
            return data.Insert(product);
        }

        public bool Insert(Product[] products)
        {
            var data = new DataHelper<Product>();
            using (var SQL = data.Connection.CreateCommand()) //Keeps all operations in the same transaction
            {
                foreach (var product in products)
                {
                    if (!data.Insert(product))
                        return false;
                }
            }
            return true;
        }

        public bool Update(Product product)
        {
            var data = new DataHelper<Product>();
            return data.Update(product);
        }

        public bool Update(Product[] products)
        {
            var data = new DataHelper<Product>();
            using (var SQL = data.Connection.CreateCommand()) //Keeps all operations in the same transaction
            {
                foreach (var product in products)
                {
                    if (!data.Update(product))
                        return false;
                }
            }
            return true;
        }

        public bool Delete(Product product)
        {
            var data = new DataHelper<Product>();
            return data.Delete(product);
        }

        public bool Delete(Product[] products)
        {
            var data = new DataHelper<Product>();
            using (var SQL = data.Connection.CreateCommand()) //Keeps all operations in the same transaction
            {
                foreach (var product in products)
                {
                    if (!data.Delete(product))
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region AsyncMethods
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var data = new DataHelper<Product>();
            return await data.GetAllAsync();
        }

        public async Task<Product> GetProductAsync(int ID)
        {
            var data = new DataHelper<Product>();
            return (await data.GetAsync(new { ID = ID })).SingleOrDefault();
        }

        public async Task<bool> InsertAsync(Product product)
        {
            var data = new DataHelper<Product>();
            return await data.InsertAsync(product);
        }

        public async Task<bool> InsertAsync(Product[] products)
        {
            var data = new DataHelper<Product>();
            using (var SQL = data.Connection.CreateCommand()) //Keeps all operations in the same transaction
            {
                foreach (var product in products)
                {
                    if (!await data.InsertAsync(product))
                        return false;
                }
            }
            return true;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var data = new DataHelper<Product>();
            return await data.UpdateAsync(product);
        }

        public async Task<bool> UpdateAsync(Product[] products)
        {
            var data = new DataHelper<Product>();
            using (var SQL = data.Connection.CreateCommand()) //Keeps all operations in the same transaction
            {
                foreach (var product in products)
                {
                    if (!await data.UpdateAsync(product))
                        return false;
                }
            }
            return true;
        }

        public async Task<bool> DeleteAsync(Product product)
        {
            var data = new DataHelper<Product>();
            return await data.DeleteAsync(product);
        }

        public async Task<bool> DeleteAsync(Product[] products)
        {
            var data = new DataHelper<Product>();
            using (var SQL = data.Connection.CreateCommand()) //Keeps all operations in the same transaction
            {
                foreach (var product in products)
                {
                    if (!await data.DeleteAsync(product))
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
}
