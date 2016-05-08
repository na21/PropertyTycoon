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
            User u = db.GetUser(User.Identity.Name);

            var boards = (from b in db.Boards
                                 where u.SkillPoints >= b.minSkillRange && u.SkillPoints <= b.maxSkillRange
                                 select b);

            List<Board> boardList = boards.ToList();
            List<Board> eligibleBoards = new List<Board>();

            foreach(Board b in boardList)
            {
                if (b.GetBoardUser(User.Identity.Name) == null)
                    eligibleBoards.Add(b);
            }


            if(eligibleBoards != null)
                ViewBag.eligibleBoards = eligibleBoards.ToList();

            if (u.Boards != null)
                ViewBag.yourBoards = u.Boards.ToList();

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
