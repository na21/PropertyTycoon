using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataLayer;
using BusinessLogic;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// This methods resets and clears all DB objects
        /// </summary>
        public void resetDbContext()
        {
            using (var bc = new GameContext())
            {
                bc.Users.RemoveRange(bc.Users);
                bc.BoardUsers.RemoveRange(bc.BoardUsers);
                bc.Boards.RemoveRange(bc.Boards);

                bc.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateBoardAndAddPlayers()
        {
            resetDbContext();

            using (var bc = new GameContext())
            {
                var player1 = new User();
                player1.UserName = "player1";
                bc.Users.Add(player1);

                // Test 1 - A player should be able to create a new Board.
                var new_board = bc.CreateNewGameBoard(player1, 2);

                var a = new_board.GetBoardUserByUsername(player1.UserName);
                Assert.AreEqual(a, player1);

                // Test 2 - A player should be able to join an existing Board.

                var player2 = new User();
                player2.UserName = "player2";
                bc.Users.Add(player2);


                //new_board.
            }
        }
    }
}
