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
    public class AsiakasController : Controller
    {
        TilausDBEntities db = new TilausDBEntities();
        // GET: Asiakas
        public ActionResult Index()
        {
            if (!setLogInStatus()) return RedirectToAction("login", "home");
                    
            List<Asiakkaat> model = db.Asiakkaats.ToList();
            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Asiakkaat asiakas = db.Asiakkaats.Find(id);
            if (asiakas == null) return HttpNotFound();
            setLogInStatus();
            List<SelectListItem> postiToimiPaikat_sl = new List<SelectListItem>();
            foreach (Postitoimipaikat postitoimipaikka in db.Postitoimipaikats)
            {
                postiToimiPaikat_sl.Add(new SelectListItem
                {
                    Value = postitoimipaikka.Postinumero,
                    Text = postitoimipaikka.Postinumero + " " + postitoimipaikka.Postitoimipaikka
                });
            }
            //ViewBag.Postinumero = new SelectList(db.Postitoimipaikats, "Postinumero", "Postitoimipaikka", asiakas.Postinumero);
            ViewBag.Postinumero = new SelectList(postiToimiPaikat_sl, "Value", "Text", asiakas.Postinumero);
            return View(asiakas);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Katso https://go.microsoft.com/fwlink/?LinkId=317598
        public ActionResult Edit([Bind(Include = "AsiakasID,Nimi,Osoite,Postinumero")] Asiakkaat asiakas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asiakas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asiakas);
        }
        public ActionResult Create()
        {
            setLogInStatus();
            // Luodaan combobox postinumeroille
            List<SelectListItem> postiToimiPaikat_sl = new List<SelectListItem>();

            foreach (Postitoimipaikat postitoimipaikka in db.Postitoimipaikats)
            {
                postiToimiPaikat_sl.Add(new SelectListItem
                {
                    Value = postitoimipaikka.Postinumero,
                    Text = postitoimipaikka.Postinumero + " " + postitoimipaikka.Postitoimipaikka
                });
            }
            ViewBag.Postinumero = new SelectList(postiToimiPaikat_sl, "Value", "Text");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsiakasID,Nimi,Osoite,Postinumero")] Asiakkaat asiakas)
        {
            if (ModelState.IsValid)
            {
                db.Asiakkaats.Add(asiakas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asiakas);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Asiakkaat asiakas = db.Asiakkaats.Find(id);
            if (asiakas == null) return HttpNotFound();
            setLogInStatus();
            return View(asiakas);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.RemoveFailed = "";
            Boolean isRemoved = true;

            Asiakkaat asiakas = db.Asiakkaats.Find(id);
            try
            {
                db.Asiakkaats.Remove(asiakas);
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
                ViewBag.RemoveFailed = "Asiakkaan poisto ei onnistunut";
            return View(asiakas);
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