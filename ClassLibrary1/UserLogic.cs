using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class UserLogic
    {
        private static Random rnd = new Random();
        /// <summary>
        /// generating a random number for each of the dice because
        /// we need to check for doubles.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="doubles"></param>
        /// <returns>returns a value in the range [1, 7)</returns>
        public static int Roll(this User user, out bool doubles)
        {
            int dice1 = rnd.Next(1, 7);
            int dice2 = rnd.Next(1, 7);

            doubles = dice1 == dice2;

            return dice1 + dice2;
        }

        /// <summary>
        /// Get number of game user has won since a given date.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="DateSince"></param>
        /// <returns></returns>
        public static int GetNumberOfGamesWonSince(this User user, DateTime DateSince)
        {
            return (from b in user.Boards
             where b.Winner == user
             select b.Id).Count();
        }

        /// <summary>
        /// Return the position of the Player on the Game Board.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int GetCurrentPositionOnBoard(this User user, Board board)
        {
            return (from bu in user.BoardUsers
                    where bu.Board == board
                    select bu.Position).FirstOrDefault();
        }

        /// <summary>
        /// This function is called when a player lands on a property. It 
        /// contains the property-based logic.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        public static void LandedOn(this User user, Board b, Move move)
        {
            BoardUser boardUser = b.GetBoardUser(user.UserName);

            int pos = move.CurrentPos;

            var prop = (from p in b.Properties
                        where p.Position == pos
                        select p).FirstOrDefault();

            // Check if user is in jail.
            if(boardUser.InJail)
                return;

            if (prop.Name == "Chance")
            {
                prop.ProcessChanceCard(boardUser);
                return;
            }

            if (prop.Name == "Community Chest")
            {
                prop.ProcessCommChestCard(boardUser);
                return;
            }

            if(prop.Name == "Income Tax")
            {
                boardUser.Money -= prop.Price;
                return;
            }

            if(prop.Name == "Luxury Tax")
            {
                boardUser.Money -= prop.Price;
                return;
            }

            //
            // The property is not owned.
            //
            if (prop.User == null)
            {
                // give player option to buy if they have enough money
                if (boardUser.Money >= prop.Price)
                {
                    bool buy = true;

                    if (buy)
                    {
                        boardUser.Money -= prop.Price;
                        prop.User = user;

                    }

                    // else player declined to buy property
                }

                // else player can't afford property


            }

            //
            // The property is owned.
            //
            else
            {
                // Someone else owns the property.
                if (prop.User != user)
                {
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
                        if (boardUser.Money >= payment)
                            boardUser.Money -= payment;

                        // Player doesn't enough money to pay.
                        //
                        else
                        {
                            //TODO: Give player option to trade or mortgage properties
                        }
                    }

                    // else property is mortgaged. player pays no rent.

                }

                // else player owns the property. do nothing
            }
        }

        /// <summary>
        /// This function is called when the player mortgages a property.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="prop"></param>
        public static void MortgageProperty(this User user, Board b, Property prop)
        {
            var boardUser = (from bu in user.BoardUsers
                             where bu.BoardId == b.Id
                             select bu).FirstOrDefault();

            // User owns this property and it's not currently mortgaged.
            if(!prop.Mortgaged && prop.User == user)
            {
                // Player can't mortgage the house if there are houses or hotels.
                // They must sell the houses/hotels to the bank first.
                if (prop.NumHouses > 0 || prop.NumHotels > 0)
                {
                    //Ask player if he wants to sell houses/hotel
                    bool sell = true;

                    if (sell)
                    {
                        if (prop.NumHotels > 0)
                            user.SellHotel(b, prop);

                        else if (prop.NumHouses > 0)
                            user.SellHouse(b, prop);

                        return;
                    }

                    // else user has declined to sell houses/hotels

                }

                else
                {
                    prop.Mortgaged = true;
                    boardUser.Money += (int)(Property.MortgagePercentage * prop.Price);
                }
            }
        }

        /// <summary>
        /// This function will check if the user owns all properties in a group.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="b"></param>
        /// <param name="pgroup"></param>
        /// <returns></returns>
        public static bool OwnsGroup(this User user, Board b, string pgroup)
        {
            int propsInGroup = (from p in b.Properties
                                where p.Group == pgroup
                                select p).Count();

            int propsOwned = (from p in b.Properties
                              where p.User == user && p.Group == pgroup
                              select p).Count();

            return propsInGroup == propsOwned;
        }

        public static bool CanBuildHouse(this User user, Board b, Property p)
        {
            BoardUser bu = b.GetBoardUser(user.UserName);

            int housePrice = (int)(p.Price * Property.HouseCostPercentage);

            if (OwnsGroup(user, b, p.Group) && p.NumHouses < 4 && bu.Money >= housePrice)
                return true;

            return false;
        }

        public static bool CanBuildHotel(this User user, Board b, Property p)
        {
            BoardUser bu = b.GetBoardUser(user.UserName);

            int hotelPrice = (int)(p.Price * Property.HotelCostPercentage);

            if (OwnsGroup(user, b, p.Group) && p.NumHouses >= 4 && bu.Money >= hotelPrice)
                return true;

            return false;
        }

        public static void BuildHouse(this User user, Board b, Property p, int n = 1)
        {
            BoardUser bu = b.GetBoardUser(user.UserName);

            int housePrice = (int)(p.Price * Property.HouseCostPercentage);

            bu.Money -= n * housePrice;
            p.NumHouses += n;
        }

        public static void BuildHotel(this User user, Board b, Property p)
        {
            BoardUser bu = b.GetBoardUser(user.UserName);

            int hotelPrice = (int)(p.Price * Property.HotelCostPercentage);

            bu.Money -= hotelPrice;
            p.NumHotels++;
            p.NumHouses = 0;
        }

        public static void SellHouse(this User user, Board b, Property p, int n = 1)
        {
            BoardUser bu = b.GetBoardUser(user.UserName);

            if (p.NumHouses > 0)
            {
                int housePrice = (int)(p.Price * Property.HouseCostPercentage);

                bu.Money += n * housePrice;
                p.NumHouses -= n;
            }
        }

        public static void SellHotel(this User user, Board b, Property p)
        {
            BoardUser bu = b.GetBoardUser(user.UserName);

            if (p.NumHotels > 0)
            {
                int hotelPrice = (int)(p.Price * Property.HotelCostPercentage);

                bu.Money += hotelPrice;
                p.NumHotels = 0;
            }
        }
    }
}
