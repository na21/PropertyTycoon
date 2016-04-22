using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class MoveLogic
    {
        public static bool HasPassedGo(this Move move)
        {
            if(move.CurrentPos > Property.NumberOfProperties)
            {
                move.CurrentPos = move.CurrentPos % Property.NumberOfProperties;
                return true;
            }

            return false;
        }

        public static bool HasLandedOnChange()
        {
            return true;
        }
    }
}
