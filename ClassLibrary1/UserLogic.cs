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
    }
}
