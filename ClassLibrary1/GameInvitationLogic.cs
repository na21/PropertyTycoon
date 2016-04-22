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

            // TODO: Create new Game Board for the User u. Max game size will be the size of invitedUser array. Assign it to invitation.Board
            // If no one accepts the invites within 24 hours, the invitation and board are dropped.
            return invitation;
        }

        /// <summary>
        /// This method allows a User to accept a specific invitation
        /// </summary>
        /// <param name="i"></param>
        /// <param name="u"></param>
        public static void AcceptInvitation(GameInvitation i, User u)
        {
            int pos = Array.IndexOf(i.InvitedUsers, u.UserName);
            i.IsAccepted[pos] = true;

            //TODO: Add this user to the game board created by the invitation (i.Board)
        }

        /// <summary>
        /// This method allows a User to decline a specific invitation
        /// </summary>
        /// <param name="i"></param>
        /// <param name="u"></param>
        public static void DeclineInvitation(GameInvitation i, User u)
        {
            var invitedUsers = i.InvitedUsers.ToList();
            invitedUsers.Remove(u.UserName);
            string[] updatedUsers = invitedUsers.ToArray();
            i.InvitedUsers = updatedUsers;
        }
    }
}
