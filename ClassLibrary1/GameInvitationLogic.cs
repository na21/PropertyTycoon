using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace BusinessLogic
{
    public static class GameInvitationLogic
    {
        /// <summary>
        /// This method creates a new game invitation
        /// </summary>
        /// <param name="u"></param>
        /// <param name="invitedUsers"></param>
        /// <returns></returns>
        public static GameInvitation CreateNewInvitation(User u, User[] invitedUsers)
        {
            var invitation = new GameInvitation();

            invitation.UserName = u.UserName;
            var i = 0;
            foreach (User invitedUser in invitedUsers)
            {
                invitation.InvitedUsers[i] = invitedUser.UserName;
                invitation.IsAccepted[i] = false;
                i++;
            }

            return invitation;
        }
    }
}
