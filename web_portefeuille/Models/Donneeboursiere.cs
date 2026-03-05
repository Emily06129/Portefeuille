namespace web_portefeuille.Models
{
    public class Donneeboursiere
    {
        public int Id { get; set; }
        public float Cloture { get; set; }
        public float Volume { get; set; }
        public DateTime Date { get; set; }
        public int ActifId { get; set; }
    }
}
