
namespace AirlinesReservationSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Seats")]
    public partial class Seats
    {
        [DisplayName("ID")]
        public int id { get; set; }
        [DisplayName("FlightSchedule")]
        public int flight_schedules_id { get; set; }

        [DisplayName("Vị trí")]
        [StringLength(255)]
        public string seat { get; set; }

        [DisplayName("trạng thái")]
        [ConcurrencyCheck]
        public int? isbooked { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }

        [DisplayName("thời gian giữ chỗ")]
        public DateTime? BookingExpiration { get; set; }
    }
}