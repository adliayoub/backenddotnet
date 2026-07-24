using System.Text.Json.Serialization;

namespace g2soir.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Prenom { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "user";

        [JsonIgnore]
        public List<Sessionf> Sessions { get; set; } = new();
    }
}
