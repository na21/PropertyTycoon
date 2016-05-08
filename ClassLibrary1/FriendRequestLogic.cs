using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class FriendRequestLogic
    {
        /// <summary>
        /// This method creates a new friend request
        /// </summary>
        /// <param name="u"></param>
        /// <param name="friend"></param>
        public static void CreateFriendRequest(User u, User friend)
        {
            var friendRequest = new FriendRequest();
            friendRequest.User = u;
            friendRequest.Friend = friend;

            GameContext gc = new GameContext();
            gc.FriendRequests.Add(friendRequest);
        }

        /// <summary>
        /// This method allows a user to accept a friend request
        /// </summary>
        /// <param name="fr"></param>
        public static void AcceptFriendRequest(FriendRequest fr)
        {
            GameContext gc = new GameContext();

            Friends f = new Friends();
            f.User1 = fr.User;
            f.User2 = fr.Friend;

            gc.Friendships.Add(f);
            gc.FriendRequests.Remove(fr);     
        }

        /// <summary>
        /// This method allows a user to decline a friend request
        /// </summary>
        /// <param name="fr"></param>
        public static void DeclineFriendRequest(FriendRequest fr)
        {
            GameContext gc = new GameContext();
            gc.FriendRequests.Remove(fr);
        }
    }
}
