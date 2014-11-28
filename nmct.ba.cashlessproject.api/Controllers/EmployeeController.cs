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

namespace nmct.ba.cashlessproject.api.Controllers
{
    public class EmployeeController : ApiController
    {
        // GET: api/Employee
        public IEnumerable<Employee> Get()
        {
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Employee;");
            List<Employee> list = new List<Employee>();
            while (reader.Read())
                list.Add(ConstructEmployee(reader));
            reader.Close();
            return list;
        }

        private Employee ConstructEmployee(DbDataReader reader)
        {
            return new Employee()
            {
                ID = int.Parse(reader["ID"].ToString()),
                EmployeeName = reader["EmployeeName"].ToString(),
                Address = reader["Address"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString()
            };
        }

        // GET: api/Employee/5
        public Employee Get(int id)
        {
            DbParameter par = Database.AddParameter("ConnectionString", "ID", id);
            DbParameter[] pars = { par };
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Employee WHERE ID=@ID;", pars);
            List<Employee> list = new List<Employee>();
            while (reader.Read())
                list.Add(ConstructEmployee(reader));
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/Employee
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Employee/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Employee/5
        public void Delete(int id)
        {
        }
    }
}
