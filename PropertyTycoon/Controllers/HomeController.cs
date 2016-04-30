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
        private static GameContext gc = new GameContext();

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

        public ActionResult Ranking(string display)
        {
            User user = (from u in gc.Users
                         where u.UserName == User.Identity.Name
                         select u).FirstOrDefault();

            PointsEarned myPoints = (from pe in gc.UserPointsEarned
                                         where pe.User == user
                                         select pe).FirstOrDefault();

            if (display == null || display == "me")
            {
                // Default page will show your rank and 9 users above you.

                var peAbove = (from pe in gc.UserPointsEarned
                                   orderby pe.Points
                                   where pe.Points < myPoints.Points
                                   select pe).Take(9);

                var userPE = (from pe in gc.UserPointsEarned
                              where pe.User == user
                              select pe).FirstOrDefault();

                List<PointsEarned> pel = peAbove.ToList();
                pel.Add(userPE);

                if(peAbove.Count() < 9)
                {
                    int n = 9 - peAbove.Count();

                    var peBelow = (from pe in gc.UserPointsEarned
                                       orderby pe.Points
                                       where pe.Points >= myPoints.Points && pe.User != user
                                       select pe).Take(n);

                    pel.AddRange(peBelow.ToList());
                }

                return View(pel);
            }

            return View();
        }

        public ActionResult Play()
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");


            User user = (from u in gc.Users
                         where u.UserName == User.Identity.Name
                         select u).FirstOrDefault();

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

            User user = (from u in gc.Users
                         where u.UserName == User.Identity.Name
                         select u).FirstOrDefault();

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