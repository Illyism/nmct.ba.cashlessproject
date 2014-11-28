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
    public class ErrorLogController : ApiController
    {
        // GET: api/ErrorLog
        public IEnumerable<ErrorLog> Get()
        {
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.ErrorLog;");
            List<ErrorLog> list = new List<ErrorLog>();
            while (reader.Read())
                list.Add(ConstructErrorLog(reader));
            reader.Close();
            return list;
        }

        private ErrorLog ConstructErrorLog(DbDataReader reader)
        {
            return new ErrorLog()
            {
                RegisterID = int.Parse(reader["RegisterID"].ToString()),
                Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                Message = reader["Message"].ToString(),
                Stacktrace = reader["Stacktrace"].ToString()
            };
        }

        // GET: api/ErrorLog/5
        public ErrorLog Get(int id)
        {
            DbParameter par = Database.AddParameter("ConnectionString", "ID", id);
            DbParameter[] pars = { par };
            DbDataReader reader = Database.GetData("ConnectionString", "Select * from CashlessProject.dbo.ErrorLog WHERE ID=@ID;", pars);
            List<ErrorLog> list = new List<ErrorLog>();
            while (reader.Read())
                list.Add(ConstructErrorLog(reader));
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/ErrorLog
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ErrorLog/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ErrorLog/5
        public void Delete(int id)
        {
        }
    }
}
