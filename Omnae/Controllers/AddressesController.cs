using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Omnae.Model.Models;
using Omnae.Data;

namespace Omnae.Controllers
{
    public class AddressesController : Controller
    {
        public AddressesController(OmnaeContext db)
        {
            this.DB = db;
        }

        private OmnaeContext DB { get; }

        // GET: Addresses
        public ActionResult Index()
        {
            var addresses = DB.Addresses.Include(a => a.StateProvince);
            return View(addresses.ToList());
        }

        // GET: Addresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Address address = DB.Addresses.Find(id);
            if (address == null)
                return HttpNotFound();
            
            return View(address);
        }

        // GET: Addresses/Create
        public ActionResult Create()
        {
            ViewBag.StateProvinceId = new SelectList(DB.StateProvinces, "Id", "Name");
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AddressLine1,AddressLine2,City,Country,StateProvinceId,PostalCode,isBilling,isShipping")] Address address)
        {
            if (ModelState.IsValid)
            {
                DB.Addresses.Add(address);
                DB.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateProvinceId = new SelectList(DB.StateProvinces, "Id", "Name", address.StateProvince);
            return View(address);
        }

        // GET: Addresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Address address = DB.Addresses.Find(id);
            if (address == null)
                return HttpNotFound();
            
            ViewBag.StateProvinceId = new SelectList(DB.StateProvinces, "Id", "Name", address.StateProvince);
            return View(address);
        }

        // POST: Addresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AddressLine1,AddressLine2,City,Country,StateProvinceId,PostalCode,isBilling,isShipping")] Address address)
        {
            if (ModelState.IsValid)
            {
                DB.Entry(address).State = EntityState.Modified;
                DB.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateProvinceId = new SelectList(DB.StateProvinces, "Id", "Name", address.StateProvince);
            return View(address);
        }

        // GET: Addresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Address address = DB.Addresses.Find(id);
            if (address == null)
                return HttpNotFound();
            
            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Address address = DB.Addresses.Find(id);
            DB.Addresses.Remove(address);
            DB.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
