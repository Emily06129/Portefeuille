using System.ComponentModel.DataAnnotations;

namespace Portefeuille.Models
{
    public class donneeboursiere
    {
        public int Id { get; set; }
        [Required]

        public float? Cloture { get; set; }

        public float? Volume { get; set; }

        public DateTime dateTime { get; set; }

    }
}
