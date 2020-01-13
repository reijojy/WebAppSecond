using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppSecond.Models;

namespace WebAppSecond.Controllers
{
    public class TilausRiviController : Controller
    {
        TilausDBEntities db = new TilausDBEntities();
        // GET: TilausRivi
        public ActionResult Index(int? id)
        {
            if (!setLogInStatus()) return RedirectToAction("login", "home");
            
            if (id == null)
            {
                ViewBag.TilausId = Session["Tilaus"];
                ViewBag.AsiakasId = Session["Asiakas"];
                id = ViewBag.TilausId;
            } else { 
                Tilaukset tilaus = db.Tilauksets.Find(id);
                Session["Tilaus"] = id;
                Session["Asiakas"] = tilaus.AsiakasID;
                ViewBag.TilausId = id;
                ViewBag.AsiakasId = tilaus.AsiakasID;
            }

            // Tehdääs LINQ jolla saadaan rivit
            IEnumerable<Tilausrivit> model = from tr in db.Tilausrivits where tr.TilausID == id select tr;
            // Lasketaan rivien määrä ja yhteissumma
            int riviLkm = riviLkm = model.Count();  
            decimal tilausSumma = 0.00m;
            foreach(Tilausrivit tilausrivi in model)
            {
                decimal d = Convert.ToDecimal(tilausrivi.Ahinta);
                double dMaara = Convert.ToDouble(tilausrivi.Maara);
                tilausSumma += d * Convert.ToDecimal(dMaara);
            }
            CultureInfo ci = new CultureInfo("fi-FI");
            ci.NumberFormat.NumberDecimalDigits = 2;
            ci.NumberFormat.CurrencySymbol = "EUR";
            ViewBag.RiviLkm = riviLkm.ToString(ci);
            ViewBag.TilausSumma = tilausSumma.ToString("C", ci);
            return View(model);
        }

        // GET: TilausRivi/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TilausRivi/Create
        public ActionResult Create()
        {
            ViewBag.TuoteID = new SelectList(db.Tuotteets, "TuoteID", "Nimi");
            ViewBag.TilausId = Session["Tilaus"];
            ViewBag.AsiakasId = Session["Asiakas"];
            setLogInStatus();
            //Tilausrivit tilausrivi = new Tilausrivit();
            //tilausrivi.TilausID = ViewBag.TilausId;
            return View();
        }

        // POST: TilausRivi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create([Bind(Include = "TilausRiviID,TuoteID,Maara,Ahinta,")] Tilausrivit tilausrivi)
        {
            if (ModelState.IsValid)
            {
                ViewBag.TilausID = Session["Tilaus"];
                tilausrivi.TilausID = ViewBag.TilausID;
                db.Tilausrivits.Add(tilausrivi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
                return View(tilausrivi);
        }

        // GET: TilausRivi/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tilausrivit tilausRivi = db.Tilausrivits.Find(id);
            if (tilausRivi == null) return HttpNotFound();
            setLogInStatus();
            // Combobox tuotteelle
            ViewBag.TuoteID = new SelectList(db.Tuotteets, "TuoteID", "Nimi", tilausRivi.TuoteID);
            return View(tilausRivi);
        }

        // POST: TilausRivi/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "TilausRiviID,TuoteID,Maara,Ahinta,")] Tilausrivit tilausRivi)
        {
            if (ModelState.IsValid)
            {
                ViewBag.TilausID = Session["Tilaus"];
                tilausRivi.TilausID = ViewBag.TilausID;
                db.Entry(tilausRivi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tilausRivi);
        }

        // GET: TilausRivi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Tilausrivit tilausRivi = db.Tilausrivits.Find(id);
            if (tilausRivi == null) return HttpNotFound();
            setLogInStatus();
            return View(tilausRivi);
         }

        // POST: TilausRivi/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Tilausrivit tilausRivi = db.Tilausrivits.Find(id);
            db.Tilausrivits.Remove(tilausRivi);
            db.SaveChanges();
            return RedirectToAction("Index");
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
