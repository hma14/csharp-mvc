using Omnae.Data;
using Omnae.Model.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Omnae.Controllers
{
    public class StateProvincesController : BaseController
    {
        private OmnaeContext db = new OmnaeContext();

        // GET: StateProvinces
        public ActionResult Index()
        {
            return View(db.StateProvinces.ToList());
        }

        // GET: StateProvinces/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StateProvince stateProvince = db.StateProvinces.Find(id);
            if (stateProvince == null)
            {
                return HttpNotFound();
            }
            return View(stateProvince);
        }

        // GET: StateProvinces/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StateProvinces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Code,Other")] StateProvince stateProvince)
        {
            if (ModelState.IsValid)
            {
                db.StateProvinces.Add(stateProvince);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stateProvince);
        }

        // GET: StateProvinces/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StateProvince stateProvince = db.StateProvinces.Find(id);
            if (stateProvince == null)
            {
                return HttpNotFound();
            }
            return View(stateProvince);
        }

        // POST: StateProvinces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Code,Other")] StateProvince stateProvince)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stateProvince).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stateProvince);
        }

        // GET: StateProvinces/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StateProvince stateProvince = db.StateProvinces.Find(id);
            if (stateProvince == null)
            {
                return HttpNotFound();
            }
            return View(stateProvince);
        }

        // POST: StateProvinces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StateProvince stateProvince = db.StateProvinces.Find(id);
            db.StateProvinces.Remove(stateProvince);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
