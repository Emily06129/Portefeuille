using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portefeuille.Models
{
    public class Allocation
    {
        public int Id { set; get; }
        [Required]

        public float Poids { set; get; }

        public float Montant { set; get; }

        public float Prixpredit { set; get; }

        [ForeignKey("Actif")]
        public int ActifId { get; set; }

        [ForeignKey("Portfolio_client")]
        public int Portfolio_clientId { get; set; }


    }
}
