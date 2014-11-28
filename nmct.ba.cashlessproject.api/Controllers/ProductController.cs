using nmct.ba.cashlessproject.classlibrary;
using NMCT.DropBox.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Feels;

namespace nmct.ba.cashlessproject.api.Controllers
{
    public class ProductController : ApiController
    {
        // GET: api/Product
        public IEnumerable<Product> Get()
        {
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Product;");
            List<Product> list = new List<Product>();
            while (reader.Read())
            {
                Product p = new Product()
                {
                    ID = int.Parse(reader["ID"].ToString()),
                    ProductName = reader["ProductName"].ToString(),
                    Price = double.Parse(reader["Price"].ToString())
                };
                list.Add(p);
            }
            reader.Close();
            return list;
        }

        // GET: api/Product/5
        public Product Get(int id)
        {
            DbParameter par = Database.AddParameter("ConnectionString", "ID", id);
            DbParameter[] pars = {par};
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Product WHERE ID=@ID;", pars);
            List<Product> list = new List<Product>();
            while (reader.Read())
            {
                Product p = new Product()
                {
                    ID = int.Parse(reader["ID"].ToString()),
                    ProductName = reader["ProductName"].ToString(),
                    Price = double.Parse(reader["Price"].ToString())
                };
                list.Add(p);
            }
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/Product
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Product/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Product/5
        public void Delete(int id)
        {
        }
    }
}
