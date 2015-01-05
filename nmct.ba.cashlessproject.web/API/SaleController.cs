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
    public class SaleController : ApiController
    {
        // GET: api/Sale
        public IEnumerable<Sale> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return SaleDA.GetSales(p.Claims);
        }

        [ActionName("Register")]
        public IEnumerable<Sale> GetByRegister(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return SaleDA.GetSaleByRegister(id, p.Claims);
        }

        [ActionName("Product")]
        public IEnumerable<Sale> GetByProduct(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return SaleDA.GetSaleByProduct(id, p.Claims);
        }

        // POST: api/Sale
        public HttpResponseMessage Post(Sale s)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            int id = SaleDA.InsertSale(s, p.Claims);
            CustomerDA.AddSale(s, p.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }

    }
}
