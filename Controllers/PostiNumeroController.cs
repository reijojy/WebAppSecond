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
    public class PostiNumeroController : Controller
    {
        TilausDBEntities db = new TilausDBEntities();
        // GET: PostiNumero
        public ActionResult Index()
        {
            if (Session["LoggedStatus"] != null && Session["LoggedStatus"] == "In")
            {
                ViewBag.LoggedText = "Kirjautunut sisään: " + Session["UserName"] + " " + Session["LoggetTime"];
            }
            else
            {
                ViewBag.LoggedText = "Et ole kirjautunut sisään";
                return RedirectToAction("login", "home");
            }
            List<Postitoimipaikat> model = db.Postitoimipaikats.ToList();
            return View(model);
        }
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Postitoimipaikat postiToimiPaikka = db.Postitoimipaikats.Find(id);
            if (postiToimiPaikka == null) return HttpNotFound();
            return View(postiToimiPaikka);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Katso https://go.microsoft.com/fwlink/?LinkId=317598
        public ActionResult Edit([Bind(Include = "Postinumero,Postitoimipaikka")] Postitoimipaikat postiToimiPaikat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postiToimiPaikat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(postiToimiPaikat);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Postinumero,Postitoimipaikka")] Postitoimipaikat postiToimiPaikat)
        {
            if (ModelState.IsValid)
            {
                db.Postitoimipaikats.Add(postiToimiPaikat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(postiToimiPaikat);
        }
        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Postitoimipaikat postiToimiPaikka = db.Postitoimipaikats.Find(id);
            if (postiToimiPaikka == null) return HttpNotFound();
            return View(postiToimiPaikka);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ViewBag.RemoveFailed = "";
            Boolean isRemoved = true;
            Postitoimipaikat postiToimiPaikka = db.Postitoimipaikats.Find(id);
           
            try
            {
                db.Postitoimipaikats.Remove(postiToimiPaikka);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                ViewBag.RemoveFailed = "Postinumeron poisto ei onnistunut";
            return View(postiToimiPaikka);
        }
    }
}