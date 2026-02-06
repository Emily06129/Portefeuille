using System.ComponentModel.DataAnnotations;

namespace Portefeuille.Models
{
    public class actif
    {
        public int Id { get; set; }
        [Required]

        public string? Symbole { get; set; }

        public string? Nom { get; set; }

        public string? Type { get; set; }

        public string? Secteur { get; set; }

    }
}
