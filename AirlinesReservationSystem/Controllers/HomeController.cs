using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Models.Form;
using AirlinesReservationSystem.Models;
using AirlinesReservationSystem.Helper;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Net;
using PayPal.Api;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirlinesReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private Model1 db = new Model1();
        private readonly string apiUrl = "https://localhost:44371/api/";
        Uri baseAddress = new Uri("https://localhost:44371/api/");
        private readonly HttpClient _client;
        public HomeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;

        }


        [HttpGet]
        //[ValidateAntiForgeryToken]

        //Hàm này trả về một PartialView và gán giá trị "hai" cho biến ViewBag.name.
        public ActionResult View()
        {
            ViewBag.name = "hai";
            return PartialView();
        }
        //Hàm này xử lý yêu cầu tìm kiếm vé máy bay. Nó hiển thị danh sách sân bay xuất phát và đến thông qua ViewBag, sau đó kiểm tra nếu yêu cầu có dữ liệu và ModelState hợp lệ. Nếu hợp lệ, nó kiểm tra các điều kiện về điểm đến và điểm xuất phát, sau đó truy vấn cơ sở dữ liệu để lấy danh sách các chuyến bay phù hợp và trả về chúng dưới dạng View. Nếu không, nó trả về View với _orderTicketForm.
        [HttpGet]
        public async Task<ActionResult> Index(OrderTicketForm _orderTicketForm)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            response["status"] = "200";
            response["message"] = "";
            _orderTicketForm.repartureDate = "%20" + _orderTicketForm.repartureDate;
            ViewBag.from = new SelectList(db.AirPorts, "id", "code");
            ViewBag.to = new SelectList(db.AirPorts, "id", "code");
            ViewBag.flightSchedule = null;
            ViewBag.title = "Search Ticket";
            if (Request.QueryString.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    // Đây là cờ báo lỗi
                    bool flagError = false;
                    // Điểm đến trùng điểm đi
                    if (!_orderTicketForm.checkDestination())
                    {
                        ModelState.AddModelError("to", "The destination must be different from the point of departure");
                        flagError = true;
                    }

                    if (flagError == false)
                    {
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(apiUrl);

                                var responses = await client.GetAsync($"FlightSchedules/getScheduleOrder/{_orderTicketForm.from},{_orderTicketForm.to},{ _orderTicketForm.repartureDate.Trim()},{"%20%20"}");

                                if (responses.IsSuccessStatusCode)
                                {
                                    var content = await responses.Content.ReadAsStringAsync();
                                    List<FlightSchedule> flightSchedules = JsonConvert.DeserializeObject<List<FlightSchedule>>(content);
                                    ViewBag.flightSchedule = flightSchedules;
                                    return View(_orderTicketForm);
                                    //AuthHelper.setIdentity(user);
                                    //AlertHelper.setToast("success", "Đăng nhập thành công.");
                                    //return View("UserProfile", user);
                                }
                                else
                                {
                                    response["status"] = "400";
                                    response["message"] = "Lỗi xảy ra.";
                                    //ViewBag.ErrorMessage = "Invalid login credentials.";

                                }
                            }
                            return Content(JsonConvert.SerializeObject(response));
                        }
                        catch (Exception ex)
                        {
                            response["status"] = "400";
                            response["message"] = "Lỗi xảy ra.";
                        }




                        //Lấy danh sach chuyến bay phù hợp vs thời gian.
                        //var query = db.FlightSchedules.Where(s => s.to_airport == _orderTicketForm.to && s.from_airport == _orderTicketForm.from);
                        //DateTime repartureDate = DateTime.Parse(_orderTicketForm.repartureDate);
                        //query = query.Where(s => EntityFunctions.TruncateTime(s.departures_at) == EntityFunctions.TruncateTime(repartureDate));
                        //List<FlightSchedule> models = query.ToList();
                        //ViewBag.flightSchedule = models;
                        //return View(_orderTicketForm);
                    }

                }
            }
            else
            {
                ModelState.Clear();
            }
            return View(_orderTicketForm);
        }

        //Hàm này trả về thông tin chi tiết của một chuyến bay cụ thể dựa trên ID của chuyến bay.
        [HttpGet]
        public async Task<ActionResult> DetailFlightSchedule(int id)
        {
            FlightSchedule flightSchedules = db.FlightSchedules.Where(s => s.id == id).FirstOrDefault();
            FlightSchedule flightSchedule = new FlightSchedule();
            Dictionary<string, string> response = new Dictionary<string, string>();
            response["status"] = "200";
            response["message"] = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);


                    var responses = client.GetAsync($"FlightSchedules/DetailFlightSchedule/{id}").Result;

                    if (responses.IsSuccessStatusCode)
                    {
                        var content = await responses.Content.ReadAsStringAsync();
                        flightSchedule = JsonConvert.DeserializeObject<FlightSchedule>(content);
                        List<Seats> seats = db.Seats.Where(s => s.flight_schedules_id == id).ToList();
                        ViewData["seatData"] = seats;
                        return PartialView(flightSchedule);
                        //List<FlightSchedule> flightSchedules = JsonConvert.DeserializeObject<List<FlightSchedule>>(content);

                    }
                    else
                    {

                        response["status"] = "400";
                        response["message"] = "Lỗi mất kết nối với dữ liệu.";
                        return Content(JsonConvert.SerializeObject(response));
                        //ViewBag.ErrorMessage = "Invalid login credentials.";

                    }
                }
                return Content(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response["status"] = "400";
                response["message"] = "Lỗi mất kết nối với dữ liệu.";
                return Content(JsonConvert.SerializeObject(response));

            }


            if (flightSchedule == null)
            {
                response["status"] = "400";
                response["message"] = "Lỗi mất kết nối với dữ liệu.";
                return Content(JsonConvert.SerializeObject(response));
            }

            return PartialView(flightSchedule);
        }
        [HttpPost]
        public ActionResult Pays(string ticketID, int flight, int amount, String seats)
        {

            List<TicketManager> lstTicket = new List<TicketManager>();
            FlightSchedule flights = db.FlightSchedules.FirstOrDefault(x => x.id == flight);
            int amountTicket = Int32.Parse(flights.cost.ToString()) * amount;
            int amountTicketSingle = Int32.Parse(flights.cost.ToString());
            string[] seatArray = seats.Split(',');
            bool checkticket = CheckLocalSeats(flight, seats);

            Dictionary<string, string> response = new Dictionary<string, string>();
            response["status"] = "200";
            response["message"] = "";
            if (!AuthHelper.isLogin())
            {
                response["status"] = "400";
                response["message"] = "Phải đăng nhập mới có thể mua được vé.";
                return Content(JsonConvert.SerializeObject(response));
            }
            if (amount == 0)
            {
                response["status"] = "400";
                response["message"] = "Bạn phải mua ít nhất 1 vé.";
                return Content(JsonConvert.SerializeObject(response));
            }
            if (checkticket == false)
            {
                response["status"] = "400";
                response["message"] = "Chỗ ngồi bạn đặt đã có người đặt rồi.";
                return Content(JsonConvert.SerializeObject(response));
            }


            for (int i = 0; i < amount; i++)
            {
                TicketManager ticket = new TicketManager();
                ticket.user_id = AuthHelper.getIdentity().id;
                ticket.flight_schedules_id = flight;
                ticket.status = TicketManager.STATUS_PAY;
                ticket.code = ticketID + "" + i.ToString();
                ticket.seat_location = Int32.Parse(seatArray[i]);

                lstTicket.Add(ticket);

                if (ModelState.IsValid)
                {
                    //db.TicketManagers.Add(ticket);
                    //db.SaveChanges();
                }
                else
                {
                    AlertHelper.setToast("danger", "Đặt vé không thành công.");
                }
            }

            Session["lstTicket"] = lstTicket;
            Session["amountTicket"] = amountTicket;
            Session["amountTicketSingle"] = amountTicketSingle;
            AlertHelper.setToast("success", "Bạn đang đến thanh toán.");
            return Content(JsonConvert.SerializeObject(response));
        }





        //Hàm này xử lý thanh toán vé máy bay. Nó kiểm tra xem người dùng đã đăng nhập chưa. Nếu đã đăng nhập, nó tạo một hoặc nhiều vé máy bay dựa trên thông tin được cung cấp và lưu chúng vào cơ sở dữ liệu. Sau đó, nó trả về một thông báo JSON với trạng thái thanh toán.
        //[HttpGet]
        //public ActionResult PayTicket(string ticketID,int flightScheduleID,int amount =1  )
        //{
        //    Dictionary<string, string> response = new Dictionary<string, string>();
        //    response["status"] = "200";
        //    response["message"] = "";
        //    if (!AuthHelper.isLogin())
        //    {
        //        response["status"] = "400";
        //        response["message"] = "Phải đăng nhập mới có thể mua được vé.";
        //        return Content(JsonConvert.SerializeObject(response));
        //    }
        //    for (int i = 0; i < amount; i++)
        //    {
        //        TicketManager ticket = new TicketManager();
        //        ticket.user_id = AuthHelper.getIdentity().id;
        //        ticket.flight_schedules_id = flightScheduleID;
        //        ticket.status = TicketManager.STATUS_PAY;
        //        ticket.code = ticketID+""+i.ToString();
        //        if (ModelState.IsValid)
        //        {
        //            db.TicketManagers.Add(ticket);
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            AlertHelper.setToast("danger", "Đặt vé không thành công.");
        //        }
        //    }

        //    AlertHelper.setToast("success", "Đặt vé thành công.");
        //    return Content(JsonConvert.SerializeObject(response));
        //}
        //Hàm này hiển thị danh sách vé máy bay của người dùng hiện tại.

        public ActionResult PayYourTicket()
        {
            if (!AuthHelper.isLogin())
            {
                return RedirectToAction("Index");
            }
            User user = AuthHelper.getIdentity();
            IEnumerable<TicketManager> ticketManagers = Session["lstTicket"] as IEnumerable<TicketManager>;
            Session["lstTicket"] = ticketManagers;

            return View(ticketManagers);
        }
        public ActionResult YourTicket()
        {
            if (!AuthHelper.isLogin())
            {
                return RedirectToAction("Index");
            }
            User user = AuthHelper.getIdentity();
            IEnumerable<TicketManager> ticketManagers = db.TicketManagers.Where(s => s.user_id == user.id).ToList();
            return View(ticketManagers);
        }
        // Hàm này trả về thông tin chi tiết của một vé máy bay cụ thể dựa trên ID của vé.
        public ActionResult DetailTicket(int id)
        {
            TicketManager ticket = db.TicketManagers.Where(s => s.id == id).FirstOrDefault();
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return PartialView(ticket);
        }
        //Hàm này hủy một vé máy bay dựa trên ID của vé.
        public ActionResult CancelTicket(int id)
        {
            TicketManager ticket = db.TicketManagers.Where(s => s.id == id).FirstOrDefault();
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ticket.status = TicketManager.STATUS_CANCEL;
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                AlertHelper.setToast("warning", "Hủy vé thành công.");
            }
            return RedirectToAction("YourTicket", "Home");
        }
        // Hàm này trả về một View để chỉnh sửa thông tin người dùng dựa trên ID được cung cấp.
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //Hàm này xử lý yêu cầu chỉnh sửa thông tin người dùng. Nó kiểm tra ModelState, sau đó cập nhật thông tin người dùng trong cơ sở dữ liệu và chuyển hướng đến trang chỉnh sửa người dùng.
        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "id,name,email,cccd,address,phone_number,password,user_type")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                AlertHelper.setToast("success", "Cập nhập thông tin khách hàng thành công.");
                return RedirectToAction("EditUser");
            }
            return View(user);
        }

        // Hàm này xử lý yêu cầu thay đổi mật khẩu của người dùng. Nó kiểm tra mật khẩu cũ, sau đó cập nhật mật khẩu mới trong cơ sở dữ liệu và trả về một thông báo JSON với kết quả.
        public ActionResult ChangePassword(string old_password, string new_password)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            response["status"] = "200";
            response["message"] = "";
            User identity = AuthHelper.getIdentity();
            if (identity.password != old_password)
            {
                response["status"] = "400";
                response["message"] = "Sai thông tin mật khẩu cũ.";
                return Content(JsonConvert.SerializeObject(response));
            }
            User user = db.Users.Find(identity.id);
            if (user != null)
            {
                user.password = new_password;
                db.SaveChanges();
                response["status"] = "200";
                response["message"] = "Đổi mật khẩu thành công.";
                AlertHelper.setToast("success", "Đổi mật khẩu thành công");
            }
            return Content(JsonConvert.SerializeObject(response));
        }
        /// PAYPAL
        /// 
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            Payments payments = new Payments();
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Home/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                    //create payments 
                    payments.email_Payment = executedPayment.payer.payer_info.email;
                    payments.name_Payment = executedPayment.payer.payer_info.first_name + " " + executedPayment.payer.payer_info.last_name;
                    payments.PayerID_Payment = executedPayment.transactions[0].related_resources[0].sale.id;
                    payments.UserID = AuthHelper.getIdentity().id;

                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            List<TicketManager> itemsTicket = Session["lstTicket"] as List<TicketManager>;

            db.Payments.Add(payments);
            db.SaveChanges();

            Payments payments1 = db.Payments.FirstOrDefault(x => x.email_Payment == payments.email_Payment && x.PayerID_Payment == payments.PayerID_Payment);




            foreach (var item in itemsTicket)
            {
                item.pay_id = payments1.id;
                db.TicketManagers.Add(item);
                //updated seat 
                Seats seat = db.Seats.FirstOrDefault(x => (x.flight_schedules_id == item.flight_schedules_id && x.seat == item.seat_location.ToString()));
                seat.isbooked = 1;
                //update booked
                FlightSchedule flight = db.FlightSchedules.FirstOrDefault(x =>  x.id == item.flight_schedules_id);
                flight.bookedSeats += 1;

                db.Entry(seat).State = EntityState.Modified;
                db.Entry(flight).State = EntityState.Modified;
            }

            db.SaveChanges();

            AlertHelper.setToast("success", "Đặt vé thành công.");
            //    return Content(JsonConvert.SerializeObject(response));

            return View("SuccessView");
        }
        public ActionResult SuccessView()
        {
            return View();
        }
        public ActionResult FailureView()
        {
            return View();
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            List<TicketManager> itemsTicket = Session["lstTicket"] as List<TicketManager>;

            Session["lstTicket"] = itemsTicket;

            int price = (int)Session["amountTicket"];
            int priceSingle = (int)Session["amountTicketSingle"];

            double convertUSD = Math.Round((double)price / 25380, 2);
            double convertUSDSingle = Math.Round((double)priceSingle / 25380, 2);


            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  

            foreach (var item in itemsTicket)
            {
                itemList.items.Add(new Item()
                {
                    name = "don ve may bay " + item.code,
                    currency = "USD",
                    price = convertUSDSingle.ToString(),
                    quantity = "1",
                    sku = "sku"
                });
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = convertUSD.ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = convertUSD.ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);

            //List<TicketManager> itemsTicket = Session["lstTicket"] as List<TicketManager>;
            //int price = (int)Session["amountTicket"];
            //int priceSingle = (int)Session["amountTicketSingle"];

            //double convertUSD = Math.Round( (double)price / 25380, 2);
            //double convertUSDSingle = Math.Round((double)priceSingle / 25380, 2);
            ////create itemlist and add item objects to it  
            //var itemList = new ItemList()
            //{
            //    items = new List<Item>()
            //};
            ////Adding Item Details like name, currency, price etc  
            //foreach (var item in itemsTicket)
            //{
            //    itemList.items.Add(new Item()
            //    {
            //        name = item.seat_location.ToString(),
            //        currency = "USD",
            //        price = convertUSDSingle.ToString(CultureInfo.InvariantCulture),
            //        quantity = "1",
            //        sku = "sku"
            //    });
            //}
            //var payer = new Payer()
            //{
            //    payment_method = "paypal"
            //};
            //// Configure Redirect Urls here with RedirectUrls object  
            //var redirUrls = new RedirectUrls()
            //{
            //    cancel_url = redirectUrl + "&Cancel=true",
            //    return_url = redirectUrl
            //};
            //// Adding Tax, shipping and Subtotal details  
            //var details = new Details()
            //{
            //    tax = "0",
            //    shipping = "0",
            //    subtotal = convertUSD.ToString(CultureInfo.InvariantCulture),
            //};
            ////Final amount with details  
            //var amount = new Amount()
            //{
            //    currency = "USD",
            //    total = convertUSD.ToString(CultureInfo.InvariantCulture), // Total must be equal to sum of tax, shipping and subtotal.  
            //    details = details
            //};
            //var transactionList = new List<Transaction>();
            //// Adding description about the transaction  
            //var paypalOrderId = DateTime.Now.Ticks;
            //transactionList.Add(new Transaction()
            //{
            //    description = $"Invoice #{paypalOrderId}",
            //    invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
            //    amount = amount,
            //    item_list = itemList
            //});
            //this.payment = new Payment()
            //{
            //    intent = "sale",
            //    payer = payer,
            //    transactions = transactionList,
            //    redirect_urls = redirUrls
            //};
            //// Create a payment using a APIContext  
            //return this.payment.Create(apiContext);
        }

        public bool CheckLocalSeats(int flightschedule, String seats)
        {
            if (seats == null || seats == "")
            {
                return false;
            }
            string[] seatArray = seats.Split(',');
            bool checkitem = true;
            foreach (var item in seatArray)
            {
                int itemNumber = Int32.Parse(item.ToString());

                TicketManager check = db.TicketManagers.FirstOrDefault(x => x.flight_schedules_id == flightschedule && x.seat_location == itemNumber);
                if (check != null)
                {
                    checkitem = false;
                    break;
                }
            }
            return checkitem;
        }


        public ActionResult About()
        {
            return View();
        }

        public ActionResult Deals()
        {
            return View();
        }
    }
}