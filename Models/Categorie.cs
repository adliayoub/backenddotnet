using System.Text.Json.Serialization;

namespace g2soir.Models
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Description { get; set; } = "";
        public List<Formation> Formations { get; set; } = new();
    }
}
