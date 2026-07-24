using System.Text.Json.Serialization;

namespace g2soir.Models
{
    public class Formation
    {
        public int Id { get; set; }
        public string Titre { get; set; } = "";
        public string Description { get; set; } = "";
        public int IdCategorie { get; set; }

        [JsonIgnore]
        public Categorie? Categorie { get; set; }

        public List<Sessionf> Sessions { get; set; } = new();
    }
}
