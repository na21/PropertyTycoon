﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DataLayer;
using System.Web.Script.Serialization;

namespace PropertyTycoon.Controllers
{
    public class GameController : ApiController
    {
        private GameContext db = new GameContext();

        // GET: api/Game/{id}/BoardUsers
        public IEnumerable<BoardUser> GetBoardGameUsers(int id)
        {
            Board board = db.Boards.Find(id);
            
            return board.BoardUsers;
        }

        // GET: api/Game/{id}/GetActivePlayer
        public User GetActivePlayer(int id)
        {
            Board board = db.Boards.Find(id);

            return board.ActiveBoardPlayer;
        }

        // GET: api/Game/{id}/GetMovesList
        public ICollection<Move> GetMovesList(int id)
        {
            Board board = db.Boards.Find(id);

            return board.Moves;
        }
        //POST: 
        
        [ResponseType(typeof(Move))]
        public IHttpActionResult CreateMove(int id, int roll, bool doubles, string userName)
        {
            
            Board board = db.Boards.Find(id);

            if (board == null)
            {
                return null;
            }
            Move newMove = new Move();
            newMove.Board = board;
            newMove.UserName = userName;
            //newMove.CurrentPos;
            board.Moves.Add(newMove);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = newMove.Id}, newMove);
        }
        // GET: api/Game/5
        [ResponseType(typeof(ICollection<BoardUser>))]
        public ICollection<BoardUser> GetBoardUser(int id)
        {
            Board board = db.Boards.Find(id);
            if (board == null)
            {
                return null;
            }

            return board.BoardUsers;
        }

        // PUT: api/Game/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBoardUser(int id, BoardUser boardUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != boardUser.BoardId)
            {
                return BadRequest();
            }

            db.Entry(boardUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Game
        [ResponseType(typeof(BoardUser))]
        public IHttpActionResult PostBoardUser(BoardUser boardUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BoardUsers.Add(boardUser);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (BoardUserExists(boardUser.BoardId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = boardUser.BoardId }, boardUser);
        }

        // DELETE: api/Game/5
        [ResponseType(typeof(BoardUser))]
        public IHttpActionResult DeleteBoardUser(int id)
        {
            BoardUser boardUser = db.BoardUsers.Find(id);
            if (boardUser == null)
            {
                return NotFound();
            }

            db.BoardUsers.Remove(boardUser);
            db.SaveChanges();

            return Ok(boardUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BoardUserExists(int id)
        {
            return db.BoardUsers.Count(e => e.BoardId == id) > 0;
        }
    }
}