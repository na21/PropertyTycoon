using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class PropertyLogic
    {
        public static int GetHouseCost(this Property p)
        {
            return (int)(p.Price * Property.HouseCostPercentage);
        }


        public static int GetHotelCost(this Property p)
        {
            return (int)(p.Price * Property.HotelCostPercentage);
        }

        public static string ProcessChanceCard(this Property p, BoardUser bu)
        {
            string result = "";
            if (p.Name == "Chance")
            {
                // Pick a Random Chance Card
                string[] chanceCards = { "GET_OUT_OF_JAIL", "PAY_EACH_PLAYER_50", "WON_LOTTERY_100" };
                string[] chanceCardDesc = { "You received a Get Out of Jail Card", "Pay each player $50", "Won Lottery receive $100" };
                Random rnd = new Random();

                int r = rnd.Next(chanceCards.Length);
                

                switch (chanceCards[r])
                {
                    case "GET_OUT_OF_JAIL":
                        bu.HasGetOutOfJail = true;
                        result = chanceCardDesc[0];
                        break;
                    case "PAY_EACH_PLAYER_50":
                        foreach(BoardUser otherPlayer in p.Board.GetOtherBoardUsersOnBoard(bu.User.UserName)){
                            otherPlayer.Money += 50;
                            bu.Money -= 50;
                        }
                        result = chanceCardDesc[1];
                        break;
                    case "WON_LOTTERY_100":
                        bu.Money += 100;
                        result = chanceCardDesc[2];
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public static string ProcessCommChestCard(this Property p, BoardUser bu)
        {
            string result = "";

            if (p.Name == "Community Chest")
            {
                // Pick a Random Chance Card
                string[] chanceCards = { "GET_OUT_OF_JAIL", "COLLECT_FROM_EACH_PLAYER_100", "INHERIT_150" };
                string[] chanceCardDesc = { "You received a Get Out of Jail Card", "Collect from each player $100", "Inherited $150" };

                Random rnd = new Random();

                int r = rnd.Next(chanceCards.Length);

                switch (chanceCards[r])
                {
                    case "GET_OUT_OF_JAIL":
                        bu.Position = 1;
                        bu.Money += 200;
                        result = chanceCardDesc[0];
                        break;
                    case "COLLECT_FROM_EACH_PLAYER_100":
                        foreach (BoardUser otherPlayer in p.Board.GetOtherBoardUsersOnBoard(bu.User.UserName)){
                            otherPlayer.Money -= 100;
                            bu.Money += 100;
                        }
                        result = chanceCardDesc[1];
                        break;
                    case "INHERIT_150":
                        bu.Money += 150;
                        result = chanceCardDesc[2];
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
    }
}
