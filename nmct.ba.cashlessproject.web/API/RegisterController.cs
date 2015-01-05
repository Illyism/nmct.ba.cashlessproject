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
using nmct.ba.cashlessproject.web.api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace nmct.ba.cashlessproject.web.api.Controllers
{
    public class RegisterController : ApiController
    {
        // GET: api/Register
        public IEnumerable<Register> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return RegisterDA.GetRegisters(p.Claims);
        }

        // GET: api/Register/5
        public HttpResponseMessage Get(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            IEnumerable<RegisterEmployee> list = RegisterDA.GetRegisterEmployees(id, p.Claims);
            string output = JsonConvert.SerializeObject(list);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(output, System.Text.Encoding.UTF8, "application/json")
            };
            return resp;
        }

        // POST: api/Product
        public HttpResponseMessage Post(RegisterEmployee regemp)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            int id = RegisterDA.InsertRegEmp(regemp, p.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }
    }
}
