using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class BoardLogic
    {
        public static string GetCurrentUserName(this Board b)
        {
            string userName = (from bu in b.BoardUsers
                               where bu.BoardId == b.Id && bu.TurnsRemaining > 0
                               select bu.UserName).FirstOrDefault();

            return userName;
        }

        public static bool isPlayerAllowedToJoin(this Board b)
        {
            return b.GetNumberofPlayers() < b.MaximumPlayers;
        }

        public static User GetBoardUserByUsername(this Board b, string userName)
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
                    where bu.BoardId == b.Id && bu.UserName == userName
                    select bu).FirstOrDefault();
        }

        public static string GetNextUserName(this Board b, int turn)
        {
            return (from bu in b.BoardUsers
                    where bu.BoardId == b.Id && bu.Turn == turn
                    select bu.UserName).FirstOrDefault();
        }
    }
}
