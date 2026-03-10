namespace web_portefeuille.Models
{
    public class InvestirViewModel
    {
        public int Id { get; set; }
        public float Budget { get; set; }
        public List<string> SymbolesActifs { get; set; } = new();

    }
}
