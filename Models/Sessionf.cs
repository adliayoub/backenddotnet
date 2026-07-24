using System.Text.Json.Serialization;

namespace g2soir.Models
{
    public class Sessionf
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public int Duree { get; set; }
        public int IdFormation { get; set; }

        [JsonIgnore]
        public Formation? Formation { get; set; }

        public List<User> Users { get; set; } = new();
    }
}
