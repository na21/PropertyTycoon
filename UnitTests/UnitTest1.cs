﻿using System;
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

                var AddedPlayer = new_board.GetPlayerByUsername(player1.UserName);

                // Test 2 - A player should be able to join an existing Board.

                var player2 = new User();
                player2.UserName = "player2";
                bc.Users.Add(player2);

                bc.AddPlayerToBoard(player2, new_board);

                bool isDoubles;
                var rollVal = player1.Roll(out isDoubles);

                new_board.AssignPlayerTurn(player1.UserName, 2);
                new_board.AssignPlayerTurn(player2.UserName, 1);

                // The Player with the current Turn should be player 2 as the Turn Index = 1
                bc.SaveChanges();

                // Initialize all the Properties on the Board.
                new_board.GenerateBoardProperties();
                bc.SaveChanges();

                // Create a move on the board.
                int rollValue;
                Move firstMove = new_board.MakeCurrentPlayerMove(out isDoubles, out rollValue);
                bc.SaveChanges();

                // For this test, it is assumed that player buys property.
                // Check if money is deducted for property.
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
                Assert.AreEqual(false, player1.CanBuildHouse(new_board, p));

                var props = (from prop in new_board.Properties
                             where prop.Group == "Purple"
                             select prop);

                foreach(Property property in props)
                {
                    property.User = player1;
                }
                bc.SaveChanges();

                // Player should now own all purple
                Assert.AreEqual(true, player1.OwnsGroup(new_board, "Purple"));

                Property purpleProp = props.FirstOrDefault();

                //Check if player can build a house after owning group.
                Assert.AreEqual(true, player1.CanBuildHouse(new_board, purpleProp));

                //Check if player can build hotel after owning group with no houses.
                Assert.AreEqual(false, player1.CanBuildHotel(new_board, purpleProp));

                purpleProp.NumHouses = 4;
                bc.SaveChanges();

                // Check that player can build hotel after owning 4 houses on property.
                Assert.AreEqual(true, player1.CanBuildHotel(new_board, purpleProp));


            }
        }
    }
}
