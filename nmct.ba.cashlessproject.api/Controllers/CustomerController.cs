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
    public class CustomerController : ApiController
    {
        // GET: api/Customer
        public IEnumerable<Customer> Get()
        {
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Customer;");
            List<Customer> list = new List<Customer>();
            while (reader.Read())
            {
                Customer c = new Customer()
                {
                    ID = int.Parse(reader["ID"].ToString()),
                    CustomerName = reader["CustomerName"].ToString(),
                    Address = reader["Address"].ToString(),
                    Balance = double.Parse(reader["Balance"].ToString())
                };
                list.Add(c);
            }
            reader.Close();
            return list;
        }

        // GET: api/Customer/5
        public Customer Get(int id)
        {
            DbParameter par = Database.AddParameter("ConnectionString", "ID", id);
            DbParameter[] pars = {par};
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Customer WHERE ID=@ID;", pars);
            List<Customer> list = new List<Customer>();
            while (reader.Read())
            {
                Customer c = new Customer()
                {
                    ID = int.Parse(reader["ID"].ToString()),
                    CustomerName = reader["CustomerName"].ToString(),
                    Address = reader["Address"].ToString(),
                    Balance = double.Parse(reader["Balance"].ToString())
                };
                list.Add(c);
            }
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/Customer
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Customer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Customer/5
        public void Delete(int id)
        {
        }
    }
}
