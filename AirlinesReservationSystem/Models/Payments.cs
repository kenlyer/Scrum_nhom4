
namespace AirlinesReservationSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Payments")]
    public partial class Payments
    {
        [DisplayName("ID")]
        public int id { get; set; }

        [DisplayName("email_Payment")]
        [StringLength(255)]
        public string email_Payment { get; set; }

        [DisplayName("name_Payment")]
        [StringLength(255)]
        public string name_Payment { get; set; }

        [DisplayName("PayerID_Payment")]
        [StringLength(255)]
        public string PayerID_Payment { get; set; }

        [DisplayName("UserID")]
        public int? UserID { get; set; }
    }
}