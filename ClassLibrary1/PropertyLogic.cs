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
        public static void ProcessChanceCard(this Property p, BoardUser bu)
        {
            if (p.Name == "Chance")
            {
                // Pick a Random Chance Card
                string[] chanceCards = { "GET_OUT_OF_JAIL", "PAY_EACH_PLAYER_50", "WON_LOTTERY_100" };
                Random rnd = new Random();

                int r = rnd.Next(chanceCards.Length);

                switch (chanceCards[r])
                {
                    case "GET_OUT_OF_JAIL":
                        bu.HasGetOutOfJail = true;
                        break;
                    case "PAY_EACH_PLAYER_50":
                        foreach(BoardUser otherPlayer in p.Board.GetOtherBoardUsersOnBoard(bu.User.UserName){
                            otherPlayer.Money += 50;
                            bu.Money -= 50;
                        }
                        break;
                    case "WON_LOTTERY_100":
                        bu.Money += 100;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void ProcessCommChestCard(this Property p, BoardUser bu)
        {
            if (p.Name == "Community Chest")
            {
                // Pick a Random Chance Card
                string[] chanceCards = { "PASS_GO_COLLECT_200", "COLLECT_FROM_EACH_PLAYER_100", "INHERIT_150" };
                Random rnd = new Random();

                int r = rnd.Next(chanceCards.Length);

                switch (chanceCards[r])
                {
                    case "PASS_GO_COLLECT_200":
                        bu.Position = 1;
                        bu.Money += 200;

                        break;
                    case "COLLECT_FROM_EACH_PLAYER_100":
                        foreach (BoardUser otherPlayer in p.Board.GetOtherBoardUsersOnBoard(bu.User.UserName){
                            otherPlayer.Money -= 100;
                            bu.Money += 100;
                        }
                        break;
                    case "INHERIT_150":
                        bu.Money += 150;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
