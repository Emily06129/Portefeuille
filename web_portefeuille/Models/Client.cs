using System.ComponentModel.DataAnnotations;

namespace web_portefeuille.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
