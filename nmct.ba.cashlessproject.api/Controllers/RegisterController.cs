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
    public class RegisterController : ApiController
    {
        // GET: api/Register
        public IEnumerable<Register> Get()
        {
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Register;");
            List<Register> list = new List<Register>();
            while (reader.Read())
                list.Add(ConstructRegister(reader));
            reader.Close();
            return list;
        }

        private Register ConstructRegister(DbDataReader reader)
        {
            return new Register()
            {
                ID = int.Parse(reader["ID"].ToString()),
                RegisterName = reader["RegisterName"].ToString(),
                Device = reader["Device"].ToString(),
                PurchaseDate = DateTime.Parse(reader["PurchaseDate"].ToString()),
                ExpiresDate = DateTime.Parse(reader["ExpiresDate"].ToString())
            };
        }

        // GET: api/Register/5
        public Register Get(int id)
        {
            DbParameter par = Database.AddParameter("ConnectionString", "ID", id);
            DbParameter[] pars = { par };
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.Register WHERE ID=@ID;", pars);
            List<Register> list = new List<Register>();
            while (reader.Read())
                list.Add(ConstructRegister(reader));
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/Register
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Register/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Register/5
        public void Delete(int id)
        {
        }
    }
}
