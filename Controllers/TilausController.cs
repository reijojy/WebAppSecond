using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppSecond.Models;

namespace WebAppSecond.Controllers
{
    public class TilausController : Controller
    {
        TilausDBEntities db = new TilausDBEntities();
        // GET: Tilaus
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
            List<Tilaukset> model = db.Tilauksets.ToList();
            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tilaukset tilaus = db.Tilauksets.Find(id);
            if (tilaus == null) return HttpNotFound();
            //ViewBag.tunniste = tilaus.AsiakasID;
            ViewBag.AsiakasID = new SelectList(db.Asiakkaats, "AsiakasID", "Nimi", tilaus.AsiakasID);

            List<SelectListItem> postiToimiPaikat_sl = new List<SelectListItem>();
            foreach (Postitoimipaikat postitoimipaikka in db.Postitoimipaikats)
            {
                postiToimiPaikat_sl.Add(new SelectListItem
                {
                    Value = postitoimipaikka.Postinumero,
                    Text = postitoimipaikka.Postinumero + " " + postitoimipaikka.Postitoimipaikka
                });
            }
            ViewBag.Postinumero = new SelectList(postiToimiPaikat_sl, "Value", "Text", tilaus.Postinumero);
            //ViewBag.Postinumero = new SelectList(db.Postitoimipaikats, "Postinumero", "Postitoimipaikka", tilaus.Postinumero);
            return View(tilaus);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Katso https://go.microsoft.com/fwlink/?LinkId=317598
        public ActionResult Edit([Bind(Include = "tilausID,AsiakasID,Toimitusosoite,Postinumero,Tilauspvm,Toimituspvm")] Tilaukset tilaus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tilaus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tilaus);
        }
        public ActionResult Create()
        {
            ViewBag.AsiakasID = new SelectList(db.Asiakkaats, "AsiakasID", "Nimi");

            // Postitoimipaikkojen selectbox
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
            //ViewBag.Postinumero = new SelectList(db.Postitoimipaikats, "Postinumero", "Postitoimipaikka");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TilausID,ASiakasID,ToimitusOsoite,Postinumero,Tilauspvm,ToimitusPvm")] Tilaukset tilaus)
        {
            if (ModelState.IsValid)
            {
                db.Tilauksets.Add(tilaus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tilaus);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tilaukset tilaus = db.Tilauksets.Find(id);
            if (tilaus == null) return HttpNotFound();
            return View(tilaus);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.RemoveFailed = "";
            Boolean isRemoved = true;
            Tilaukset tilaus = db.Tilauksets.Find(id);
            // Tilauksella saattaa olla tuoterivejä jolloin poisto 
            // ei onnistu
            try
            {
                db.Tilauksets.Remove(tilaus);
                db.SaveChanges();
            } catch(DbUpdateException e)
            {
                isRemoved = false;
            } catch(SqlException se)
            {
                isRemoved = false;
            }

            if (isRemoved)
                return RedirectToAction("Index");
            else 
                ViewBag.RemoveFailed = "Tilauksen poisto ei onnistunut";
            return View(tilaus);
        }
    }
}