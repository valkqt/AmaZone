using AmaZone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AmaZone.Controllers
{

    public class LoginController : Controller
    {
        private string connectionString = "Server=Mephisto\\SQLEXPRESS; Initial Catalog=Amazone; Integrated Security=true; TrustServerCertificate=True";

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User user)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand select = new SqlCommand($"select * from Users where username='{user.username}' and password='{user.password}'", con);
                SqlDataReader reader = select.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var authTicket = new FormsAuthenticationTicket(1, reader["username"].ToString(), DateTime.Now, DateTime.Now.AddMinutes(15), true, user.ToRoleString(int.Parse(reader["role"].ToString())));
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                        HttpCookie cookie = new HttpCookie("user", reader["id"].ToString());

                        Response.Cookies.Add(cookie);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                con.Close();
            }



            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Request.Cookies.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}