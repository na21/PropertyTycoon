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

        public IEnumerable<GameInvitation> gi;

        public InvitationViewModel(string [] newFriends, IEnumerable<GameInvitation> invites)
        {
            friendRequests = newFriends;
            gi = invites;
        }
    }
}