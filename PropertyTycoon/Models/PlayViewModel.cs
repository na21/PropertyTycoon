using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyTycoon.Models
{
    public class PlayViewModel
    {
        public string Status { get; set; }

        public bool HasActiveGame { get; set; }

        public bool MyTurn { get; set; }

        public bool CanRoll { get; set; }

        public bool CanBuyProperty { get; set; }

        public bool CanMortgageProperty { get; set; }

        public bool CanBuyHouse { get; set; }

        public bool CanBuyHotel { get; set; }

        public bool CanSellHouse { get; set; }

        public bool CanSellHotel { get; set; }

    }
}