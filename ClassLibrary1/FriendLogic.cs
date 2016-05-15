using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class FriendLogic
    {
        /// <summary>
        /// This method creates a new friend request
        /// </summary>
        /// <param name="u"></param>
        /// <param name="friend"></param>
        public static void CreateFriendRequest(User u, User friend, GameContext gc)
        {
            var friendRequest = new FriendRequest();
            friendRequest.User = u;
            friendRequest.Friend = friend;

            gc.FriendRequests.Add(friendRequest);
            gc.SaveChanges();
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
            f.UserName1 = fr.User.UserName;
            f.UserName2 = fr.Friend.UserName;

            gc.Friendships.Add(f);
            gc.FriendRequests.Remove(fr);
            gc.SaveChanges();
        }

        /// <summary>
        /// This method allows a user to cancel a friend request
        /// </summary>
        /// <param name="u"></param>
        /// <param name="friend"></param>
        /// <param name="gc"></param>
        public static void CancelFriendRequest(User u, User friend, GameContext gc)
        {
            FriendRequest friendrequest = gc.FriendRequests.Where(fr => fr.User.UserName.Equals(u.UserName) && fr.Friend.UserName.Equals(friend.UserName)).FirstOrDefault();
            gc.FriendRequests.Remove(friendrequest);
            gc.SaveChanges();
        }
        
        
        /// <summary>
        /// This method allows a user to decline a friend request
        /// </summary>
        /// <param name="fr"></param>
        public static void DeclineFriendRequest(FriendRequest fr)
        {
            GameContext gc = new GameContext();
            gc.FriendRequests.Remove(fr);
            gc.SaveChanges();
        }

        /// <summary>
        /// This method allows a user to delete an existing friend
        /// </summary>
        /// <param name="u"></param>
        /// <param name="friend"></param>
        public static void DeleteFriend(User u, User friend)
        {
            GameContext gc = new GameContext();
            Friends f = new Friends();

            f.User1 = u;
            f.UserName1 = u.UserName;
            f.User2 = friend;
            f.UserName2 = friend.UserName;

            gc.Friendships.Remove(f);
            gc.SaveChanges();
        }

        //public static IEnumerable<Friends> getFriends(User u)
        //{

        //}
    }
}
