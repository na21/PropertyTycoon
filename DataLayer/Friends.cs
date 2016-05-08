using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Friends
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User1")]
        public string UserName1 { get; set; }
        public User User1 { get; set; }

        [ForeignKey("User2")]
        public string UserName2 { get; set; }
        public User User2 { get; set; }
    }
}