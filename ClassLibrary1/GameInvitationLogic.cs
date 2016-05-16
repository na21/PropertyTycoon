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
        public static void CreateNewInvitation(User u, String[] invitedUsers, GameContext gc)
        {
            var invitation = new GameInvitation();
            Board b = gc.CreateNewGameBoard(u, invitedUsers.Length);
            invitation.Board = b;
            invitation.UserName = u.UserName;

            User invitedUser1 = gc.GetUser(invitedUsers[0]);
            User invitedUser2 = gc.GetUser(invitedUsers[1]);
            User invitedUser3 = gc.GetUser(invitedUsers[2]);

            if (invitedUser1 != null)
                invitation.InvitedUser1 = invitedUser1;
            if (invitedUser2 != null)
                invitation.InvitedUser2 = invitedUser2;
            if (invitedUser3 != null)
                invitation.InvitedUser3 = invitedUser3;

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

        /// <summary>
        /// Returns a list of pending GameInvitations for a User
        /// </summary>
        /// <param name="u"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public static IEnumerable<GameInvitation> getGameInvitationNotifications(User u, GameContext gc)
        {
            IEnumerable<GameInvitation> gameInvitations = gc.GameInvitations.Where(gi => gi.InvitedUser1.Equals(u) || gi.InvitedUser2.Equals(u) || gi.InvitedUser3.Equals(u));
            return gameInvitations;
        }

        public static GameInvitation getGameInvitation(User u, User invitedUser1, User invitedUser2, User invitedUser3, GameContext gc)
        {
            GameInvitation gi = gc.GameInvitations.Where(g => g.UserName.Equals(u.UserName) && g.InvitedUser1.Equals(invitedUser1) &&
                                                         g.InvitedUser2.Equals(invitedUser2) && g.InvitedUser3.Equals(invitedUser3)).SingleOrDefault();
            return gi;
        }
    }
}
