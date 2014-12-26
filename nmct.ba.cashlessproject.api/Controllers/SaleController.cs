using nmct.ba.cashlessproject.api.Helpers;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace nmct.ba.cashlessproject.api.Controllers
{
    public class SaleController : ApiController
    {
        // GET: api/Sale
        public IEnumerable<Sale> Get()
        {
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Sale;");
            List<Sale> list = new List<Sale>();
            while (reader.Read())
                list.Add(ConstructSale(reader));
            reader.Close();
            return list;
        }

        private Sale ConstructSale(DbDataReader reader)
        {
            return new Sale()
            {
                ID = int.Parse(reader["ID"].ToString()),
                Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                CustomerID = int.Parse(reader["CustomerID"].ToString()),
                RegisterID = int.Parse(reader["RegisterID"].ToString()),
                ProductID = int.Parse(reader["ProductID"].ToString()),
                Amount = int.Parse(reader["Amount"].ToString()),
                TotalPrice = double.Parse(reader["TotalPrice"].ToString())
            };
        }

        // GET: api/Sale/5
        public Sale Get(int id)
        {
            DbParameter par = Database.AddParameter("ConnectionString", "ID", id);
            DbParameter[] pars = { par };
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Sale WHERE ID=@ID;", pars);
            List<Sale> list = new List<Sale>();
            while (reader.Read())
                list.Add(ConstructSale(reader));
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/Sale
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Sale/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Sale/5
        public void Delete(int id)
        {
        }
    }
}
