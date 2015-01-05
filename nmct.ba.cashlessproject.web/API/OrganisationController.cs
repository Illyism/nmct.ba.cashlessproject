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
using nmct.ba.cashlessproject.web.Models;
using System.Web;

namespace nmct.ba.cashlessproject.web.api.Controllers
{
    public class PasswordForm
    {
        public string password;
    }

    public class OrganisationController : ApiController
    {
        [HttpPost]
        [ActionName("Password")]
        public HttpResponseMessage Password(PasswordForm pass)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            OrganisationDA.ChangePassword(p.Claims, pass.password);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
