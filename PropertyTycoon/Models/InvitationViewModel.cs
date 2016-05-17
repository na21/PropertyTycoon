using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyTycoon.Models
{
    public class InvitationViewModel
    {
        public string[] friendRequests;

        public string[] friendsList;

        public IEnumerable<GameInvitation> gi;

        public InvitationViewModel(string [] fr, string [] allFriends, IEnumerable<GameInvitation> invites)
        {
            friendRequests = fr;
            gi = invites;
            friendsList = allFriends; 
        }
    }
}