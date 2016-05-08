using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class GameContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<BoardUser> BoardUsers { get; set; }

        public virtual DbSet<PointsEarned> UserPointsEarned { get; set; }

        /// <summary>
        /// This function creates a new Board for a User.
        /// </summary>
        /// <param name="u">User</param>
        public Board CreateNewGameBoard(User player, int maxPlayers)
        {
            var board = new Board();
            board.Status = "New";
            board.MaximumPlayers = maxPlayers;

            var boardUser = new BoardUser();

            boardUser.Board = board;
            boardUser.BoardId = board.Id;
            boardUser.User = player;
            BoardUsers.Add(boardUser);
            board.BoardUsers.Add(boardUser);
            board.ActiveBoardPlayer = player;

            Boards.Add(board);

            SaveChanges();
            
            return board;
        }

        public void AddPlayerToBoard(User player, Board board)
        {
            var boardUser = new BoardUser();

            boardUser.Board = board;
            boardUser.User = player;

            board.BoardUsers.Add(boardUser);

            SaveChanges();
        }

        public User GetUser(string userName)
        {
            return (from u in Users
                    where u.UserName == userName
                    select u).FirstOrDefault();
        }

        public User getUserFromIdentity(IPrincipal iUser)
        {
            return (from u in Users
                    where u.UserName == iUser.Identity.Name
                    select u).FirstOrDefault();
        }
    }
}
