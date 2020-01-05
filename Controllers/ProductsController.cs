using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppSecond.Models;

namespace WebAppSecond.Controllers
{
    public class ProductsController : Controller
    {
        TilausDBEntities db = new TilausDBEntities();
        // GET: Products
        public ActionResult Index()
        {
            if (!setLogInStatus()) return RedirectToAction("login", "home");
           
            List<Tuotteet> model = db.Tuotteets.ToList();
            db.Dispose();
            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tuotteet tuote = db.Tuotteets.Find(id);
            if (tuote == null) return HttpNotFound();
            setLogInStatus();
            return View(tuote);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Katso https://go.microsoft.com/fwlink/?LinkId=317598
        public ActionResult Edit([Bind(Include = "TuoteID,Nimi,Ahinta,Kuva,Kuvalinkki")] Tuotteet tuote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tuote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tuote);
        }
        public ActionResult Create()
        {
            setLogInStatus();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TuoteID,Nimi,Ahinta,Kuvalinkki")] Tuotteet tuote)
        {
            if (ModelState.IsValid)
            {
                db.Tuotteets.Add(tuote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tuote);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tuotteet tuote = db.Tuotteets.Find(id);
            if (tuote == null) return HttpNotFound();
            setLogInStatus();
            return View(tuote);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            ViewBag.RemoveFailed = "";
            Boolean isRemoved = true;

            Tuotteet tuote = db.Tuotteets.Find(id);
            try
            {
                db.Tuotteets.Remove(tuote);
                db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                isRemoved = false;
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                isRemoved = false;
            }

            if (isRemoved)
                return RedirectToAction("Index");
            else
                ViewBag.RemoveFailed = "Tuotteen poisto ei onnistunut";
            return View(tuote);
        }
        private Boolean setLogInStatus()
        {
            if (Session["LoggedStatus"] != null && Session["LoggedStatus"] == "In")
            {
                ViewBag.LoggedText = "Kirjautunut sisään: " + Session["UserName"] + " " + Session["LoggetTime"];
                return true;
            }
            else
            {
                ViewBag.LoggedText = "Et ole kirjautunut sisään";
                return false;
            }
        }
    }
}