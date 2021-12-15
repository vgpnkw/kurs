using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using ExcursionApp.Data;
using ExcursionApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ExcursionApp.Controllers;


    public class ExcursionController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ApplicationDbContext _context;
        
        public ExcursionController(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        //----< Displays the list of all the available Flights >---------

        [AllowAnonymous]
        public IActionResult ViewExcursion()
        {
            try
            {
                return View(_context.Excursion.ToList<Excursion>());
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        //----< Displays the Details of the Booked Flight >---------

        public IActionResult ViewMyExcursions()
        {
            try
            {
                return View(_context.ReservationInfos.ToList<ReservationInfo>());
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        //----< Displays the Details of the Selected Flight >---------

        [AllowAnonymous]
        public ActionResult ExcursionDetails(int? id)
        {
            if(id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        ExcursionApp.Models.Excursion excursion = _context.Excursion.Find(id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        if (excursion == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return View(excursion);
        }

        //----<Gets form to edit a specific flight detail >---------

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditExcursionDetails(int? id)
        {
            if(id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        ExcursionApp.Models.Excursion excursion = _context.Excursion.Find(id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        if (excursion == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(excursion);
        }

        //----<Post back edited result of a specific flight detail >---------

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditExcursionDetails(int? id, Models.Excursion flt)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var excursion = _context.Excursion.Find(id);
            if(excursion != null)
            {
                excursion.ExcursionNumber = flt.ExcursionNumber;
                excursion.ExcursionName = flt.ExcursionName;
                excursion.Source = flt.Source;
                excursion.Destination = flt.Destination;
                excursion.DepartsOn = flt.DepartsOn;
                excursion.ArrivesOn = flt.ArrivesOn;
                excursion.EconomyNos = flt.EconomyNos;
                excursion.FirstNos= flt.FirstNos;
                excursion.PriceEconomy= flt.PriceEconomy;
                excursion.PriceFirst= flt.PriceFirst;
                //excursion.Image = flt.Image;
                //excursion.PatrImageName = flt.PatrImageName;
                try
                {
                    _context.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        //----< Deletes the selected flight from the list >--------
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteExcursion(int? id)
        {
            if(id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            try
            {
                var excursion = _context.Excursion.Find(id);
                if(excursion != null)
                {
                    _context.Remove(excursion);
                    _context.SaveChanges();
                }
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index");
        }

        //----< This action searches the database for available flights
        //       based on search criteria and returns the result to the view >----
        [AllowAnonymous]
        public IActionResult AvailableExcursion(ExcursionSearch excursionSearch)
        {
            if (_context.Excursion.Any(fl => fl.ExcursionID == 0))
            {
                return RedirectToAction("Error", "Home");
            }
            try
            {
                TempData["DOJ"] = excursionSearch.DateSearch;
                
                /*
                 * ----< The if condition checks whether there ia a flight available for selected inputs 
                 *          Also the Controller returns only the required fields to the View, This is where we are returning the price
                 *          of the selected class so that the correct price can be displayed in View>----
                 */
                int totalTicketCount = excursionSearch.AdultSearch + excursionSearch.ChildrenSearch + excursionSearch.InfantSearch; 
                IQueryable<ReturningValue> excursionOrder = Enumerable.Empty<ReturningValue>().AsQueryable();

                if (excursionSearch.ClassSearch.Equals("economy"))
                {
                    var search = _context.Excursion.Where(s => s.Source.Equals(excursionSearch.FromSearch)
                                                     && s.Destination.Equals(excursionSearch.ToSearch)
                                                     && s.DepartureDate.Equals(excursionSearch.DateSearch)
                                                     && s.EconomyNos >= (excursionSearch.AdultSearch + excursionSearch.ChildrenSearch + excursionSearch.InfantSearch));
                excursionOrder = search.OrderBy(s => s.DepartsOn)
                        .Select(s => new ReturningValue
                        {
                            ExcursionNumber = s.ExcursionNumber,
                            Name = s.ExcursionName,
                            Departure = s.DepartsOn,
                            Arrival = s.ArrivesOn,
                            PFirst = 0,                     //Selected class is Economy, so make First 0
                            PEconomy = s.PriceEconomy * totalTicketCount,
                            TicketSelected = totalTicketCount
                        });
                }
                if (excursionSearch.ClassSearch.Equals("first"))
                {
                    var search = _context.Excursion.Where(s => s.Source.Equals(excursionSearch.FromSearch)
                                                     && s.Destination.Equals(excursionSearch.ToSearch)
                                                     && s.DepartureDate.Equals(excursionSearch.DateSearch)
                                                     && s.FirstNos >= (excursionSearch.AdultSearch + excursionSearch.ChildrenSearch + excursionSearch.InfantSearch));
                excursionOrder = search.OrderBy(s => s.DepartsOn)
                        .Select(s => new ReturningValue
                        {
                            ExcursionNumber = s.ExcursionNumber,
                            Name = s.ExcursionName,
                            Departure = s.DepartsOn,
                            Arrival = s.ArrivesOn,
                            PFirst = s.PriceFirst * totalTicketCount,
                            PEconomy = 0,               //Selected class is Economy, so make Economy 0
                            TicketSelected=totalTicketCount
                        });

                   
                }
                return View(excursionOrder);
            }
            catch (Exception)
            {
                //This is where we return no flights available for the given search
                return RedirectToAction("Error", "Home");
            }
        }

        //----< This action returns the seat numbers and their availability status
        //      depending based on the flight number to the view, This page is 
        //      is available only to users who are logedIn >----

        [Authorize]
        public IActionResult BookExcursion(int? id, int ticketNum, string ticketClass)
        {
            if (id == null || ticketNum == 0 || ticketClass == null )
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            try
            {
                TempData["ExcursionId"] = id;

                IQueryable<AvailableSeats> excursionSeatings = Enumerable.Empty<AvailableSeats>().AsQueryable();
                var seats = _context.ExcursionSeatings.Where(s => s.ExcursionNumber.Equals(id));

                // ----< This is where we return the Available seat numbers and the seat status of 
                //          selected class of Ticket, ie First or Economy >----

                if (ticketClass.Equals("Economy"))
                {

                    excursionSeatings = seats.Select(s => new AvailableSeats
                    {
                    excursionSeating = new ExcursionSeating
                        {
                            ExcursionNumber = s.ExcursionNumber,
                            EconomyClassSeatNumbers = s.EconomyClassSeatNumbers,
                            EconomyClassSeatStatus = s.EconomyClassSeatStatus
                        },
                        NumberOfTickets = ticketNum
                    });
                                                                
                }
                else
                {
                    excursionSeatings = seats.Select(s => new AvailableSeats
                    {
                    excursionSeating = new ExcursionSeating
                        {
                            ExcursionNumber = s.ExcursionNumber,
                            FirstClassSeatNumbers = s.FirstClassSeatNumbers,
                            FirstClassSeatStatus = s.FirstClassSeatStatus
                        },
                        NumberOfTickets = ticketNum
                    });
                }
                return View(excursionSeatings);
            }
            catch(Exception)
            {
                Console.Write("TUT");
                return RedirectToAction("Error", "Home");
            }
            
        }

        //----< This action Adds the Information entered by the user on the reservation
        //      page in to database. Here the design is to convert all the First Name,
        //      Last names and DOBs into comma seperated strings and add the strings
        //      into the respective Columns. Other Columns are Seat number, Flight number
        //      Journey date and the booking date >----

       
        public IActionResult BookTicket(TicketInfo bookTicket)
        {
            string firstName = string.Join(",", bookTicket.FirstName.ToArray());
            string lastName = string.Join(",", bookTicket.LastName.ToArray());
            string DOB = string.Join(",", bookTicket.DOB.ToArray());
            string DOJ = TempData["DOJ"].ToString();
            int excursionNumber = (int) TempData["ExcursionId"];
            string seatNumbers = string.Join(",", bookTicket.seatSelection.ToArray());

            //----< We need to make the sected seats unavailable to other users, inorder to change 
            // the current seat status we need to fetch it form the database, change the value
            // and update the database. We follow the logic of creating a dictionary of seats before chaneg
            // and by using using the seat numbers obtained from the view we update the dictionary.
            //Once the dictionary is updated we create a new string and update it to database. >----

            string[] seatNumber = { };
            string[] seatStatus = { };
            IDictionary<string, string> seatsDictionary = new Dictionary<string, string>();

            var seats = _context.ExcursionSeatings.Where(s => s.ExcursionNumber.Equals(excursionNumber));

            if(bookTicket.TicketClass.Equals("Economy"))
            {
                var economySeat = seats.Select(s => s.EconomyClassSeatNumbers).Single();
                
                var economyStatus = seats.Select(s => s.EconomyClassSeatStatus).Single();
                seatNumber = economySeat.Split(",");
                seatStatus = economyStatus.Split(",");

                for (int i = 0; i < seatNumber.Length; i++)
                {
                    seatsDictionary.Add(seatNumber[i], seatStatus[i]);
                }
            }
            else
            {
                var firstSeat = seats.Select(s => s.FirstClassSeatNumbers).Single();
                var firstStatus = seats.Select(s => s.FirstClassSeatStatus).Single();
                seatNumber = firstSeat.Split(",");
                seatStatus = firstStatus.Split(",");

                for (int i = 0; i < seatNumber.Length; i++)
                {
                    seatsDictionary.Add(seatNumber[i], seatStatus[i]);
                }
            }

            //Now updating the dictionary with the new value obtained
            foreach(var s in bookTicket.seatSelection.ToArray())
            {
                Console.WriteLine(s);
                seatsDictionary[s] = "X";
            }
            string[] updatedStatusArray = seatsDictionary.Values.ToArray();
            string updatedSeatStaus = string.Join(",", updatedStatusArray.ToArray());

            //Now we update the seat status column
            var excursionSeatings = _context.ExcursionSeatings;
            var result = excursionSeatings.SingleOrDefault(s => s.ExcursionNumber == excursionNumber);
            if (bookTicket.TicketClass.Equals("Economy"))
            {               
                if(result != null)
                {
                    result.EconomyClassSeatStatus = updatedSeatStaus;
                    _context.SaveChanges();
                }
            }
            else
            {
                if (result != null)
                {
                    result.FirstClassSeatStatus = updatedSeatStaus;
                    _context.SaveChanges();
                }
            }

            var reservarion = _context.ReservationInfos;
            if(reservarion != null)
            {
                var reservationInfo = new ReservationInfo
                {
                    ExcursionNumber = excursionNumber,
                    JourneyDate = DOJ,
                    BookingDate = DateTime.Now,
                    FirstNames = firstName,
                    LastNames = lastName,
                    DOBs = DOB,
                    SeatNumbers = seatNumbers
                };
                _context.ReservationInfos.Add(reservationInfo);
            }

            try
            {
                _context.SaveChanges();
            }
            catch(Exception)
            {
                return RedirectToAction("Error","Home");
            }


            return RedirectToAction("Index","Home");
        }


    }
