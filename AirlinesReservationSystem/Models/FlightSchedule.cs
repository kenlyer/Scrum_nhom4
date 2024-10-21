﻿namespace AirlinesReservationSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FlightSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FlightSchedule()
        {
            TicketManagers = new HashSet<TicketManager>();
            //bookedSeats = 0;
            //availableSeats = 0;
        }

        [DisplayName("ID")]
        public int id { get; set; }

        [DisplayName("Máy bay")]
        public int plane_id { get; set; }

        [DisplayName("Từ")]
        public int from_airport { get; set; }

        [DisplayName("Đến")]
        public int to_airport { get; set; }

        [DisplayName("Thời gian đi")]
        [DataType(DataType.DateTime)]
        public DateTime departures_at { get; set; }

        [DisplayName("Thời gian đến")]
        [DataType(DataType.DateTime)]
        public DateTime arrivals_at { get; set; }

        [DisplayName("Giá")]
        public int cost { get; set; }

        [DisplayName("Mã")]
        public string code { get; set; }

        [DisplayName("From")]
        public virtual AirPort AirPort { get; set; }

        [DisplayName("To")]
        public virtual AirPort AirPort1 { get; set; }

        [Range(20, 30, ErrorMessage = "Số lượng vé phải lớn hơn 20 và nhỏ hơn 30")]
        [DisplayName("Số lượng chỗ")]
        public int totalSeats { get; set; }

        [DisplayName("đã đặt")]
        public int bookedSeats { get; set; }

        [DisplayName("còn trống")]
        public int availableSeats { get; set; }

        [DisplayName("Trạng thái")]
        public string status_fs { get; set; }

        [DisplayName("Plane")]
        public virtual Plane Plane { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketManager> TicketManagers { get; set; }
    }
}
