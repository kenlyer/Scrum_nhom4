
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
        [Required(ErrorMessage = "Email_Payment is required.")]
        [StringLength(255, MinimumLength = 15, ErrorMessage = "Email_Payment must be between 15 and 255 characters.")]
        public string email_Payment { get; set; }

        [DisplayName("name_Payment")]
        [Required(ErrorMessage = "Name_Payment is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Name_Payment must be between 1 and 255 characters.")]
        public string name_Payment { get; set; }

        [DisplayName("PayerID_Payment")]
        [Required(ErrorMessage = "PayerID_Payment is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "PayerID_Payment must be between 1 and 255 characters.")]
        public string PayerID_Payment { get; set; }

        [DisplayName("UserID")]
        public int? UserID { get; set; }
    }
}