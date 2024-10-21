using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Helper;

namespace AirlinesReservationSystem.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        //Đây là một phương thức được ghi đè từ lớp cơ sở Controller. Phương thức này được gọi trước khi một hành động trong AuthController hoặc các lớp con của nó được thực thi. Trong trường hợp này, nó kiểm tra xem người dùng đã xác thực (đăng nhập) hay chưa bằng cách gọi AuthHelper.getIdentity1(). Nếu người dùng chưa được xác thực (trả về null), nó sẽ chuyển hướng người dùng đến trang đăng nhập bằng cách sử dụng RedirectToRouteResult.
        // GET: Admin/Auth
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AuthHelper.getIdentity1() == null)
            {
                //Đây là một loại kết quả trả về từ một hành động của MVC. Nó chuyển hướng người dùng đến một tuyến đường (route) mới trong ứng dụng. Trong trường hợp này, nó chuyển hướng người dùng đến hành động Login trong SecurityController của khu vực Admin.
                //filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Forbidden");
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Security", Action = "Login" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}