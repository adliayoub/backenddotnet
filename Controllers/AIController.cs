using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace g2soir.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpFactory;

        public AIController(IConfiguration config, IHttpClientFactory httpFactory)
        {
            _config = config;
            _httpFactory = httpFactory;
        }

        /// <summary>
        /// Génère un session token Anam.ai v2 (non-legacy).
        /// Le body doit inclure personaConfig avec personaId, name, avatarId, voiceId.
        /// Voir : https://docs.anam.ai/resources/migrating-legacy
        /// </summary>
        [HttpGet("anam-token")]
        public async Task<IActionResult> GetAnamSessionToken()
        {
            var apiKey    = _config["Anam:ApiKey"];
            var personaId = _config["Anam:PersonaId"];
            var personaName = _config["Anam:PersonaName"] ?? "Formateur IA";
            var avatarId  = _config["Anam:AvatarId"];
            var voiceId   = _config["Anam:VoiceId"];

            if (string.IsNullOrEmpty(apiKey) || apiKey.StartsWith("REMPLACER"))
                return BadRequest(new { message = "Anam:ApiKey manquant dans appsettings.json" });
            if (string.IsNullOrEmpty(personaId) || personaId.StartsWith("REMPLACER"))
                return BadRequest(new { message = "Anam:PersonaId manquant dans appsettings.json" });
            if (string.IsNullOrEmpty(avatarId) || avatarId.StartsWith("REMPLACER"))
                return BadRequest(new { message = "Anam:AvatarId manquant dans appsettings.json" });
            if (string.IsNullOrEmpty(voiceId) || voiceId.StartsWith("REMPLACER"))
                return BadRequest(new { message = "Anam:VoiceId manquant dans appsettings.json" });

            try
            {
                var http = _httpFactory.CreateClient();
                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Nouveau format requis par Anam.ai (non-legacy)
                // personaConfig doit être inclus dans le body du session-token
                var body = new
                {
                    clientLabel = "g2soir-backend",
                    personaConfig = new
                    {
                        personaId,
                        name     = personaName,
                        avatarId,
                        voiceId
                    }
                };

                var response = await http.PostAsync(
                    "https://api.anam.ai/v1/auth/session-token",
                    new StringContent(
                        JsonSerializer.Serialize(body),
                        Encoding.UTF8,
                        "application/json")
                );

                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode,
                        new { message = $"Erreur Anam.ai {(int)response.StatusCode} : {responseBody}" });
                }

                var json         = JsonDocument.Parse(responseBody);
                var sessionToken = json.RootElement.GetProperty("sessionToken").GetString();

                return Ok(new { sessionToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur : " + ex.Message });
            }
        }

        /// <summary>
        /// Proxy OpenAI — nécessite JWT.
        /// </summary>
        [HttpPost("ask")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Ask([FromBody] QuestionDto dto)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey.StartsWith("REMPLACER"))
                return BadRequest(new { message = "OpenAI:ApiKey manquant dans appsettings.json" });

            var systemPrompt = $@"Tu es un formateur IA expert et pédagogue sur la plateforme g2soir.
Tu aides les apprenants à comprendre les formations disponibles.
Contexte : {dto.Context ?? "Formation générale"}.
Réponds en français, de manière claire et encourageante. Maximum 3 phrases.";

            var requestBody = new
            {
                model       = "gpt-3.5-turbo",
                messages    = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user",   content = dto.Question }
                },
                max_tokens  = 300,
                temperature = 0.7
            };

            var http = _httpFactory.CreateClient();
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await http.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return StatusCode(500, new { message = "Erreur OpenAI : " + err });
            }

            var content = await response.Content.ReadAsStringAsync();
            var json    = JsonDocument.Parse(content);
            var answer  = json.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { answer });
        }
    }

    public class QuestionDto
    {
        public string Question { get; set; } = "";
        public string? Context { get; set; }
    }
}
