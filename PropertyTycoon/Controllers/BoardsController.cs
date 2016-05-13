using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using BusinessLogic;
using PropertyTycoon.Models;

namespace PropertyTycoon.Controllers
{   [Authorize]
    public class BoardsController : Controller
    {
        private GameContext db = new GameContext();

        // GET: Boards
        public ActionResult Index()
        {
            return RedirectToAction("GameFeed");
        }

        public ActionResult GameFeed()
        {
            User u = db.GetUser(User.Identity.Name);

            List<Board> activeBoards = new List<Board>();
            List<Board> completedBoards = new List<Board>();


            if (u.Boards != null)
            {
                foreach(Board b in u.Boards)
                {
                    if (b.Status == "Completed")
                        completedBoards.Add(b);

                    else
                        activeBoards.Add(b);
                }
            }

            ViewBag.activeBoards = activeBoards;
            ViewBag.completedBoards = completedBoards;

            return View();
        }

        public ActionResult MatchMaking()
        {
            User u = db.GetUser(User.Identity.Name);

            var boards = (from b in db.Boards
                          where u.SkillPoints >= b.minSkillRange && u.SkillPoints <= b.maxSkillRange
                          select b);

            List<Board> eligibleBoards = new List<Board>();

            if (boards != null)
            {
                foreach (Board b in boards.ToList())
                {
                    if (b.GetBoardUser(User.Identity.Name) == null)
                        eligibleBoards.Add(b);
                }
            }

            ViewBag.eligibleBoards = eligibleBoards;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Join(int? id)
        {
            User u = db.GetUser(User.Identity.Name);

            if (id == null)
                return View("BoardError");

            var board = (from b in db.Boards
                         where b.Id == id
                         select b).FirstOrDefault();

            if (board == null)
                return View("BoardError");

            db.AddPlayerToBoard(u, board);

            // Start the game once all players have joined.
            if(board.GetNumberofPlayers() == board.MaximumPlayers)
            {
                board.Status = "Active";
            }

            db.SaveChanges();
            return RedirectToAction("Details", new { id = board.Id });
        }

        // GET: Boards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = db.Boards.Find(id);

            if (board == null)
            {
                return HttpNotFound();
            }

            if (board.Status != "Active")
                return RedirectToAction("GameFeed");
            
            return View(new GameBoardViewModel(board, User));
        }

        // GET: Boards/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Board board)
        {
            if (ModelState.IsValid)
            {
                User boardOwner = db.getUserFromIdentity(User);
               
                Board new_board = db.CreateNewGameBoard(boardOwner, board.MaximumPlayers);
                new_board.GenerateBoardProperties();

                new_board.minSkillRange = board.minSkillRange;
                new_board.maxSkillRange = board.maxSkillRange;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(board);
        }

        // GET: Boards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = db.Boards.Find(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View(board);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MaximumPlayers,Status,minSkillRange,maxSkillRange")] Board board)
        {
            if (ModelState.IsValid)
            {
                db.Entry(board).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(board);
        }

        public ActionResult Forfeit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Board board = db.Boards.Find(id);

            if (board == null)
            {
                return HttpNotFound();
            }

            return View(board);
        }

        [HttpPost, ActionName("Forfeit")]
        [ValidateAntiForgeryToken]
        public ActionResult ForfeitConfirmed(int id)
        {
            Board board = db.Boards.Find(id);
            User u = db.GetUser(User.Identity.Name);

            if (board == null)
            {
                return HttpNotFound();
            }

            // Only active games can be forfeit.
            if (board.Status != "Active")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            board.PlayerForfeit(u);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Boards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = db.Boards.Find(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View(board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Board board = db.Boards.Find(id);

            if(board == null)
            {
                return HttpNotFound();
            }

            // In progress or completed games cannot be deleted.
            if(board.Status != "New")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Boards.Remove(board);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
