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

            //
            // The property is not owned.
            //
            if (prop.User == null)
            {
                // give player option to buy
                // TODO: check player funds before offering to buy property
                bool buy = true;

                if (buy)
                {

                    // if they can afford it
                    if (boardUser.Money >= prop.Price)
                    {
                        boardUser.Money -= prop.Price;
                        prop.User = user;

                        // set prop.User ??
                    }

                    // else player can't afford property

                }
            }

            //
            // The property is owned.
            //
            else
            {
                // Someone else owns the property.
                if (prop.User != user)
                {
                    if (!prop.Mortgaged)
                    {
                        int payment = prop.Rent;

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
                prop.Mortgaged = true;
                boardUser.Money += (int)(Property.MortgagePercentage * prop.Price);
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
            return OwnsGroup(user, b, p.Group) && p.NumHouses < 4;
        }

        public static bool CanBuildHotel(this User user, Board b, Property p)
        {
            if (OwnsGroup(user, b, p.Group) && p.NumHouses >= 4)
                return true;

            return false;
        }

        public static void BuildHouse(this User user, Property p, int n = 1)
        {
            p.NumHouses += n;
        }

        public static void BuildHotel(this User user, Property p)
        {
            p.NumHotels++;
            p.NumHouses = 0;
        }
    }
}
