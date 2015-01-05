using nmct.ba.cashlessproject.web.Helpers;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using nmct.ba.cashlessproject.web.api.Models;

namespace nmct.ba.cashlessproject.web.api.Controllers
{
    public class EmployeeController : ApiController
    {
        // GET: api/Employee
        public IEnumerable<Employee> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return EmployeeDA.GetEmployees(p.Claims);
        }

        // GET: api/Employee/5
        public Employee Get(string id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return EmployeeDA.GetEmployeeByNationalNumber(id, p.Claims);
        }

        // POST: api/Employee
        public HttpResponseMessage Post(Employee c)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            int id = EmployeeDA.InsertEmployee(c, p.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }

        // PUT: api/Employee/5
        public HttpResponseMessage Put(Employee c)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            EmployeeDA.UpdateEmployee(c, p.Claims);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE: api/Employee/5
        public HttpResponseMessage Delete(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            EmployeeDA.DeleteEmployee(id, p.Claims);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
