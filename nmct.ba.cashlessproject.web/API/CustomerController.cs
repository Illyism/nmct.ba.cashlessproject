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
    public class CustomerController : ApiController
    {
        // GET: api/Customer
        public IEnumerable<Customer> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return CustomerDA.GetCustomers(p.Claims);
        }

        // GET: api/Customer/5
        public Customer Get(string id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return CustomerDA.GetCustomerByNationalNumber(id, p.Claims);
        }

        // POST: api/Customer
        public HttpResponseMessage Post(Customer c)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            int id = CustomerDA.InsertCustomer(c, p.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }

        // PUT: api/Customer/5
        public HttpResponseMessage Put(Customer c)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            CustomerDA.UpdateCustomer(c, p.Claims);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE: api/Customer/5
        public HttpResponseMessage Delete(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            CustomerDA.DeleteCustomer(id, p.Claims);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
