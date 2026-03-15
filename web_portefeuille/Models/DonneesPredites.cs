namespace web_portefeuille.Models
{
    public class DonneesPredites
    {
        public int Id { get; set; }
        public int ActifId { get; set; }
        public string Symbole { get; set; }
        public DateTime DatePrediction { get; set; }
        public float PrixPredit { get; set; }
        public DateTime DateCreation { get; set; }
    }
}