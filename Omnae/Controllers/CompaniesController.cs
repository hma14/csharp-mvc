using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Omnae.Model.Models;
using Omnae.Data;

namespace Omnae.Controllers
{
    public class CompaniesController : Controller
    {
        public CompaniesController(OmnaeContext db)
        {
            this.DB = db;
        }

        private OmnaeContext DB { get; }

        // GET: Companies
        public ActionResult Index()
        {
            var companies = DB.Companies.Include(c => c.Address).Include(c => c.Shipping);
            return View(companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Company company = DB.Companies.Find(id);
            if (company == null)
                return HttpNotFound();
            
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            ViewBag.AddressId = new SelectList(DB.Addresses, "Id", "AddressLine1");
            ViewBag.ShippingId = new SelectList(DB.Shippings, "Id", "Attention_FreeText");
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,AddressId,ShippingId")] Company company)
        {
            if (ModelState.IsValid)
            {
                DB.Companies.Add(company);
                DB.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AddressId = new SelectList(DB.Addresses, "Id", "AddressLine1", company.AddressId);
            ViewBag.ShippingId = new SelectList(DB.Shippings, "Id", "Attention_FreeText", company.ShippingId);
            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Company company = DB.Companies.Find(id);
            if (company == null)
                return HttpNotFound();
            
            ViewBag.AddressId = new SelectList(DB.Addresses, "Id", "AddressLine1", company.AddressId);
            ViewBag.ShippingId = new SelectList(DB.Shippings, "Id", "Attention_FreeText", company.ShippingId);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AddressId,ShippingId")] Company company)
        {
            if (ModelState.IsValid)
            {
                DB.Entry(company).State = EntityState.Modified;
                DB.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AddressId = new SelectList(DB.Addresses, "Id", "AddressLine1", company.AddressId);
            ViewBag.ShippingId = new SelectList(DB.Shippings, "Id", "Attention_FreeText", company.ShippingId);
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Company company = DB.Companies.Find(id);
            if (company == null)
                return HttpNotFound();
            
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = DB.Companies.Find(id);
            DB.Companies.Remove(company);
            DB.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
