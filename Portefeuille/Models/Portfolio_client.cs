using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portefeuille.Models
{
    public class Portfolio_client
    {
        public int Id { get; set; }
        [Required]

        public float ScoreSharp { get; set; }

        public DateTime DateCreation { get; set; }

        public float RendementPrevu { get; set; }

        public float RisqueVolatilite { get; set; }


        public virtual ICollection<Allocation>? ListeAllocations { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

    }
}
