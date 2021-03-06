﻿using BusinessLogic;
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
                u.SkillPoints = 50;

                u.Badges = new List<Badge>();

                Badge b = new Badge()
                {
                    Date = DateTime.Now,
                    UserName = u.UserName,
                    User = u,
                    Name = "Registered"
                };

                u.Badges.Add(b);

                gc.Users.Add(u);

                Stat s = new Stat()
                {
                    User = u,
                    UserName = u.UserName,
                    GamesCreated = 0,
                    GamesJoined = 0,
                    GamesForfeit = 0
                };

                gc.Stats.Add(s);

                gc.SaveChanges();
            }

            return View();
        }

        public ActionResult Ranking(string display, int? n)
        {
            if (!User.Identity.IsAuthenticated)
                return View("NotAuthorized");

            User user = gc.GetUser(User.Identity.Name);

            //
            // Prep drop-down list box wiith these choices.
            //
            List<SelectListItem> choices = new List<SelectListItem>();

            choices.Add(new SelectListItem()
            {
                Value = "alltime",
                Text = "All-Time",
                Selected = true
            });

            choices.Add(new SelectListItem()
            {
                Value = "week",
                Text = "Week",
                Selected = false
            });
            choices.Add(new SelectListItem()
            {
                Value = "month",
                Text = "Month",
                Selected = false
            });
            choices.Add(new SelectListItem()
            {
                Value = "today",
                Text = "Today",
                Selected = false
            });

            ViewBag.display = choices;

            List<SelectListItem> num = new List<SelectListItem>();

            num.Add(new SelectListItem()
            {
                Value = "10",
                Text = "10",
                Selected = true
            });

            num.Add(new SelectListItem()
            {
                Value = "100",
                Text = "100",
                Selected = false
            });
            num.Add(new SelectListItem()
            {
                Value = "1000",
                Text = "1000",
                Selected = false
            });

            ViewBag.n = num;

            int take = n ?? 10;

            if (take < 0)
                take = 10;

            if (take > 1000)
                take = 1000;

            //
            // Display all-time point leaders.
            //
            if (display == null || display == "alltime")
            {
                var usersAllTime = (from u in gc.Users
                                orderby u.SkillPoints descending
                                select u).Take(take);

                List<User> ul = new List<User>();

                if (usersAllTime != null)
                    ul.AddRange(usersAllTime.ToList());

                return View(ul);
            }

            //
            // Display leaders for month, week, or today.
            //
            else if(display == "month" || display == "week" || display == "today")
            {
                DateTime startTime;

                switch(display)
                {
                    case "month": startTime = DateTime.Now.AddMonths(-1);
                        break;

                    case "week": startTime = DateTime.Now.AddDays(-7);
                        break;

                    case "today": startTime = DateTime.Now.AddHours(-24);
                        break;

                    default: startTime = DateTime.Now;
                        break;
                }

                var userSums = (from pe in gc.UserPointsEarned
                                where pe.CreatedAt >= startTime
                                group pe by new { pe.UserName } into g
                                select new { g.Key.UserName, Sum = g.Sum(pe => pe.Points) } into s
                                orderby s.Sum descending
                                select s).Take(take);

                List<User> ul = new List<User>();

                if (userSums != null)
                {
                    foreach (var item in userSums)
                    {
                        User u = new User()
                        {
                            UserName = item.UserName,
                            SkillPoints = item.Sum
                        };

                        ul.Add(u);
                    }
                }

                return View(ul);
            }

            // Default: No rankings to display.
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

            User u = gc.getUserFromIdentity(User);

            String[] friends = FriendLogic.getFriends(u, gc);
            String[] pendingFriendRequests = FriendLogic.getFriendRequestNotifications(u, gc);

            InvitationViewModel vm = new InvitationViewModel(pendingFriendRequests, friends, GameInvitationLogic.getGameInvitationNotifications(u, gc));

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelRequest(String friendUsername)
        {
            User u = gc.getUserFromIdentity(User);
            User friend = gc.GetUser(friendUsername);

            FriendLogic.CancelFriendRequest(u, friend, gc);
            return RedirectToAction("Friends");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFriend(String friendUsername)
        {
            User u = gc.getUserFromIdentity(User);
            User friend = gc.GetUser(friendUsername);

            FriendLogic.CreateFriendRequest(u , friend, gc);
            
            return RedirectToAction("Friends");     
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFriend(String friendUsername)
        {
            User u = gc.getUserFromIdentity(User);
            User friend = gc.GetUser(friendUsername);

            FriendLogic.DeleteFriend(u, friend, gc);
            return RedirectToAction("Friends");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptFriendRequest(String friendRequestUsername)
        {
            User u = gc.getUserFromIdentity(User);
            User friend = gc.GetUser(friendRequestUsername);

            FriendLogic.AcceptFriendRequest(u, friend, gc);

            return RedirectToAction("Friends");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeclineFriendRequest(String friendRequestUsername)
        {
            User u = gc.getUserFromIdentity(User);
            User friend = gc.GetUser(friendRequestUsername);

            FriendLogic.DeclineFriendRequest(u, friend, gc);

            return RedirectToAction("Friends");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGameInvitation(String[] invitedUsers)
        {
            User u = gc.getUserFromIdentity(User);
            GameInvitationLogic.CreateNewInvitation(u, invitedUsers, gc);

            return RedirectToAction("Friends");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptGameInvitation(String username, String invitedUser1, String invitedUser2, String invitedUser3)
        {
            User current = gc.getUserFromIdentity(User);
            User u = gc.GetUser(username);
            User u1 = gc.GetUser(invitedUser1);
            User u2 = gc.GetUser(invitedUser2);
            User u3 = gc.GetUser(invitedUser3);

            GameInvitation gi = GameInvitationLogic.getGameInvitation(u, u1, u2, u3, gc);
            GameInvitationLogic.AcceptInvitation(current, gi, gc);

            return RedirectToAction("Friends");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeclineGameInvitation(String username, String invitedUser1, String invitedUser2, String invitedUser3)
        {
            User current = gc.getUserFromIdentity(User);
            User u = gc.GetUser(username);
            User u1 = gc.GetUser(invitedUser1);
            User u2 = gc.GetUser(invitedUser2);
            User u3 = gc.GetUser(invitedUser3);

            GameInvitation gi = GameInvitationLogic.getGameInvitation(u, u1, u2, u3, gc);
            GameInvitationLogic.DeclineInvitation(current, gi, gc);

            return RedirectToAction("Friends");
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

                            //board.MakeCurrentPlayerMove(out doubles, out roll);
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