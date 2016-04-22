using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class GameContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<BoardUser> BoardUsers { get; set; }

        /// <summary>
        /// This function creates a new Board for a User.
        /// </summary>
        /// <param name="u">User</param>
        public Board CreateNewGameBoard(User u, int maxPlayers)
        {
            var board = new Board();
            board.MaximumPlayers = maxPlayers;

            Boards.Add(board);

            var boardUser = new BoardUser();

            boardUser.Board = board;
            boardUser.User = u;
            BoardUsers.Add(boardUser);

            SaveChanges();

            return board;
        }

    }
}
