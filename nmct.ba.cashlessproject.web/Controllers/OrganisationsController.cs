using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nmct.ba.cashlessproject.web.Controllers
{
    public class OrganisationsController : Controller
    {
        // GET: Organisations
        [Authorize]
        public ActionResult Index()
        {
            List<Organisation> orgs = OrganisationDA.GetOrganisations();
            return View(orgs);
        }

        // GET: Organisations/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            Organisation org = OrganisationDA.DecryptOrganisation(OrganisationDA.GetOrganisation(id));
            return View(org);
        }

        // GET: Organisations/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Organisations/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Organisation dOrg)
        {
                Organisation org = OrganisationDA.EncryptOrganisation(dOrg);
                OrganisationDA.InsertOrganisation(org);
                OrganisationDA.MakeDB(dOrg);
                return RedirectToAction("Index");
        }

        // GET: Organisations/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Organisation org = OrganisationDA.DecryptOrganisation(OrganisationDA.GetOrganisation(id));
            return View(org);
        }

        // POST: Organisations/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(Organisation dOrg, FormCollection collection)
        {
            Organisation org = OrganisationDA.EncryptOrganisation(dOrg);
            OrganisationDA.UpdateOrganisation(org);
            return RedirectToAction("Index");
        }

        // GET: Organisations/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            Organisation org = OrganisationDA.DecryptOrganisation(OrganisationDA.GetOrganisation(id));
            return View(org);
        }

        // POST: Organisations/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                OrganisationDA.DeleteOrganisation(id);
                return RedirectToAction("Index");
            }
            catch
            {
                Organisation org = OrganisationDA.DecryptOrganisation(OrganisationDA.GetOrganisation(id));
                return View(org);
            }
        }
    }
}
