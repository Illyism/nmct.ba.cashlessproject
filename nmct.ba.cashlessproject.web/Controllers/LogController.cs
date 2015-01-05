using nmct.ba.cashlessproject.web.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nmct.ba.cashlessproject.web.Controllers
{
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index()
        {
            ErrorLogDA.Sync();
            ViewBag.index = 0;
            return View(ErrorLogDA.GetErrorLogs());
        }
    }
}