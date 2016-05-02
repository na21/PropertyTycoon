using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyTycoon.Models
{
    public class GameDetailViewModel
    {
        public int BoardId { get; set; }

        public string Winner { get; set; }

        public int MinSkillRange { get; set; }

        public int MaxSkillRange { get; set; }

        public int MaxPlayers { get; set; }

        public DateTime Date { get; set; }

        public int Points { get; set; }
    }
}