using System.ComponentModel.DataAnnotations;

namespace Portefeuille.Models
{
    public class client
    {
        public int Id { get; set; }
        [Required]

        public string? Nom { get; set; }
        public string? Email { get; set; }
        public float? Budget { get; set; }


    }
}
