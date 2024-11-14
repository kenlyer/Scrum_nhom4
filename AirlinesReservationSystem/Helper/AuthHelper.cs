using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AirlinesReservationSystem.Helper;
using AirlinesReservationSystem.Models;

namespace AirlinesReservationSystem.Helper
{
    public class AuthHelper
    {
        public static void setIdentity(User user)
        {
            HttpContext.Current.Session["loginSesstion"] = user;
        }
        public static void setIdentityEmployee(User user)
        {
            HttpContext.Current.Session["loginSesstionEmployee"] = user;
        }
        public static User getIdentity1()
        {
            try
            {
                if (isAdmin((User)HttpContext.Current.Session["loginSesstion"]) == true)
                {
                    User session = (User)HttpContext.Current.Session["loginSesstion"];
                    return session;
                }
            }
            catch
            { }
            return null;
        }
        public static User getIdentityEmployeee()
        {
            try
            {
                if (isAdmin((User)HttpContext.Current.Session["loginSesstionEmployee"]) == true)
                {
                    User session = (User)HttpContext.Current.Session["loginSesstionEmployee"];
                    return session;
                }
            }
            catch
            { }
            return null;
        }
        public static User getIdentity()
        {
            try
            {
                User session = (User)HttpContext.Current.Session["loginSesstion"];
                return session;
            }
            catch
            { }
            return null;
        }

        public static void removeIdentity()
        {
            HttpContext.Current.Session["loginSesstion"] = null;
        }
        public static void removeIdentityEmployee()
        {
            HttpContext.Current.Session["loginSesstionEmployee"] = null;
        }
        public static bool isLogin()
        {
            if (HttpContext.Current.Session["loginSesstion"] == null)
            {
                return false;
            }
            return true;
        }
        public static bool isLoginEmployeee()
        {
            if (HttpContext.Current.Session["loginSesstionEmployee"] == null)
            {
                return false;
            }
            return true;
        }
        //Kiểm tra user đang đăng nhập có quyền admin hay không.
        //user.quyen == 0
        public static bool isAdmin(User user)
        {
            if (user == null)
            {
                return false;
            }

            if (user.user_type == 0)
            {
                return true;
            }
            return false;
        }
    }
}