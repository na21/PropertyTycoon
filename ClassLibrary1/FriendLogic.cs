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
            FriendRequest friendRequest = new FriendRequest();
            friendRequest.User = u;
            friendRequest.Friend = friend;

            gc.FriendRequests.Add(friendRequest);
            gc.SaveChanges();
        }

        /// <summary>
        /// This method allows a user to accept a friend request
        /// </summary>
        /// <param name="fr"></param>
        public static void AcceptFriendRequest(User u, User friend, GameContext gc)
        {
            Friends f = new Friends();
            f.User1 = u;
            f.User2 = friend;
            f.UserName1 = u.UserName;
            f.UserName2 = friend.UserName;

            gc.Friendships.Add(f);
            CancelFriendRequest(friend, u, gc);
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
        public static void DeclineFriendRequest(User u, User friend, GameContext gc)
        {
            CancelFriendRequest(friend, u, gc);
            gc.SaveChanges();
        }

        /// <summary>
        /// This method allows a user to delete an existing friend
        /// </summary>
        /// <param name="u"></param>
        /// <param name="friend"></param>
        public static void DeleteFriend(User u, User friend, GameContext gc)
        {
            Friends friends = gc.Friendships.Where(f => f.UserName1.Equals(u.UserName) && f.UserName2.Equals(friend.UserName) || 
                                                        f.UserName1.Equals(friend.UserName) && f.UserName2.Equals(u.UserName)).SingleOrDefault();
            gc.Friendships.Remove(friends);
            gc.SaveChanges();
        }

        /// <summary>
        /// Returns a list of the User's friends
        /// </summary>
        /// <param name="u"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public static String [] getFriends(User u, GameContext gc)
        {
            IQueryable<String> f1 = gc.Friendships.Where(f => f.UserName1 == u.UserName).Select(f => f.UserName2);
            IQueryable<String> f2 = gc.Friendships.Where(f => f.UserName2 == u.UserName).Select(f => f.UserName1);
            String [] friends = f1.Union(f2).ToArray();
            return friends;
        }
        
        /// <summary>
        /// Returns list of pending FriendRequests for a User
        /// </summary>
        /// <param name="u"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public static String [] getFriendRequestNotifications(User u, GameContext gc)
        {
            IQueryable<String> friendRequests = gc.FriendRequests.Where(fr => fr.Friend.UserName == u.UserName).Select(fr => fr.User.UserName);
            String[] allFriendRequests = friendRequests.ToArray();
            return allFriendRequests;
        }               
    }
}
