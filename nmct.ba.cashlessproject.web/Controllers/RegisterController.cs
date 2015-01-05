using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nmct.ba.cashlessproject.web.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        [Authorize]
        public ActionResult Index()
        {
            List<Register> regs = RegisterDA.GetRegisters();
            ViewBag.OrganisationRegisters = RegisterDA.GetOrganisationRegisters() as IEnumerable<OrganisationRegister>;
            ViewBag.UnusedRegisters = RegisterDA.GetUnusedRegisters() as IEnumerable<Register>;
            return View(regs);
        }

        

        // GET: Register/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            Register reg = RegisterDA.GetRegister(id);
            return View(reg);
        }

        // GET: Register/Create
        [Authorize]
        public ActionResult Create()
        {
            Register reg = new Register()
            {
                PurchaseDate = DateTime.Now,
                ExpiresDate = new DateTime(DateTime.Now.Year + 5, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0)
            };
            return View(reg);
        }

        // POST: Register/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Register reg)
        {
            try
            {
                RegisterDA.InsertRegister(reg);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(reg);
            }
        }

        // GET: Register/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Register reg = RegisterDA.GetRegister(id);
            return View(reg);
        }

        // POST: Register/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(Register reg, FormCollection collection)
        {
            try
            {
                RegisterDA.UpdateRegister(reg);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Register/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            Register reg = RegisterDA.GetRegister(id);
            return View(reg);
        }

        // POST: Register/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection form)
        {
            try
            {
                RegisterDA.DeleteRegister(id);
                return RedirectToAction("Index");
            }
            catch
            {
                Register reg = RegisterDA.GetRegister(id);
                return View(reg);
            }
        }




        // GET: Register/Assign/5
        [Authorize]
        public ActionResult Assign(int id)
        {
            Register reg = RegisterDA.GetRegister(id);

            ViewBag.Organisations = new SelectList(OrganisationDA.GetOrganisations(), "ID", "OrganisationName");
            ViewBag.Register = reg;
            OrganisationRegister orgReg = new OrganisationRegister()
            {
                RegisterName = reg.RegisterName,
                RegisterID = reg.ID,
                Device = reg.Device,
                FromDate = DateTime.Now,
                UntilDate = reg.ExpiresDate
            };
            return View(orgReg);
        }

        // POST: Register/Assign/5
        [Authorize]
        [HttpPost]
        public ActionResult Assign(OrganisationRegister orgReg)
        {
            try
            {
                RegisterDA.AssignRegister(orgReg);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Organisations = new SelectList(OrganisationDA.GetOrganisations(), "ID", "OrganisationName");
                return View(orgReg);
            }
        }

        // GET: Register/UnAssign/5
        [Authorize]
        public ActionResult UnAssign(int? regId)
        {
            if (regId == null) return RedirectToAction("Index");
            OrganisationRegister orgReg = RegisterDA.GetOrganisationRegister(regId.Value);
            return View(orgReg);
        }

        // POST: Register/UnAssign/5
        [HttpPost]
        [Authorize]
        public ActionResult UnAssign(int regId)
        {
            try
            {
                RegisterDA.UnAssignRegister(regId);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

    }
}
