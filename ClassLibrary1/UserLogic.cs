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
        public static void PlayerLandedOn(this User user, Board b, int pos)
        {

            var boardUser = (from bu in user.BoardUsers
                             where bu.BoardId == b.Id
                             select bu).FirstOrDefault();

            var prop = (from p in b.Properties
                        where p.Position == pos
                        select p).FirstOrDefault();


            //
            // The property is not owned.
            //
            if (prop.User == null)
            {
                // give option player to buy

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
                    int payment = prop.Rent;

                    payment += prop.NumHotels * 10;
                    payment += prop.NumHouses * 5;

                    // Player has enough money to pay.
                    if(boardUser.Money >= payment)
                        boardUser.Money -= payment;

                    // Player doesn't enough money to pay.
                    //
                    else
                    {
                        //TODO: Give player option to trade or mortgage properties
                    }

                }

                // else player owns the property; do nothing
            }
        }
    }
}
