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
            if (gc.GetUser(User.Identity.Name) == null)
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

            User user = gc.GetUser(User.Identity.Name);

            //
            // Display your rank and up to 9 users above you.
            //
            if (display == null || display == "me")
            {

                var usersAbove = (from u in gc.Users
                                   where u.SkillPoints > user.SkillPoints
                               orderby u.SkillPoints ascending
                               select u).Take(9);

                List<User> ul = new List<User>();

                if (usersAbove != null)
                {
                    usersAbove.Reverse();
                    ul.AddRange(usersAbove.ToList());
                }
                
                ul.Add(user);

                int aboveCount = usersAbove == null ? 0 : usersAbove.Count();

                // If there aren't 9 users ranked above you, show 9 - n users ranked below.
                if(aboveCount < 9)
                {
                    int n = 9 - aboveCount;

                    var usersBelow = (from u in gc.Users
                                   where u.SkillPoints <= user.SkillPoints && u.UserName != user.UserName
                                   orderby u.SkillPoints descending
                                   select u).Take(n);

                    if(usersBelow != null)
                        ul.AddRange(usersBelow.ToList());
                }

                return View(ul);
            }

            //
            // Display all-time point leaders.
            //
            else if(display == "alltime")
            {
                var usersAllTime = (from u in gc.Users
                                orderby u.SkillPoints descending
                                select u).Take(10);

                List<User> ul = new List<User>();


                if (usersAllTime != null)
                    ul.AddRange(usersAllTime.ToList());

                return View(ul);
            }

            else if(display == "week")
            {
                DateTime weekStart = DateTime.Now.AddDays(-7);

                var groupPEbyUser = (from pe in gc.UserPointsEarned
                                 where pe.CreatedAt >= weekStart
                                 group pe by pe.UserName);

                // TODO: Get sum of user's weekly totals
                // take top 10

                return View();
            }

            else if(display == "month")
            {
                DateTime monthStart = DateTime.Now.AddMonths(-1);

                var groupPEbyUser = (from pe in gc.UserPointsEarned
                                     where pe.CreatedAt >= monthStart
                                     group pe by pe.UserName);

                // TODO: Get sum of user's monthly totals
                // Take top 10

                return View();
            }

            return View();
        }

        public ActionResult History(string userName)
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            User user = userName == null ? gc.GetUser(User.Identity.Name) : gc.GetUser(userName);

            //
            // Requested user not found.
            //
            if (user == null)
                return View();

            //
            // Requested user found, get match history.
            //
            var matchHistory = (from pe in gc.UserPointsEarned
                                where pe.UserName == user.UserName
                                orderby pe.CreatedAt descending
                                select pe);

            List<PointsEarned> pel = new List<PointsEarned>();

            if (matchHistory != null)
                pel.AddRange(matchHistory.ToList());

            return View(pel);
        }

        public ActionResult Game(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            if (id == null)
                return View();

            var game = (from pe in gc.UserPointsEarned
                        where pe.BoardId == id
                        select pe).FirstOrDefault();

            var board = (from b in gc.Boards
                         where b.Id == id
                         select b).FirstOrDefault();

            if (game == null || board == null)
                return View();

            GameDetailViewModel gdvm = new GameDetailViewModel()
            {
                Winner = board.Winner.UserName,
                MinSkillRange = board.minSkillRange,
                MaxSkillRange = board.maxSkillRange,
                MaxPlayers = board.MaximumPlayers,
                Date = game.CreatedAt,
                Points = game.Points
            };


            return View(gdvm);
        }

        public ActionResult Play()
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            User user = gc.GetUser(User.Identity.Name);

            Board board = (from bu in gc.BoardUsers
                           where bu.UserName == user.UserName
                           select bu.Board).FirstOrDefault();

            PlayViewModel pvm = new PlayViewModel()
            {
                HasActiveGame = board != null,
                MyTurn = false

                // TODO:
                // need function GetCurrentProperty(user)
            };

            if (board != null)
                pvm.MyTurn = board.GetPlayerWithCurrentTurn() == user;
           

            return View(pvm);
        }

        public ActionResult Friends()
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Play(string button)
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            User user = gc.GetUser(User.Identity.Name);

            Board board = (from bu in gc.BoardUsers
                           where bu.UserName == user.UserName
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