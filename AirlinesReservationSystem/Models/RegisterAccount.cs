using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirlinesReservationSystem.Models
{
    public class RegisterAccount
    {
        public RegisterAccount()
        {

        }
        [DisplayName("Email")]
        [StringLength(255)]
        public string email { get; set; }

        [DisplayName("Mật khẩu")]
        [StringLength(255)]
        public string password { get; set; }

        [DisplayName("Mật khẩu lại")]
        [StringLength(255)]
        public string rePassword { get; set; }
    }
}