using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcursionApp.Models;

    public class ExcursionSeating
    {
        public int ExcursionSeatingID { get; set; }
        public int ExcursionNumber { get; set; }
        public string? FirstClassSeatNumbers { get; set; }
        public string? FirstClassSeatStatus { get; set; }
        public string? EconomyClassSeatNumbers { get; set; }
        public string? EconomyClassSeatStatus { get; set; }
    }

    public class AvailableSeats
    {
        public ExcursionSeating? excursionSeating { get; set; }
        public int NumberOfTickets { get; set; }
    }

