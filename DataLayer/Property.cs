using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Property
    {
        public static double MortgagePercentage;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int BoardId { get; set; }

        [ForeignKey("BoardId")]
        public virtual Board Board { get; set; }

        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        public int Position { get; set; }

        public int Rent { get; set; }

        public int Price { get; set; }

        public string Group { get; set; }

        public string Name { get; set; }

        public int NumHotels { get; set; }

        public int NumHouses { get; set; }

        public bool Mortgaged { get; set; }
    }
}
