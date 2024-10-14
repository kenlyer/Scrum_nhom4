﻿using System.Collections.Generic;
using System.Web.Mvc;
using AirlinesReservationSystem.Helper;
using Newtonsoft.Json;
using AirlinesReservationSystem.Models;
using System.Linq;
using System;

namespace AirlinesReservationSystem.Controllers
{
	public class SecurityController : Controller
	{
		Model1 db = new Model1();

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

		//Hàm này xử lý yêu cầu đăng ký tài khoản mới. Nó kiểm tra tính hợp lệ của thông tin đăng ký, bao gồm kiểm tra xem mật khẩu và mật khẩu nhập lại có khớp nhau không và kiểm tra xem email đã tồn tại trong cơ sở dữ liệu chưa. Nếu thông tin đăng ký hợp lệ, nó tạo một bản ghi mới trong cơ sở dữ liệu cho người dùng mới và trả về một thông báo JSON cho biết đăng ký thành công. Nếu có lỗi trong quá trình đăng ký, nó trả về một danh sách lỗi JSON để thông báo về các lỗi ModelState.
		public ActionResult Register(string email, string password, string rePassword)
		{
			Dictionary<string, string> response = new Dictionary<string, string>();
			response["status"] = "200";
			response["message"] = "";
			string a = password;
			string b = rePassword;

			//if(rePassword.Equals(password)==false)
			//         {
			//	response["status"] = "400";
			//	response["message"] = "Mật khẩu không khớp nhau.";
			//	return Content(JsonConvert.SerializeObject(response));
			//}
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
	}
}