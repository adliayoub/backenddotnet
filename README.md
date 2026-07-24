# g2soir — Backend ASP.NET Core Web API

## Prérequis
- .NET 8 SDK → https://dotnet.microsoft.com/download/dotnet/8.0
- Extension VSCode : C# Dev Kit
- `dotnet tool install --global dotnet-ef`

## ⚙️ Configuration appsettings.json

```json
{
  "OpenAI": { "ApiKey": "sk-..." },
  "Anam": {
    "ApiKey":      "votre_api_key",
    "PersonaId":   "id_du_persona",
    "PersonaName": "Formateur IA",
    "AvatarId":    "id_de_l_avatar",
    "VoiceId":     "id_de_la_voix"
  }
}
```

### Où trouver les valeurs Anam.ai (anam.ai → Dashboard) :
| Champ         | Chemin dans le dashboard         |
|---------------|----------------------------------|
| ApiKey        | Settings → API Keys              |
| PersonaId     | Personas → votre persona → ID    |
| PersonaName   | Le nom de votre persona          |
| AvatarId      | Avatars → votre avatar → ID      |
| VoiceId       | Voices → votre voix → ID         |

## 🚀 Lancer
```bash
dotnet restore
dotnet ef migrations add InitialCreate   # 1ère fois seulement
dotnet ef database update                # 1ère fois seulement
dotnet run
```
→ http://localhost:5000/swagger
