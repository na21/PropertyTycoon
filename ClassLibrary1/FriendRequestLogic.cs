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
        /// <returns></returns>
        public static FriendRequest CreateFriendRequest(User u, User friend)
        {
            var friendRequest = new FriendRequest();
            friendRequest.User = u;
            friendRequest.Friend = friend;

            return friendRequest;
        }

        /// <summary>
        /// This method allows a user to accept a friend request
        /// </summary>
        /// <param name="fr"></param>
        public static void AcceptFriendRequest(FriendRequest fr)
        {
            User user1 = fr.User;
            User user2 = fr.Friend;

            Friends f = new Friends();
            f.User1 = user1;
            f.User2 = user2;

            f.Friendships.Add(user1);
            f.Friendships.Add(user1);         
        }

        /// <summary>
        /// This method allows a user to decline a friend request
        /// </summary>
        /// <param name="fr"></param>
        public static void DeclineFriendRequest(FriendRequest fr)
        {
            FriendRequest f = new FriendRequest();
            f.FriendRequests.Remove(fr);
        }
    }
}
