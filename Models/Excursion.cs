using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExcursionApp.Models;
public class Excursion
{
    public int ExcursionID { get; set; }

    [Display(Name = "Trip Number")]
    public int ExcursionNumber { get; set; }

    [Display(Name = "Transportation Company Name")]
    public string? ExcursionName { get; set; }

    [Display(Name = "The beginning of the way")]
    public string? Source { get; set; }

    [Display(Name = "Destination")]
    public string? Destination { get; set; }

    [Display(Name ="Daparture Date")]
    public DateTime DepartureDate { get; set; }

    [Display(Name = "Depature Time")]
    public string? DepartsOn { get; set; }

    [Display(Name = "Arrival Time")]
    public string? ArrivesOn { get; set; }

    [Display(Name = "Economy Seats")]
    public int EconomyNos { get; set; }

    [Display(Name = "Firat Class Seats")]
    public int FirstNos { get; set; }

    [Display(Name = "Price Economy")]
    public int PriceEconomy { get; set; }

    [Display(Name = "Price First Class")]
    public int PriceFirst { get; set; }

}

