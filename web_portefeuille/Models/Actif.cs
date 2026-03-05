namespace web_portefeuille.Models
{
    public class Actif
    {
        public int Id { get; set; }
        public string? Symbole { get; set; }
        public string? Nom { get; set; }
        public string? Type { get; set; }      // "Action" ou "Crypto"
        public string? Secteur { get; set; }
    }
}
