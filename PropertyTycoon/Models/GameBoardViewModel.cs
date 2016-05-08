using DataLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace PropertyTycoon.Models
{
    public class GameBoardViewModel
    {
        public Board GameBoard;
        
        public Hashtable propTable;

        public string currentUsername;

        public GameBoardViewModel(Board b, IPrincipal iUser)
        {
            GameBoard = b;
            
            propTable = new Hashtable();
            foreach(Property p in GameBoard.Properties)
            {
                propTable[p.Position] = p;
            }

            currentUsername = iUser.Identity.Name;
        }

        public Property GetPropertyFromPosition(int Position)
        {
           return (Property)propTable[Position];
        }
    }

}