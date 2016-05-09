using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Move
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int BoardId { get; set; }

        [ForeignKey("BoardId")]
        public virtual Board Board { get; set; }

        public int CurrentPos { get; set; }

        public int Roll { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

        public bool IsFirstMove { get; set; }

        [JsonIgnoreAttribute]
        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}
