using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portefeuille.Models
{
    public class Donneeboursiere
    {
        public int Id { get; set; }
        [Required]

        public float Cloture { get; set; }

        public float Volume { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("Actif")]

        public int ActifId { get; set; }

    }
}
