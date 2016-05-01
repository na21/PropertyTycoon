using BusinessLogic;
using DataLayer;
using PropertyTycoon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PropertyTycoon.Controllers
{
    public class HomeController : Controller
    {
        private GameContext gc = new GameContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult RegistrationComplete()
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            //
            // We redirect to this action immediately following registration.
            // This is where we add the user to our custom user table.
            // Our custom user table contains application-specific user information.
            //
            // This technique was demonstrated in class with the TicTacToe example.
            // From this point forward, we should be able to pull the Identity user from
            // our custom user table as user records are never deleted.
            //
            if (gc.GetUserFromIdentity(User) == null)
            {
                User u = new User();
                u.UserName = User.Identity.Name;

                gc.Users.Add(u);
                gc.SaveChanges();
            }

            return View();
        }

        public ActionResult Ranking(string display)
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            User user = gc.GetUserFromIdentity(User);

            //
            // Display your rank and up to 9 users above you.
            //
            if (display == "me")
            {
                var myPoints = (from pe in gc.UserPointsEarned
                              where pe.User == user
                              select pe).FirstOrDefault();

                var peAbove = (from pe in gc.UserPointsEarned
                                   where pe.Points > myPoints.Points
                               orderby pe.Points ascending
                               select pe).Take(9);

                peAbove.Reverse();
                List<PointsEarned> pel = peAbove.ToList();
                
                pel.Add(myPoints);

                // If there aren't 9 users ranked above you, show 9 - n users ranked below.
                if(peAbove.Count() < 9)
                {
                    int n = 9 - peAbove.Count();

                    var peBelow = (from pe in gc.UserPointsEarned
                                       where pe.Points <= myPoints.Points && pe.User != user
                                   orderby pe.Points descending
                                   select pe).Take(n);

                    pel.AddRange(peBelow.ToList());
                }

                return View(pel);
            }

            //
            // Display all-time point leaders.
            //
            else if(display == "alltime")
            {
                var peAllTime = (from pe in gc.UserPointsEarned
                                orderby pe.Points descending
                                select pe).Take(10);

                return View(peAllTime.ToList());
            }

            else if(display == "week")
            {
                DateTime weekStart = DateTime.Now.AddDays(-7);

                var peWeek = (from pe in gc.UserPointsEarned
                              where pe.CreatedAt >= weekStart
                              orderby pe.Points descending
                              select pe).Take(10);

                return View(peWeek.ToList());
            }

            else if(display == "month")
            {
                DateTime monthStart = DateTime.Now.AddMonths(-1);

                var peMonth = (from pe in gc.UserPointsEarned
                               where pe.CreatedAt >= monthStart
                               orderby pe.Points descending
                               select pe).Take(10);

                return View(peMonth.ToList());
            }

            return View();
        }

        public ActionResult Play()
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");


            User user = gc.GetUserFromIdentity(User);

            Board board = (from bu in gc.BoardUsers
                           where bu.User == user
                           select bu.Board).FirstOrDefault();

            PlayViewModel pvm = new PlayViewModel()
            {
                MyTurn = board.GetPlayerWithCurrentTurn() == user
            };

            return View(pvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Play(string button)
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            User user = gc.GetUserFromIdentity(User);

            Board board = (from bu in gc.BoardUsers
                           where bu.User == user
                           select bu.Board).FirstOrDefault();

            switch (button)
            {
                case "Roll":
                    {
                        if (board.GetPlayerWithCurrentTurn() == user)
                        {
                            bool doubles;
                            int roll;

                            board.MakeCurrentPlayerMove(out doubles, out roll);
                        }
                        break;
                    }
                case "BuyProperty": break;
                case "MortgageProperty":break;
                case "BuyHouse": break;
                case "BuyHotel": break;
                case "SellHouse": break;
                case "SellHotel": break;

                default: return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PlayViewModel pvm = new PlayViewModel()
            {
                MyTurn = false
            };

            return View(pvm);
        }
    }
}