using AmaZone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AmaZone.Controllers
{
    public class OrdersController : Controller
    {
        private string connectionString = "Server=Mephisto\\SQLEXPRESS; Initial Catalog=Amazone; Integrated Security=true; TrustServerCertificate=True";

        [Authorize(Roles = "admin")]
        public ActionResult All()
        {
            SqlConnection con = new SqlConnection(connectionString);
            List<Order> orders = new List<Order>();

            try
            {
                con.Open();
                SqlCommand select = new SqlCommand("select Users.name, Orders.* from (Orders " +
                    "inner join Users on Orders.client = Users.id) inner join OrdersState on Orders.id = OrdersState.idString ", con);
                SqlDataReader reader = select.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Order order = new Order();
                        order.idString = reader["id"].ToString();
                        order.client = reader["name"].ToString();
                        order.address = reader["address"].ToString();
                        order.arrivalDate = (DateTime)reader["arrivalDate"];
                        order.recipientName = reader["recipientName"].ToString();
                        order.destination = reader["destination"].ToString();
                        order.freight = double.Parse(reader["freight"].ToString());
                        order.shippingDate = (DateTime)reader["shippingDate"];
                        order.state = reader["state"].ToString();
                        if (!orders.Exists(elem => elem.idString == order.idString))
                        {
                            orders.Add(order);

                        }
                    }
                    return View(orders);

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
            return View(orders);

        }

        [Authorize(Roles = "user")]
        public ActionResult Me()
        {
            SqlConnection con = new SqlConnection(connectionString);
            List<Order> orders = new List<Order>();

            try
            {
                if (Request.Cookies["user"] != null)
                {
                    string name = Request.Cookies["user"].Value;
                    con.Open();
                    SqlCommand select = new SqlCommand("select Users.name, Orders.* from (Orders " +
                        "inner join Users on Orders.client = Users.id) inner join OrdersState on Orders.id = OrdersState.idString " +
                        $"where Orders.client = '{name}'", con);
                    SqlDataReader reader = select.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Order order = new Order();
                            order.idString = reader["id"].ToString();
                            order.client = reader["name"].ToString();
                            order.address = reader["address"].ToString();
                            order.arrivalDate = (DateTime)reader["arrivalDate"];
                            order.recipientName = reader["recipientName"].ToString();
                            order.destination = reader["destination"].ToString();
                            order.freight = double.Parse(reader["freight"].ToString());
                            order.shippingDate = (DateTime)reader["shippingDate"];
                            order.state = reader["state"].ToString();
                            if (!orders.Exists(elem => elem.idString == order.idString))
                            {
                                orders.Add(order);

                            }
                        }
                        return View(orders);
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
            return View(orders);
        }


        [HttpPost]
        public ActionResult Details([Bind(Include = "idString, state, location, description")] OrderState state)
        {
            SqlConnection con = new SqlConnection(connectionString);


            if (ModelState.IsValid)
            {
                try
                {
                    con.Open();

                    {

                        SqlCommand insert2 = new SqlCommand("insert into OrdersState " +
                            "(idString, state, location, description, timestamp) " +
                            $"values (@id, @state, @location, @description, @timestamp)", con);
                        insert2.Parameters.AddWithValue("@id", state.idString);

                        insert2.Parameters.AddWithValue("@state", state.state);
                        if (state.description == null)
                        {
                            state.description = "";
                        }
                        insert2.Parameters.AddWithValue("@description", state.description);
                        insert2.Parameters.AddWithValue("@location", state.location);
                        insert2.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString() + ":000");
                        insert2.ExecuteNonQuery();

                        SqlCommand update = new SqlCommand($"update Orders set state = '{state.state}' where id = '{state.idString}'", con);
                        update.ExecuteNonQuery();

                        return RedirectToAction("Details", "Orders");
                    }



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


            return RedirectToAction("Details", "Orders");

        }
        public ActionResult Details(string id)
        {
            SqlConnection con = new SqlConnection(connectionString);
            OrderDetails model = new OrderDetails();
            model.state.stateList.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            model.state.stateList.Add(new SelectListItem { Text = "In transit", Value = "In transit" });



            try
            {
                con.Open();
                SqlCommand select = new SqlCommand($"select * from OrdersState " +
                    $"inner join Orders on Orders.id = OrdersState.idString " +
                    $"where idString = '{id}' ", con);
                SqlDataReader reader = select.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Order order = new Order();
                        order.idString = id;
                        //order.client = reader["name"].ToString();
                        order.address = reader["address"].ToString();
                        order.arrivalDate = (DateTime)reader["arrivalDate"];
                        order.recipientName = reader["recipientName"].ToString();
                        order.destination = reader["destination"].ToString();
                        order.freight = double.Parse(reader["freight"].ToString());
                        order.shippingDate = (DateTime)reader["shippingDate"];
                        model.order = order;

                        OrderState state = new OrderState();
                        state.state = reader["state"].ToString();
                        state.location = reader["location"].ToString();
                        state.description = reader["description"].ToString();
                        state.timestamp = (DateTime)reader["timestamp"];
                        model.history.Add(state);
                    }
                    ViewBag.id = id;
                    return View(model);

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

            return View();
        }

        [Authorize(Roles = "user")]
        public ActionResult Add()
        {
            return View();
        }

        public JsonResult GetTodaysOrders()
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            List<Order> orders = new List<Order>();

            SqlCommand select = new SqlCommand($"select * from Orders where shippingDate = '{DateTime.Now.Date.ToString("yyyy-MM-dd")}'", con);
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                Order order = new Order();
                order.idString = reader["id"].ToString();
                orders.Add(order);
            }
            con.Close();
            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "destination,address, recipientName, freight, arrivalDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(connectionString);
                try
                {
                    con.Open();
                    if (Request.Cookies["user"] != null)
                    {
                        int userid = int.Parse(Request.Cookies["user"].Value);
                        Random random = new Random();
                        double rd = 3 + random.NextDouble() * 85;
                        SqlCommand insert = new SqlCommand("insert into Orders " +
                            "(id, client, destination, address, shippingDate, recipientName, freight, arrivalDate, state)" +
                            "values (@id, @client, @destination, @address, @shippingDate, @recipientName, @freight, @arrivalDate, @state)", con);
                        string randomString = MvcApplication.CreateString(8);
                        insert.Parameters.AddWithValue("@id", randomString);
                        insert.Parameters.AddWithValue("@client", (int)userid);
                        insert.Parameters.AddWithValue("@destination", order.destination);
                        insert.Parameters.AddWithValue("@address", order.address);
                        insert.Parameters.AddWithValue("@shippingDate", DateTime.Now);
                        insert.Parameters.AddWithValue("@recipientName", order.recipientName);
                        insert.Parameters.AddWithValue("@freight", rd);
                        insert.Parameters.AddWithValue("@arrivalDate", order.arrivalDate);
                        insert.Parameters.AddWithValue("@state", "Pending Approval");

                        insert.ExecuteNonQuery();

                        SqlCommand insert2 = new SqlCommand("insert into OrdersState " +
                            "(idString, state, location, description, timestamp)" +
                            "values (@id, 'Pending Approval', @destination, '', @timestamp)", con);
                        insert2.Parameters.AddWithValue("@id", randomString);
                        insert2.Parameters.AddWithValue("@destination", order.destination);
                        insert2.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString() + ":000");
                        insert2.ExecuteNonQuery();
                        return RedirectToAction("Add", "Orders");
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                        return View(order);
                    }

                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                    TempData["pepe"] = ex.Message;


                }
                finally
                {
                    con.Close();

                }
                return RedirectToAction("Add", "Orders");
            }
            else
            {
                TempData["pepe"] = "not valid model";
                return View(order);
            }


        }
    }
}