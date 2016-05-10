using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame
{
    public static class PrintExt
    {
        public static void Print(this BoardUser bu)
        {
            Console.WriteLine("Username: {0}", bu.UserName);
            Console.WriteLine("${0}", bu.Money);
            Console.WriteLine("Position: {0}", bu.Position);
            Console.WriteLine("Turn: {0}", bu.Turn);
            Console.WriteLine("InJail: {0}", bu.InJail);
            Console.WriteLine("HasGetOutOfJail: {0}", bu.HasGetOutOfJail);
            Console.WriteLine("GameOver: {0}", bu.GameOver);

        }

        public static void Print(this Move m)
        {
            Console.WriteLine("CurrentPos: {0}", m.CurrentPos);
            Console.WriteLine("Roll: {0}", m.Roll);
        }

        public static void Print(this Property p)
        {
            Console.WriteLine("Name: {0}", p.Name);
            Console.WriteLine("Position: {0}", p.Position);
            Console.WriteLine("Rent: {0}", p.Rent);
            Console.WriteLine("Price: {0}", p.Price);
            Console.WriteLine("Group: {0}", p.Group);
            Console.WriteLine("Mortgaged: {0}", p.Mortgaged);
            Console.WriteLine("# Houses: {0}", p.NumHouses);
            Console.WriteLine("# Hotels: {0}", p.NumHotels);

        }
    }
}
