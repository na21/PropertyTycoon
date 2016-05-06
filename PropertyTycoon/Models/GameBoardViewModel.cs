using DataLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyTycoon.Models
{
    public class GameBoardViewModel
    {
        public Board GameBoard;

        public Hashtable propTable;
        public GameBoardViewModel(Board b)
        {
            GameBoard = b;
            
            propTable = new Hashtable();
            foreach(Property p in GameBoard.Properties)
            {
                propTable[p.Position] = p;
            }

        }

        public Property GetPropertyFromPosition(int Position)
        {
           return (Property)propTable[Position];
        }
    }

}