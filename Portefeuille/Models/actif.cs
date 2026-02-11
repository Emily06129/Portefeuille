using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Portefeuille.Models
{
    public class Actif
    {
        public int Id { get; set; }
        [Required]

        public string? Symbole { get; set; }

        public string? Nom { get; set; }

        public string? Type { get; set; }

        public string? Secteur { get; set; }

        public virtual ICollection<Donneeboursiere>? ListeDonneeBoursieres { get; set; }

        public virtual ICollection<Allocation>? ListeAllocations { get; set; }

    }
}
