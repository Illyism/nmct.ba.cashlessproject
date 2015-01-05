using nmct.ba.cashlessproject.web.Helpers;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using nmct.ba.cashlessproject.web.Models.API;

namespace nmct.ba.cashlessproject.web.api.Controllers
{
    public class ErrorLogController : ApiController
    {
        // GET: api/ErrorLog
        public IEnumerable<ErrorLog> Get()
        {
            DbDataReader reader = Database.GetData("CashlessAdmin", "Select * from ErrorLog;");
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
            DbParameter par = Database.AddParameter("CashlessAdmin", "ID", id);
            DbParameter[] pars = { par };
            DbDataReader reader = Database.GetData("CashlessAdmin", "Select * from ErrorLog WHERE ID=@ID;", pars);
            List<ErrorLog> list = new List<ErrorLog>();
            while (reader.Read())
                list.Add(ConstructErrorLog(reader));
            reader.Close();
            if (list.Count == 0) return null;
            return list[0];
        }

        // POST: api/Errorlog
        public HttpResponseMessage Post(ErrorLog c)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            int id = ErrorLogDA.InsertErrorLog(c, p.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }
    }
}
