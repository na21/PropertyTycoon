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
            Board b = new Board();
            invitation.Board = b;
            invitation.UserName = u.UserName;

            BoardUser bu = new BoardUser();
            bu.Board = b;
            bu.User = u;
            bu.UserName = u.UserName;

            b.BoardUsers.Add(bu);

            var i = 0;
            foreach (User invitedUser in invitedUsers)
            {
                invitation.InvitedUsers[i] = invitedUser.UserName;
                invitation.IsAccepted[i] = false;
                i++;
            }
            
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

            BoardUser bu = new BoardUser();
            bu.User = u;
            bu.UserName = u.UserName;
            bu.BoardId = i.Board.Id;
            i.Board.BoardUsers.Add(bu);
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
