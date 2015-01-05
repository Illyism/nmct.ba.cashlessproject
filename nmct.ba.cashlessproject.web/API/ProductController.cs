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
    public class ProductController : ApiController
    {
        // GET: api/Product
        public IEnumerable<Product> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return ProductDA.GetProducts(p.Claims);
        }

        // GET: api/Product/5
        public Product Get(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return ProductDA.GetProduct(id, p.Claims);
        }

        // POST: api/Product
        public HttpResponseMessage Post(Product c)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            int id = ProductDA.InsertProduct(c, p.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }

        // PUT: api/Product/5
        public HttpResponseMessage Put(Product c)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            ProductDA.UpdateProduct(c, p.Claims);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE: api/Product/5
        public HttpResponseMessage Delete(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            ProductDA.DeleteProduct(id, p.Claims);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
