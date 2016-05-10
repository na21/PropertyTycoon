using BusinessLogic;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame
{
    public class Program
    {
        private static GameContext db = new GameContext();
        private static Random rnd = new Random();


        static void Main(string[] args)
        {
            // --------------------------------------
            // begin initial game setup
            // --------------------------------------
            int numPlayers = 2;

            User p1 = new User();
            User p2 = new User();
            
            p1.UserName = "Player" + rnd.Next(750000);
            Console.WriteLine("Player 1: {0}", p1.UserName);

            p2.UserName = "Player" + rnd.Next(750000);
            Console.WriteLine("Player 2: {0}", p2.UserName);

            db.Users.Add(p1);
            db.Users.Add(p2);

            // adds board to db.Boards and calls SaveChanges()
            Board board = db.CreateNewGameBoard(p1, numPlayers);

            board.minSkillRange = 0;
            board.maxSkillRange = 500;

            db.AddPlayerToBoard(p2, board);

            board.AssignPlayerTurn(p1.UserName, 1);
            board.AssignPlayerTurn(p2.UserName, 2);

            board.GenerateBoardProperties();

            db.SaveChanges();
            // --------------------------------------
            // end initial game setup
            // --------------------------------------

            // --------------------------------------
            // begin main game loop
            // --------------------------------------
            bool gameOver = false;

            while (!gameOver)
            {
                User u = board.GetPlayerWithCurrentTurn();

                Console.WriteLine();
                Console.WriteLine("Player {0} starting turn...", u.UserName);
                Console.WriteLine();

                Console.WriteLine("*** User (before move) ****");
                board.GetBoardUser(u.UserName).Print();
                Console.WriteLine();


                bool doubles;
                int roll;
                Move m = board.MakeCurrentPlayerMove(out doubles, out roll);
                u.LandedOn(board, m);
                db.SaveChanges();

                Property p = board.GetPropertyFromPosition(m.CurrentPos);

                //
                // Print move outcome and player status to console
                //
                Console.WriteLine("*** Move ****");
                m.Print();
                Console.WriteLine();

                Console.WriteLine("*** Property ****");
                p.Print();
                Console.WriteLine();

                Console.WriteLine("*** User (after move) ****");
                board.GetBoardUser(u.UserName).Print();

                char c = Console.ReadKey().KeyChar;

                if (c == 'q')
                    gameOver = true;

                Console.Clear();

            } //end turn

            // --------------------------------------
            // end main game loop
            // --------------------------------------

            //Console.ReadKey();
        }
    }
}
