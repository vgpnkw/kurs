﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExcursionApp.Models;

    public class ReturningValue
    {
        public int ExcursionNumber { get; set; }
        [Display(Name = "Excursion Name")]
        public string? Name { get; set; }
        [Display(Name = "Depature Time")]
        public string? Departure { get; set; }
        [Display(Name = "Arrival Time")]
        public string? Arrival { get; set; }
        public int PFirst { get; set; }
        public int PEconomy { get; set; }
        public int TicketSelected { get; set; }
}

