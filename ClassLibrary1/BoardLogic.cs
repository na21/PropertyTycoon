using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
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

        public static BoardUser GetBoardUser(this Board b, string userName)
        {
            return (from bu in b.BoardUsers
                    where bu.BoardId == b.Id && bu.UserName == userName
                    select bu).FirstOrDefault();
        }
    }
}
