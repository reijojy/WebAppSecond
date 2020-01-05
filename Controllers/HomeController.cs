using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppSecond.Models;

namespace WebAppSecond.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            setLogInStatus();
            return View();
        }

        public ActionResult About()
        {
            setLogInStatus();
            ViewBag.Message = "Applikaation tiedot.";
            return View();
        }

        public ActionResult Contact()
        {
            setLogInStatus();
            ViewBag.Message = "Yhteystiedot.";

            return View();
        }
        public ActionResult Login()
        {
            setLogInStatus();
            return View();
        }
        [HttpPost]
        public ActionResult Authorize(Login LoginModel)
        {
            TilausDBEntities db = new TilausDBEntities();
            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä 
            var LoggedUser = db.Logins.SingleOrDefault(x => x.UserName == LoginModel.UserName && x.PassWord == LoginModel.PassWord);
            if (LoggedUser != null)
            {
                ViewBag.LoginMessage = "Successfull login";
                ViewBag.LoggedStatus = "In";
                Session["UserName"] = LoggedUser.UserName;
                DateTime justNow = DateTime.Now;
                Session["LoggetTime"] = justNow.ToString("dd.MM.yy HH:mm:ss"); 
                Session["LoggedStatus"] = "In";
                //return View("Login", LoginModel);
                return RedirectToAction("Index", "Home"); //Tässä määritellään mihin onnistunut kirjautuminen johtaa --> Home/Index 
            }
            else
            {
                ViewBag.LoginMessage = "Login unsuccessfull";
                ViewBag.LoggedStatus = "Out";
                Session["LoggedStatus"] = "Out";
                Session["UserName"] = null;
                LoginModel.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Login", LoginModel);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            ViewBag.LoggedStatus = "Out";
                        return RedirectToAction("Index", "Home"); //Uloskirjautumisen jälkeen pääsivulle
        }
        private void setLogInStatus()
        {
            if (Session["LoggedStatus"] != null && Session["LoggedStatus"] == "In")
            {
                ViewBag.LoggedText = "Kirjautunut sisään: " + Session["UserName"] + " " + Session["LoggetTime"];
            }
            else ViewBag.LoggedText = "Et ole kirjautunut sisään";
        }
    }
}