using System.Collections.Generic;
using System.Web.Mvc;
using AirlinesReservationSystem.Helper;
using Newtonsoft.Json;
using AirlinesReservationSystem.Models;
using System.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AirlinesReservationSystem.Controllers
{
    public class SecurityController : Controller
    {
		Model1 db = new Model1();
		private readonly string apiUrl = "https://localhost:44371/api/";

		Uri baseAddress = new Uri("https://localhost:44371/api/");

		private readonly HttpClient _client;

		public SecurityController()
		{
			_client = new HttpClient();
			_client.BaseAddress = baseAddress;

		}


		public object EntityValidationErrors { get; private set; }
		// Hàm này xử lý yêu cầu đăng nhập. Nó kiểm tra thông tin đăng nhập của người dùng (email và mật khẩu) bằng cách truy vấn cơ sở dữ liệu để tìm kiếm người dùng với email tương ứng, sau đó so sánh mật khẩu. Nếu thông tin đăng nhập hợp lệ, nó đặt thông tin xác thực của người dùng vào phiên làm việc và trả về một thông báo JSON cho biết đăng nhập thành công.
		public ActionResult Login(string email = "", string password = "")
		{

			Dictionary<string, string> response = new Dictionary<string, string>();
			response["status"] = "200";
			response["message"] = "";
			User user = Conection.getDb().Users.Where(s => s.email == email).FirstOrDefault();
			if (user == null || user.password != password)
			{
				response["status"] = "400";
				response["message"] = "Thông tin tài khoản không hợp lệ.";
			}
			else
			{

				AuthHelper.setIdentity(user);
				AlertHelper.setToast("success", "Đăng nhập thành công.");
			}
			return Content(JsonConvert.SerializeObject(response));
		}

		// Đăng nhập người dùng
		[HttpPost]
		public async Task<ActionResult> LoginAPI(string email, string password)
		{
			Dictionary<string, string> response = new Dictionary<string, string>();
			response["status"] = "200";
			response["message"] = "";
			try
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(apiUrl);

					var responses = await client.GetAsync($"Login/GetUser/{email},{password}");

					if (responses.IsSuccessStatusCode)
					{
						var content = await responses.Content.ReadAsStringAsync();
						var user = JsonConvert.DeserializeObject<User>(content);
						AuthHelper.setIdentity(user);
						AlertHelper.setToast("success", "Đăng nhập thành công.");
						//return View("UserProfile", user);
					}
					else
					{
						response["status"] = "400";
						response["message"] = "Thông tin tài khoản không hợp lệ.";
						//ViewBag.ErrorMessage = "Invalid login credentials.";

					}
				}
				return Content(JsonConvert.SerializeObject(response));
			}
			catch (Exception ex)
			{
				response["status"] = "404";
				response["message"] = "Lỗi mất kết nối với dữ liệu.";
				return Content(JsonConvert.SerializeObject(response));
			}


		}
		//Hàm này xử lý yêu cầu đăng xuất. Nếu người dùng đã đăng nhập, nó gỡ bỏ thông tin xác thực của người dùng khỏi phiên làm việc và trả về một thông báo JSON cho biết đăng xuất thành công.
		public ActionResult Logout()
		{
			if (AuthHelper.isLogin() == true)
			{
				AuthHelper.removeIdentity();
				AlertHelper.setToast("warning", "Đăng xuất thành công.");
			}
			return RedirectToAction("Index", "Home");
		}
		//Hàm này xử lý yêu cầu đăng ký tài khoản mới. Nó kiểm tra tính hợp lệ của thông tin đăng ký, bao gồm kiểm tra xem mật khẩu và mật khẩu nhập lại có khớp nhau không và kiểm tra xem email đã tồn tại trong cơ sở dữ liệu chưa. Nếu thông tin đăng ký hợp lệ, nó tạo một bản ghi mới trong cơ sở dữ liệu cho người dùng mới và trả về một thông báo JSON cho biết đăng ký thành công. Nếu có lỗi trong quá trình đăng ký, nó trả về một danh sách lỗi JSON để thông báo về các lỗi ModelState.
		public ActionResult Register(string email, string password, string rePassword)
		{
			Dictionary<string, string> response = new Dictionary<string, string>();
			response["status"] = "200";
			response["message"] = "";
			string a = password;
			string b = rePassword;

			if (rePassword.Equals(password) == false)
			{
				response["status"] = "400";
				response["message"] = "Mật khẩu không khớp nhau.";
				return Content(JsonConvert.SerializeObject(response));
			}
			if (Models.User.emailExists(email) == true)
			{
				response["status"] = "400";
				response["message"] = "Email này đã tồn tại.";
				return Content(JsonConvert.SerializeObject(response));
			}
			User model = new User();
			model.email = email;
			model.password = password;
			model.user_type = Models.User.TYPE_CUSTOM;
			if (ModelState.IsValid)
			{
				db.Users.Add(model);
				db.SaveChanges();
				AlertHelper.setToast("success", "Tạo tài khoản mới thành công.");
				response["message"] = "Tạo tài khoản mới thành công.";
			}
			else
			{
				response["status"] = "400";
				response["message"] = JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList());
			}
			return Content(JsonConvert.SerializeObject(response));
		}

		//Register API 
		[HttpPost]
		public async Task<ActionResult> RegisterAPI(string email, string password, string rePassword)
		{
			RegisterAccount registerAccount = new RegisterAccount();

			Dictionary<string, string> response = new Dictionary<string, string>();
			response["status"] = "200";
			response["message"] = "";
			//IList<string> postData = new List<string> { email, password, rePassword};

			//var data = new Dictionary<string, string>
			//{
			//	{ "email", email },
			//	{ "password", password },
			//	{ "rePassword", rePassword }
			//};
			//var content = new FormUrlEncodedContent(data);

			try
			{
				registerAccount.email = email; registerAccount.password = password; registerAccount.rePassword = rePassword;

				string data = JsonConvert.SerializeObject(registerAccount);

				StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

				//HttpResponseMessage reponsed = _client.PostAsync(_client.BaseAddress + "Login/RegisterUser/register", content).Result;
				var responses = _client.PostAsync(_client.BaseAddress + "Login/RegisterUser/register", content).Result;
				if (responses.IsSuccessStatusCode)
				{
					//var contents =  reponsed.Content.ReadAsStringAsync();
					//var user = JsonConvert.DeserializeObject<User>(contents);
					//AuthHelper.setIdentity(user);
					AlertHelper.setToast("success", "Đăng ký thành công.");
					//return View("UserProfile", user);
				}
				else
				{
					var errorContent = await responses.Content.ReadAsStringAsync();
					response["status"] = "400";
					response["message"] = errorContent;
					//response["status"] = "400";
					//response["message"] = "Thông tin tài khoản không hợp lệ.";
					//ViewBag.ErrorMessage = "Invalid login credentials.";
				}
				return Content(JsonConvert.SerializeObject(response));
				//using (var client = new HttpClient())
				//{
				//	client.BaseAddress = new Uri(apiUrl);

				//	var responses = await client.PostAsync("/Login", (HttpContent)postData);

				//	//if (!responses.IsSuccessStatusCode)
				//	//{
				//	//	// Đọc nội dung lỗi trả về từ API
				//	//	var errorContent = await responses.Content.ReadAsStringAsync();
				//	//	var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorContent);

				//	//	Console.WriteLine($"Error: {errorResponse["status"]}, Message: {errorResponse["message"]}");
				//	//}
				//	//else
				//	//{
				//	//	// Đọc nội dung phản hồi thành công
				//	//	var successContent = await responses.Content.ReadAsStringAsync();
				//	//	var successResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(successContent);

				//	//	Console.WriteLine($"Success: {successResponse["status"]}, Message: {successResponse["message"]}");
				//	//}
				//	if (responses.IsSuccessStatusCode)
				//                {
				//                    var contents = await responses.Content.ReadAsStringAsync();
				//                    var user = JsonConvert.DeserializeObject<User>(contents);
				//                    AuthHelper.setIdentity(user);
				//                    AlertHelper.setToast("success", "Đăng ký thành công.");
				//                    //return View("UserProfile", user);
				//                }
				//                else
				//                {
				//                    response["status"] = "400";
				//                    response["message"] = "Thông tin tài khoản không hợp lệ.";
				//                    //ViewBag.ErrorMessage = "Invalid login credentials.";

				//                }
				//            }
				//return Content(JsonConvert.SerializeObject(response));
			}

			catch (Exception ex)
			{
				//response["status"] = "404";
				//response["message"] = "Lỗi mất kết nối với dữ liệu.";
				//return Content(JsonConvert.SerializeObject(response));
			}
			return Content(JsonConvert.SerializeObject(response));

		}
	}
}