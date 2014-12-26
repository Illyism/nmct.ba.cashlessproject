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
    public class OrganisationController : ApiController
    {
        // GET: api/Organisation
        public IEnumerable<Organisation> Get()
        {
            DbDataReader reader = Database.GetData("CashlessAdmin", "Select * from Organisation;");
            List<Organisation> list = new List<Organisation>();
            while (reader.Read())
                list.Add(ConstructOrganisation(reader));
            reader.Close();
            return list;
        }

        private Organisation ConstructOrganisation(DbDataReader reader)
        {
            return new Organisation()
            {
                ID = int.Parse(reader["ID"].ToString()),
                Login = reader["Login"].ToString(),
                Password = reader["Password"].ToString(),
                DbName = reader["DbName"].ToString(),
                DbLogin = reader["DbLogin"].ToString(),
                DbPassword = reader["DbPassword"].ToString(),
                OrganisationName = reader["OrganisationName"].ToString(),
                Address = reader["Address"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString()
            };
        }

        // GET: api/Organisation/5
        public Organisation Get(int id)
        {
            DbParameter par = Database.AddParameter("CashlessAdmin", "ID", id);
            DbParameter[] pars = { par };
            DbDataReader reader = Database.GetData("CashlessAdmin", "Select * from Organisation WHERE ID=@ID;", pars);
            List<Organisation> list = new List<Organisation>();
            while (reader.Read())
                list.Add(ConstructOrganisation(reader));
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/Organisation
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Organisation/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Organisation/5
        public void Delete(int id)
        {
        }
    }
}
