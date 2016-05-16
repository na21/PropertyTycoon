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
        public static void CreateNewInvitation(User u, User[] invitedUsers, GameContext gc)
        {
            var invitation = new GameInvitation();
            Board b = gc.CreateNewGameBoard(u, invitedUsers.Length, 20);
            invitation.Board = b;
            invitation.UserName = u.UserName;
            
            if (invitedUsers[0] != null)
                invitation.InvitedUser1 = invitedUsers[0];
            if (invitedUsers[1] != null)
                invitation.InvitedUser2 = invitedUsers[1];
            if (invitedUsers[2] != null)
                invitation.InvitedUser3 = invitedUsers[2];

            gc.GameInvitations.Add(invitation);
        }

        /// <summary>
        /// This method allows a User to accept a specific invitation
        /// </summary>
        /// <param name="i"></param>
        /// <param name="u"></param>
        public static void AcceptInvitation(User u, GameInvitation gi, GameContext gc)
        {
            if (gi.InvitedUser1.Equals(u))
            {
                gi.InvitedUser1 = null;
            }
            else if (gi.InvitedUser2.Equals(u))
            {
                 gi.InvitedUser2 = null;
            }
            else if (gi.InvitedUser3.Equals(u))
            {
                 gi.InvitedUser3 = null;
            }
           
            gc.AddPlayerToBoard(u, gi.Board);
            DeleteInvitation(gi, gc);
            gc.SaveChanges();
        }

        /// <summary>
        /// This method allows a User to decline a specific invitation
        /// </summary>
        /// <param name="i"></param>
        /// <param name="u"></param>
        public static void DeclineInvitation(User u, GameInvitation gi, GameContext gc)
        {
            if (gi.InvitedUser1.Equals(u))
            {
                gi.InvitedUser1 = null;
            }
            else if (gi.InvitedUser2.Equals(u))
            {
                gi.InvitedUser2 = null;
            }
            else if (gi.InvitedUser3.Equals(u))
            {
                gi.InvitedUser3 = null;
            }

            DeleteInvitation(gi, gc);
            gc.SaveChanges();
        }

        /// <summary>
        /// This method will delete the GameInvitation if all invitedUsers have responded to the request. 
        /// Will also delete the board if all users have declined
        /// </summary>
        /// <param name="gi"></param>
        public static void DeleteInvitation(GameInvitation gi, GameContext gc)
        {
            if ((gi.InvitedUser1.Equals(null)) && (gi.InvitedUser2.Equals(null)) && (gi.InvitedUser3.Equals(null)))
            {
                if (gi.Board.BoardUsers.Count == 1)
                    gc.Boards.Remove(gi.Board);

                gc.GameInvitations.Remove(gi);
            }
            gc.SaveChanges();
        }
    }
}
