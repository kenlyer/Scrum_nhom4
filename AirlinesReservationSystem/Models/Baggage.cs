using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AirlinesReservationSystem.Models
{
    [Table("Baggage")]
    public partial class Baggage
    {
        [DisplayName("ID")]
        public int id { get; set; }
        //id
        [DisplayName("Xách tay")]
        [Range(7, 7, ErrorMessage = "sai dữ liệu Xách tay")]
        public int carryon_bag { get; set; }
        //carryon_bag
        [DisplayName("Ký gửi")]
        [Range(1, 15, ErrorMessage = "Ký gửi không được nhỏ hơn 0 và lớn hơn 15")]
        public int signed_luggage { get; set; }
        //signed_luggage
        [DisplayName("Code")]
        [Required(ErrorMessage = "Code is required.")]
        public String code { get; set; }
        //code
        [DisplayName("User")]
        public int user_id { get; set; }
        //user_id

    }
}