using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AmaZone.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var authTicket = new FormsAuthenticationTicket(
                1, "mario", DateTime.Now, DateTime.Now.AddMinutes(15), true, "admin");
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
            return View();
        }

        [Authorize (Roles="admin") ]
        public ActionResult Riservato()
        {
            return View();
        }

    }
}