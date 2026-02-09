using System.ComponentModel.DataAnnotations;

namespace Portefeuille.Models
{
    public class Allocation
    {
        public int Id { set; get; }
        [Required]

        public float Poids { set; get; }

        public float Montant { set; get; }

        public float Prixpredit { set; get; }
    }
}
