using System.ComponentModel.DataAnnotations;

namespace Portefeuille.Models
{
    public class Portefeuille
    {
        public int Id { get; set; }
        [Required]

        public float ScoreSharp { get; set; }

        public DateTime DateCreation { get; set; }

        public float RendementPrevu { get; set; }

        public float RisqueVolatilite { get; set; }
    }
}
