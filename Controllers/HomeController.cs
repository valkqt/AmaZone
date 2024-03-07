using AmaZone.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AmaZone.Controllers
{
    public class HomeController : Controller
    {
        private string connectionString = "Server=Mephisto\\SQLEXPRESS; Initial Catalog=Amazone; Integrated Security=true; TrustServerCertificate=True";
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Riservato()
        {
            return RedirectToAction("Add", "Orders");
        }



        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register([Bind(Include = "username,password, name, UserCode")] User user)
        {
            SqlConnection con = new SqlConnection(connectionString);
            if (ModelState.IsValid) {
                try
                {
                    con.Open();

                    SqlCommand insert = new SqlCommand("insert into Users (username, password, name, UserCode, role)" +
                        " values (@username, @password, @name, @UserCode, @role)", con);
                    insert.Parameters.AddWithValue("@username", user.username);
                    insert.Parameters.AddWithValue("@password", user.password);
                    insert.Parameters.AddWithValue("@name", user.name);
                    insert.Parameters.AddWithValue("@UserCode", user.UserCode);
                    insert.Parameters.AddWithValue("@role", user.ParseRole(Role.User));
                    insert.ExecuteNonQuery();

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
                    return RedirectToAction("Index", "Home");


                }
                catch (Exception ex)
                {
                    TempData["pepe"] = ex.Message;

                    Response.Write(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }

            return View(user);
        }


    }
}