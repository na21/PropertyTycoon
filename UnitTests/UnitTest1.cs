using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataLayer;
using BusinessLogic;
using System.Linq;

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
                bc.Friendships.RemoveRange(bc.Friendships);
                bc.FriendRequests.RemoveRange(bc.FriendRequests);

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
                new_board.minSkillRange = Board.LowestSkillPoints;
                new_board.maxSkillRange = Board.LowestSkillPoints + 500;

                var AddedPlayer = new_board.GetPlayerByUsername(player1.UserName);
                Assert.AreEqual(AddedPlayer, player1);

                // Test 2 - A player should be able to join an existing Board.

                var player2 = new User();
                player2.UserName = "player2";
                bc.Users.Add(player2);

                // Since there is only one player on the board and max is 2 
                // isPlayerAllowedToJoin() should return true.
                Assert.AreEqual(true, new_board.isPlayerAllowedToJoin());

                bc.AddPlayerToBoard(player2, new_board);

                // After Adding a new player the count should increase to 2.
                Assert.AreEqual(2, new_board.GetNumberofPlayers());

                bool isDoubles;
                var rollVal = player1.Roll(out isDoubles);

                // Check if Roll of Dice is in the correct range.
                Assert.IsTrue(rollVal > 0 && rollVal < 13);
                Assert.IsNotNull(isDoubles);

                new_board.AssignPlayerTurn(player1.UserName, 2);
                new_board.AssignPlayerTurn(player2.UserName, 1);

                // The Player with the current Turn should be player 2 as the Turn Index = 1
                Assert.AreEqual(new_board.GetPlayerWithCurrentTurn(), player2);
                bc.SaveChanges();

                // Initialize all the Properties on the Board.
                new_board.GenerateBoardProperties();
                bc.SaveChanges();

                Assert.AreEqual(new_board.Properties.Count, 40);

                // Create a move on the board.

            }
        }

        [TestMethod]
        public void TestProperties()
        {
            resetDbContext();

            using (var bc = new GameContext())
            {
                var player1 = new User();
                player1.UserName = "player1";
                bc.Users.Add(player1);

                // Test 1 - A player should be able to create a new Board.
                var new_board = bc.CreateNewGameBoard(player1, 2);
                new_board.minSkillRange = Board.LowestSkillPoints;
                new_board.maxSkillRange = Board.LowestSkillPoints + 500;

                var AddedPlayer = new_board.GetPlayerByUsername(player1.UserName);

                // Test 2 - A player should be able to join an existing Board.

                var player2 = new User();
                player2.UserName = "player2";
                bc.Users.Add(player2);

                bc.AddPlayerToBoard(player2, new_board);

                bool isDoubles;
                //int rollVal;// = player1.Roll(out isDoubles);

                //new_board.AssignPlayerTurn(player1.UserName, 2);
                //new_board.AssignPlayerTurn(player2.UserName, 1);

                // The Player with the current Turn should be player 2 as the Turn Index = 1
                bc.SaveChanges();

                // Initialize all the Properties on the Board.
                new_board.GenerateBoardProperties();
                bc.SaveChanges();

                // Create a move on the board.
                int rollValue = 10;
                isDoubles = false;

                Move firstMove = new_board.MakeCurrentPlayerMove(isDoubles, rollValue);
                

                var endMove = new_board.EndCurrentPlayerTurn(); // This method should end player 1's turn

                Assert.AreEqual(new_board.ActiveBoardPlayer, player2);

                var nextPlayer = new_board.GetPlayerWithCurrentTurn();
                Assert.AreEqual(nextPlayer, player2);

                rollValue = 12;
                Move p2FirstMove = new_board.MakeCurrentPlayerMove(isDoubles, rollValue);

                endMove = new_board.EndCurrentPlayerTurn(); // This method should end player 2's turn

                Assert.AreEqual(new_board.ActiveBoardPlayer, player2);

                // player2 rolled higher value, so next turn is p2's
                nextPlayer = new_board.GetPlayerWithCurrentTurn();

                Assert.AreEqual(nextPlayer, player2);

                rollValue = 6;
                var p2NextTurn = new_board.MakeCurrentPlayerMove(isDoubles, rollValue);
                Assert.AreEqual(p2NextTurn.CurrentPos, rollValue + 1);

                endMove = new_board.EndCurrentPlayerTurn();
                Assert.AreEqual(new_board.ActiveBoardPlayer, player1);

                // Next player is p1
                nextPlayer = new_board.GetPlayerWithCurrentTurn();
                Assert.AreEqual(nextPlayer, player1);

                rollValue = 10;
                isDoubles = true;

                var p1NextTurn = new_board.MakeCurrentPlayerMove(isDoubles, rollValue);
                Assert.AreEqual(p1NextTurn.CurrentPos, rollValue + 1);

                nextPlayer = new_board.GetPlayerWithCurrentTurn();
                Assert.AreEqual(nextPlayer, player1);
                // Do not end player turn if doubles
                bc.SaveChanges();

                // For this test, it is assumed that player buys property.
                // Check if money is deducted for property.

                firstMove.CurrentPos = 2;
                BoardUser bu = new_board.GetBoardUser(player1.UserName);
                bu.Money = 1000;

                Property p = new_board.GetPropertyFromPosition(firstMove.CurrentPos);
                int expectedMoney = bu.Money - p.Price;

                player1.LandedOn(new_board, firstMove);
                bc.SaveChanges();
                Assert.AreEqual(expectedMoney, bu.Money);

                // Player owns property p now.
                // Test mortgage property.

                expectedMoney = bu.Money + (int)(p.Price * Property.MortgagePercentage);
                player1.MortgageProperty(new_board, p);
                bc.SaveChanges();

                Assert.AreEqual(expectedMoney, bu.Money);

                // Now that property is mortgaged, it shouldn't
                // deduct money from player2 for landing on it.
                bu = new_board.GetBoardUser(player2.UserName);
                bu.Money = 500;

                expectedMoney = bu.Money;
                player2.LandedOn(new_board, firstMove);
                bc.SaveChanges();

                Assert.AreEqual(expectedMoney, bu.Money);

                // Check if player owns a group of properties.
                Assert.AreEqual(false, player1.OwnsGroup(new_board, "Purple"));

                //Check if player can build a house before owning group.
                Assert.AreEqual(false, bu.CanBuildHouse(p));

                var props = (from prop in new_board.Properties
                             where prop.Group == "Purple"
                             select prop);

                foreach (Property property in props)
                {
                    property.User = player1;
                }
                bc.SaveChanges();

                // Player should now own all purple
                Assert.AreEqual(true, player1.OwnsGroup(new_board, "Purple"));

                Property purpleProp = props.FirstOrDefault();

                //Check if player can build a house after owning group.
                Assert.AreEqual(true, bu.CanBuildHouse(purpleProp));

                //Check if player can build hotel after owning group with no houses.
                Assert.AreEqual(false, bu.CanBuildHotel(purpleProp));

                purpleProp.NumHouses = 3;
                bu.BuildHouse(purpleProp);
                bc.SaveChanges();

                //Check that player has built a house with BuildHouse()
                Assert.AreEqual(true, purpleProp.NumHouses == 4);

                // Check that player can build hotel after owning 4 houses on property.
                Assert.AreEqual(true, bu.CanBuildHotel(purpleProp));

                //Build a hotel now that we have 4 houses.
                bu.BuildHotel(purpleProp);

                Assert.AreEqual(true, purpleProp.NumHouses == 0);
                Assert.AreEqual(true, purpleProp.NumHotels == 1);

                bu = new_board.GetBoardUser(player1.UserName);
                bu.Money = 10;

                purpleProp.NumHouses = 0;
                purpleProp.NumHotels = 0;
                bc.SaveChanges();

                Assert.AreEqual(false, bu.CanBuildHotel(purpleProp));
                Assert.AreEqual(false, bu.CanBuildHouse(purpleProp));

            }
        }

        [TestMethod]
        public void TestPointsEarned()
        {
            resetDbContext();

            using (var bc = new GameContext())
            {
                var player1 = new User();
                player1.UserName = "player1";
                bc.Users.Add(player1);

                var new_board = bc.CreateNewGameBoard(player1, 2);
                new_board.minSkillRange = Board.LowestSkillPoints;
                new_board.maxSkillRange = Board.LowestSkillPoints + 500;

                var AddedPlayer = new_board.GetPlayerByUsername(player1.UserName);

                var player2 = new User();
                player2.UserName = "player2";
                bc.Users.Add(player2);

                bc.AddPlayerToBoard(player2, new_board);
                new_board.Status = "Completed";
                new_board.Winner = player1;

                PointsEarned pe = new PointsEarned();
                pe.Board = new_board;
                pe.User = player1;
                pe.CreatedAt = DateTime.Now.AddDays(-1);
                bc.SaveChanges();

                Assert.AreEqual(player1.GetNumberOfGamesWonSince(DateTime.Now.AddDays(-5)), 1);
            }
        }

        [TestMethod]
        public void TestCreateFriendRequest()
        {
            resetDbContext();

            using (var bc = new GameContext())
            {
                User u1 = new User();
                u1.UserName = "Test";

                User u2 = new User();
                u2.UserName = "TestFriend";

                FriendRequest fr = new FriendRequest();
                fr.User = u1;
                fr.Friend = u2;
                
                bc.FriendRequests.Add(fr);

                FriendRequest expected = new FriendRequest();
                expected = bc.FriendRequests.Find(fr.Id);

                Assert.AreEqual(expected, fr);
            }
        }
    }
}
