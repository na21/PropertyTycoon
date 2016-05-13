using DataLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class BoardLogic
    {

        public static bool isPlayerAllowedToJoin(this Board b)
        {
            return b.GetNumberofPlayers() < b.MaximumPlayers;
        }

        public static User GetPlayerByUsername(this Board b, string userName)
        {
            return (from bu in b.Users
                    where bu.UserName == userName
                    select bu).FirstOrDefault();
        }

        /// <summary>
        /// Return Number of Players currently on the Board.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetNumberofPlayers(this Board b)
        {
            return b.Users.Count();
        }

        public static BoardUser GetBoardUser(this Board b, string userName)
        {
            return (from bu in b.BoardUsers
                    where bu.UserName == userName
                    select bu).FirstOrDefault();
        }

        public static Property GetPropertyWithPos(this Board b, int pos)
        {
            return (from p in b.Properties
                    where p.Position == pos
                    select p).FirstOrDefault();
        }

        /// <summary>
        /// This returns the player with the next turn after the Active Player on the Board.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static User GetUserWithNextTurn(this Board b)
        {
            return (from bu in b.BoardUsers
                                   orderby bu.Turn ascending
                                   where bu.User != b.ActiveBoardPlayer
                                   select bu.User).FirstOrDefault();
        }

        /// <summary>
        /// Assigns a Turn index to the given User.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="userName"></param>
        public static void AssignPlayerTurn(this Board b, string userName, int turnIdx)
        {
            BoardUser bu = b.GetBoardUser(userName);
            bu.Turn = turnIdx;
        }

        /// <summary>
        /// Return Board Users other than the user on the Game Board.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static IEnumerable<BoardUser> GetOtherBoardUsersOnBoard(this Board b, string userName)
        {
            return (from bu in b.BoardUsers
                    where bu.UserName != userName
                    select bu);
        }

        /// <summary>
        /// This returns the player with the current move on the board.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static User GetPlayerWithCurrentTurn(this Board b)
        {
            // Check if all the players on the board have made their
            // first turns.
            if(b.Moves == null)
            {
                return b.ActiveBoardPlayer;
            }

            IEnumerable<User> users = (from m in b.Moves
                                 where m.IsFirstMove == true
                                 orderby m.Roll ascending
                                 select m.User).Distinct();

            int firsTurnMoves = users.Count();
            bool assignTurns = (from bu in b.BoardUsers
             where bu.Turn == -1
             select bu.UserName).Distinct().Count() == firsTurnMoves;

            if(assignTurns)
            {
                int moveIndex = firsTurnMoves;
                foreach(var u in users)
                {
                    b.AssignPlayerTurn(u.UserName, moveIndex--);
                }

                return users.Last();

            } else
            {
                return b.ActiveBoardPlayer;
            }
            
        }

        public static Property GetPropertyFromPosition(this Board b, int pos)
        {
            return (from p in b.Properties
                        where p.Position == pos
                        select p).FirstOrDefault();
        }

        public static User GetUserWithNextFirstTurn(this Board b, User u)
        {

            var otherUsers = (from bu in b.BoardUsers
                                     where bu.UserName != u.UserName
                                     select bu.User);

            foreach(var ou in otherUsers)
            {
                var firstTurns = (from m in b.Moves
                                  where m.IsFirstMove == true && m.UserName == ou.UserName
                                  select m.UserName).Distinct().FirstOrDefault();

                if (firstTurns == null)
                    return ou;
            }

            return null;
        }

        public static bool isPlayerFirstMove(this Board b, User player)
        {
            Move move = (from m in b.Moves
                      where m.UserName == player.UserName && m.IsFirstMove == true
                      select m).FirstOrDefault();

            return move == null;
        }

        public static Move EndCurrentPlayerTurn(this Board b)
        {
            User player = b.GetPlayerWithCurrentTurn();

            Move newMove = new Move();
            newMove.Roll = 0;
            newMove.Board = b;
            newMove.Description = player.UserName + " ended their Turn.";
            newMove.UserName = player.UserName;
            newMove.User = player;

            if (b.Moves == null)
            {
                b.Moves = new Collection<Move>();
            }

            b.ActiveBoardPlayer = b.GetUserWithNextTurn();

            var otherPlayer = b.GetUserWithNextFirstTurn(player);
            if (otherPlayer != null)
            {
                b.ActiveBoardPlayer = otherPlayer;
            }
            else
            {
                // All 1st turns completed get user with highest Roll Value

                if (b.Moves.Last().IsFirstMove == true)
                {
                    b.ActiveBoardPlayer = (from m in b.Moves
                                           where m.IsFirstMove == true
                                           orderby m.Roll descending
                                           select m.User).Distinct().FirstOrDefault();
                }
            }

            BoardUser bu = b.GetBoardUser(player.UserName);
            bu.HasRolled = false;
            b.Moves.Add(newMove);

            return newMove;
        }

        /// <summary>
        /// This method creates a new move for the current player on the Board.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Move MakeCurrentPlayerMove(this Board b, bool isDoubles, int RollValue)
        {
            User player = b.GetPlayerWithCurrentTurn();

            Move newMove = new Move();
            newMove.Roll = RollValue;
            newMove.Board = b;
            newMove.UserName = player.UserName;
            newMove.User = player;
            newMove.Description = "";

            BoardUser bu = b.GetBoardUser(player.UserName);

            if (b.Moves == null)
            {
                b.Moves = new Collection<Move>();
            }

            if (b.isPlayerFirstMove(player))
            {
                newMove.IsFirstMove = true;
                newMove.CurrentPos = 1;

                newMove.Description = player.UserName + "'s : First Move - Rolled " + newMove.Roll.ToString();

                b.Moves.Add(newMove);
                
                return newMove;
            }

            if (isDoubles)
                newMove.Description += player.UserName + " has rolled doubles!, value: " + newMove.Roll.ToString();
            else
                newMove.Description += player.UserName + " has rolled, value:  " + newMove.Roll.ToString();


            // If User currently in Jail, only double can take them out.
            if (bu.InJail)
            {
                if (isDoubles)
                {
                    bu.InJail = false;
                    newMove.CurrentPos = player.GetCurrentPositionOnBoard(b) + RollValue;
                    newMove.Description += " player gets out of Jail. ";
                }
                else
                {
                    newMove.Description += " player remains in Jail. ";
                    newMove.CurrentPos = player.GetCurrentPositionOnBoard(b);
                }

            } else
            {
                newMove.CurrentPos = player.GetCurrentPositionOnBoard(b) + RollValue;
            }

            // If landed on Go to jail.
            if (newMove.CurrentPos == Board.GoToJailPosition)
            {
                newMove.Description += " player goes to Jail !";
                newMove.CurrentPos = Board.JailPosition;
                bu.InJail = true;
            }

            // If Player passes Go add $200 to his account.
            // Resets the position after player passes go.
            if (newMove.HasPassedGo())
            {
                bu.Money += Board.PassGoMoney;
                newMove.Description += " Passed Go! Collect $200.";
            }


            Property prop = b.GetPropertyFromPosition(newMove.CurrentPos);

            newMove.Description += " Landed on " + prop.Name + " !";

            if (prop.Name == "Chance")
                newMove.Description +=  " " + prop.ProcessChanceCard(bu);
            else if (prop.Name == "Community Chest")
                newMove.Description += " " +  prop.ProcessCommChestCard(bu);
            else if (prop.Name == "Income Tax")
            {
                bu.Money -= prop.Price;
                newMove.Description += " Pay $" + prop.Price.ToString(); 
            }
            else if (prop.Name == "Luxury Tax")
            {
                bu.Money -= prop.Price;
                newMove.Description += " Pay $" + prop.Price.ToString();
            } else if (prop.User != null && prop.User != player)
            {
                // Need to Pay Rent.
                BoardUser buOwner = b.GetBoardUser(prop.User.UserName);

                // Only pay rent if the property isn't mortgaged and the owner
                // isn't in jail.
                if (!prop.Mortgaged && !buOwner.InJail)
                {

                    int payment = prop.Rent;

                    //Check if owners owns the entire group, if so rent is doubled.
                    if (prop.User.OwnsGroup(b, prop.Group))
                        payment *= 2;

                    payment += prop.NumHotels * 10;
                    payment += prop.NumHouses * 5;

                    // Player has enough money to pay.
                    if (bu.Money >= payment)
                    {
                        bu.Money -= payment;
                        buOwner.Money += payment;
                        newMove.Description += "Pay $" + payment.ToString() + " Rent to " + buOwner.UserName;
                    }

                }
            }

            // Update Player  position
            if (isDoubles)
                bu.HasRolled = false;
            else
                bu.HasRolled = true;

            bu.Position = newMove.CurrentPos;
            b.Moves.Add(newMove);
            return newMove;
        }

        /// <summary>
        /// Called when a player forfeits the game.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="u"></param>
        public static void PlayerForfeit(this Board b, User u)
        {
            PointsEarned pe = new PointsEarned()
            {
                User = u,
                Board = b,
                CreatedAt = DateTime.Now,
                Points = -25
            };

            u.PointsEarned.Add(pe);
            u.SkillPoints -= 25;

            BoardUser bu = b.GetBoardUser(u.UserName);
            bu.GameOver = true;

            // if only 1 player remains after forfeit, the game
            // is over.
            if (b.GetNumberofActivePlayers() == 1)
            {
                b.Status = "Completed";
                
                foreach(User user in b.Users)
                {
                    BoardUser boardUser = b.GetBoardUser(user.UserName);

                    if(!boardUser.GameOver)
                    {
                        b.Winner = boardUser.User;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Return the number of players still in the game.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetNumberofActivePlayers(this Board b)
        {
            int count = 0;

            foreach (User user in b.Users)
            {
                BoardUser bu = b.GetBoardUser(user.UserName);

                if (!bu.GameOver)
                    ++count;
            }

            return count;
        }

        /// <summary>
        /// Sets the properties and their options to the Game Board.
        /// </summary>
        /// <param name="b"></param>
        public static void GenerateBoardProperties(this Board b)
        {
            // http://www.math.yorku.ca/~zabrocki/math2042/Monopoly/prices.html
            Property p = new Property();
            b.Properties = new Collection<Property>();

            p.Name = "Mediterranean Ave.";
            p.Position = 2;
            p.Price = 60;
            p.Rent = 2;
            p.Group = "Purple";
            p.Board = b;
            p.BoardId = b.Id;
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Baltic Ave.";
            p.Position = 4;
            p.Price = 60;
            p.Rent = 4;
            p.Group = "Purple";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Oriental Ave.";
            p.Position = 7;
            p.Price = 100;
            p.Rent = 6;
            p.Group = "Light-Green";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Vermont Ave.";
            p.Position = 9;
            p.Price = 100;
            p.Rent = 6;
            p.Group = "Light-Green";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Connecticut Ave.";
            p.Position = 10;
            p.Price = 120;
            p.Rent = 8;
            p.Group = "Light-Green";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "St. Charles Place";
            p.Position = 12;
            p.Price = 140;
            p.Rent = 10;
            p.Group = "Violet";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();

            p.Name = "States Ave.";
            p.Position = 14;
            p.Price = 140;
            p.Rent = 10;
            p.Group = "Violet";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Virginia Ave.";
            p.Position = 15;
            p.Price = 160;
            p.Rent = 12;
            p.Group = "Violet"; p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "St. James Place";
            p.Position = 17;
            p.Price = 180;
            p.Rent = 14;
            p.Group = "Orange";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Tennessee Ave.";
            p.Position = 19;
            p.Price = 180;
            p.Rent = 14;
            p.Group = "Orange";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "New York Ave.";
            p.Position = 20;
            p.Price = 200;
            p.Rent = 16;
            p.Group = "Orange";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Kentucky Ave.";
            p.Position = 22;
            p.Price = 220;
            p.Rent = 18;
            p.Group = "Red";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Indiana Ave.";
            p.Position = 24;
            p.Price = 220;
            p.Rent = 18;
            p.Group = "Red";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Illinois Ave.";
            p.Position = 25;
            p.Price = 240;
            p.Rent = 20;
            p.Group = "Red";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Atlantic Ave.";
            p.Position = 27;
            p.Price = 260;
            p.Rent = 22;
            p.Group = "Yellow";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();

            p.Name = "Ventnor Ave.";
            p.Position = 28;
            p.Price = 260;
            p.Rent = 22;
            p.Group = "Yellow";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Marvin Gardens";
            p.Position = 30;
            p.Price = 280;
            p.Rent = 22;
            p.Group = "Yellow";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Pacific Ave.";
            p.Position = 32;
            p.Price = 300;
            p.Rent = 26;
            p.Group = "Dark-Green";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "North Carolina Ave.";
            p.Position = 33;
            p.Price = 300;
            p.Rent = 26;
            p.Group = "Dark-Green";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Pennsylvania Ave.";
            p.Position = 35;
            p.Price = 320;
            p.Rent = 28;
            p.Group = "Dark-Green";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Park Place";
            p.Position = 38;
            p.Price = 350;
            p.Rent = 35;
            p.Group = "Dark-Blue";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Boardwalk";
            p.Position = 40;
            p.Price = 400;
            p.Rent = 50;
            p.Group = "Dark-Blue";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Electric Company";
            p.Position = 13;
            p.Price = 150;
            p.Rent = 0;
            p.Group = "Utilities";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Water Works";
            p.Position = 29;
            p.Price = 150;
            p.Rent = 0;
            p.Group = "Utilities";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Reading Railroad";
            p.Position = 6;
            p.Price = 200;
            p.Rent = 0;
            p.Group = "Railroad";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Pennsylvania Railroad";
            p.Position = 16;
            p.Price = 200;
            p.Rent = 0;
            p.Group = "Railroad";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "B. & O. Railroad";
            p.Position = 26;
            p.Price = 200;
            p.Rent = 0;
            p.Group = "Railroad";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Short Line Railroad";
            p.Position = 36;
            p.Price = 200;
            p.Rent = 0;
            p.Group = "Railroad";
            p.User = null;
            b.Properties.Add(p);

            // Other properties like chance, community chest etc.

            p = new Property();
            p.Name = "Go";
            p.Position = 1;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Just Visiting/Jail";
            p.Position = 11;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Free Parking";
            p.Position = 21;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Income Tax";
            p.Position = 5;
            p.Price = 200;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Chance";
            p.Position = 8;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Chance";
            p.Position = 23;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Chance";
            p.Position = 37;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Community Chest";
            p.Position = 3;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Community Chest";
            p.Position = 18;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Community Chest";
            p.Position = 34;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Luxury Tax";
            p.Position = 39;
            p.Price = 100;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);

            p = new Property();
            p.Name = "Go to Jail";
            p.Position = 31;
            p.Price = 0;
            p.Rent = 0;
            p.Group = "No-Group";
            p.User = null;
            b.Properties.Add(p);
        }
    }
}
