using System;
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
using BusinessLogic;

namespace PropertyTycoon.Controllers
{
    public class EndMoveModel
    {
        public int BoardId { get; set; }
    }

    public class CreateMoveModel
    {
        public int Roll { get; set; }
        public bool Doubles { get; set; }

        public int BoardId { get; set; }
    }

    public class MoveResponseModel
    {
        public Move move { get; set; }

        public User ActivePlayer { get; set; }
    }

    public class ActivePlayerModel
    {
        public PropertyActionModel propertyState { get; set; }

        public User user { get; set; }

        public bool HasRolled { get; set; }
    }

    public class PropertyActionModel
    {
        public bool isPropertyPurchasable { get; set; }

        public int PropertyCost { get; set; }

        public bool isChanceorCommunity { get; set; }

        public string ChanceCommDescription { get; set; }

        public string PropertyName { get; set; }
    }

    public class BuyPropertyModel
    {
        public int BoardId { get; set; }

    }

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
        public ActivePlayerModel GetActivePlayer(int id)
        {
            Board board = db.Boards.Find(id);

            ActivePlayerModel response = new ActivePlayerModel();
            response.user = board.ActiveBoardPlayer;


            BoardUser bu = board.GetBoardUser(board.ActiveBoardPlayer.UserName);

            response.HasRolled = bu.HasRolled;

            Property property = (from p in board.Properties
                                 where p.Position == bu.Position
                                 select p).FirstOrDefault();


            response.propertyState = SetPropertyState(property, bu);

            return response;
        }

        // GET: api/Game/{id}/GetMovesList
        public IEnumerable<Move> GetMovesList(int id)
        {
            Board board = db.Boards.Find(id);

            return board.Moves;
        }

        // GET: api/Game/{id}/GetPropInfo/{propId}
        public PropertyActionModel GetPropInfo(int id, int position)
        {
            Board board = db.Boards.Find(id);

            BoardUser bu = board.GetBoardUser(board.ActiveBoardPlayer.UserName);

            Property property = (from p in board.Properties
                                 where p.Position == position
                                 select p).FirstOrDefault();



            return SetPropertyState(property, bu);
        }

        public PropertyActionModel SetPropertyState(Property property, BoardUser bu)
        {
            PropertyActionModel response = new PropertyActionModel();
            response.PropertyCost = property.Price;
            response.PropertyName = property.Name;

            if (property.Group == "No-Group")
            {
                response.isChanceorCommunity = true;
            }
            else
            {
                response.isChanceorCommunity = false;

                response.isPropertyPurchasable = property.User == null && bu.Money >= property.Price;

            }
            return response;
        }
        [HttpPost]
        [ResponseType(typeof(MoveResponseModel))]
        public IHttpActionResult EndMove(EndMoveModel m)
        {
            Board board = db.Boards.Find(m.BoardId);

            if (board == null)
            {
                return null;
            }

            var newMove = board.EndCurrentPlayerTurn();

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            MoveResponseModel response = new MoveResponseModel();
            response.move = newMove;
            response.ActivePlayer = board.ActiveBoardPlayer;

            return CreatedAtRoute("DefaultApi", null, response);
        }

        //POST: 
        [HttpPost]
        [ResponseType(typeof(Move))]
        public IHttpActionResult BuyProperty(BuyPropertyModel m)
        {
            Board board = db.Boards.Find(m.BoardId);

            if (board == null)
            {
                return null;
            }

            Move newMove = new Move();
            newMove.User = board.ActiveBoardPlayer;
            BoardUser boardUser = board.GetBoardUser(board.ActiveBoardPlayer.UserName);

            int position = boardUser.Position;

            Property property = (from p in board.Properties
                        where p.Position == position
                        select p).FirstOrDefault();

            if (property.User == null)
            {
                // give player option to buy if they have enough money
                if (boardUser.Money >= property.Price)
                {
                    boardUser.Money -= property.Price;
                    property.User = board.ActiveBoardPlayer;

                    newMove.Description = board.ActiveBoardPlayer.UserName + " purchased " + property.Name;
                } else
                    newMove.Description = board.ActiveBoardPlayer.UserName + " does not have enough money to buy " + property.Name;

                board.Moves.Add(newMove);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    throw;
                }

                return CreatedAtRoute("DefaultApi", null, newMove);
            } else
            {
                return null;
            }

            
            
        }
        //POST: 
        [HttpPost]
        [ResponseType(typeof(Move))]
        public IHttpActionResult CreateMove(CreateMoveModel m)
        {

            Board board = db.Boards.Find(m.BoardId);

            if (board == null)
            {
                return null;
            }
            
            Move newMove = board.MakeCurrentPlayerMove(m.Doubles, m.Roll);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }
            
            return CreatedAtRoute("DefaultApi", null, newMove) ;
            
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