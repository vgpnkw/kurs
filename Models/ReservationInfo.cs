using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcursionApp.Models;

    public class ReservationInfo
    {
        public int ReservationInfoID { get; set; }
        public int ExcursionNumber { get; set; }
        public string? JourneyDate { get; set; }
        public DateTime BookingDate { get; set; }
        public string? FirstNames { get; set; }
        public string? LastNames { get; set; }
        public string? DOBs { get; set; }
        public string? SeatNumbers { get; set; }
    }

