using g2soir.Models;
using g2soir.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g2soir.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogueController : ControllerBase
    {
        private readonly IServices _service;
        public CatalogueController(IServices service) { _service = service; }

        // CATEGORIES
        [HttpGet("categories")]
        public IActionResult GetAllCategories() => Ok(_service.GetAll());

        [HttpGet("categories/{id}")]
        public IActionResult GetCategorieById(int id)
        {
            var cat = _service.GetCatById(id);
            return cat == null ? NotFound() : Ok(cat);
        }

        [HttpPost("categories")]
        [Authorize]
        public IActionResult AddCategorie([FromBody] Categorie cat)
        {
            _service.AddCat(cat);
            return Ok(cat);
        }

        [HttpPut("categories/{id}")]
        [Authorize]
        public IActionResult UpdateCategorie(int id, [FromBody] Categorie cat)
        {
            _service.UpdateCat(id, cat);
            return Ok();
        }

        [HttpDelete("categories/{id}")]
        [Authorize]
        public IActionResult DeleteCategorie(int id)
        {
            _service.DeleteCatById(id);
            return Ok();
        }

        // FORMATIONS
        [HttpGet("formations")]
        public IActionResult GetAllFormations()
        {
            var formations = _service.GetAllFormations();
            // Project category info since Formation.Categorie has [JsonIgnore]
            var result = formations.Select(f => new
            {
                f.Id,
                f.Titre,
                f.Description,
                f.IdCategorie,
                Categorie = f.Categorie != null ? new { f.Categorie.Id, f.Categorie.Nom, f.Categorie.Description } : null,
                Sessions = f.Sessions.Select(s => new { s.Id, s.DateDebut, s.Duree, s.IdFormation }).ToList()
            });
            return Ok(result);
        }

        [HttpGet("formations/{id}")]
        public IActionResult GetFormationById(int id)
        {
            var f = _service.GetFormationById(id);
            if (f == null) return NotFound();
            // Project category and sessions info
            var result = new
            {
                f.Id,
                f.Titre,
                f.Description,
                f.IdCategorie,
                Categorie = f.Categorie != null ? new { f.Categorie.Id, f.Categorie.Nom, f.Categorie.Description } : null,
                Sessions = f.Sessions.Select(s => new { s.Id, s.DateDebut, s.Duree, s.IdFormation }).ToList()
            };
            return Ok(result);
        }

        [HttpPost("categories/{idcat}/formations")]
        [Authorize]
        public IActionResult AddFormation(int idcat, [FromBody] Formation formation)
        {
            formation.IdCategorie = idcat;
            _service.AddFormation(idcat, formation);
            return Ok(new { formation.Id, formation.Titre, formation.Description, formation.IdCategorie });
        }

        [HttpPut("formations/{id}")]
        [Authorize]
        public IActionResult UpdateFormation(int id, [FromBody] Formation formation)
        {
            _service.UpdateFormation(id, formation);
            return Ok();
        }

        [HttpDelete("formations/{id}")]
        [Authorize]
        public IActionResult DeleteFormation(int id)
        {
            _service.DeleteFormationById(id);
            return Ok();
        }

        // SESSIONS
        [HttpGet("sessions")]
        public IActionResult GetAllSessions()
        {
            var sessions = _service.GetAllSessions();
            var result = sessions.Select(s => new
            {
                s.Id,
                s.DateDebut,
                s.Duree,
                s.IdFormation,
                Formation = s.Formation != null ? new { s.Formation.Id, s.Formation.Titre, s.Formation.Description } : null
            });
            return Ok(result);
        }

        [HttpGet("formations/{idformation}/sessions")]
        public IActionResult GetSessionsByFormation(int idformation)
            => Ok(_service.GetAllSessions(idformation));

        [HttpGet("sessions/{id}")]
        public IActionResult GetSessionById(int id)
        {
            var s = _service.GetSessionById(id);
            if (s == null) return NotFound();
            var result = new
            {
                s.Id,
                s.DateDebut,
                s.Duree,
                s.IdFormation,
                Formation = s.Formation != null ? new { s.Formation.Id, s.Formation.Titre, s.Formation.Description } : null,
                Users = s.Users.Select(u => new { u.Id, u.Nom, u.Prenom, u.Email }).ToList()
            };
            return Ok(result);
        }

        [HttpPost("formations/{idformation}/sessions")]
        [Authorize]
        public IActionResult AddSession(int idformation, [FromBody] Sessionf session)
        {
            session.IdFormation = idformation;
            _service.AddSession(idformation, session);
            return Ok(new { session.Id, session.DateDebut, session.Duree, session.IdFormation });
        }

        [HttpDelete("sessions/{id}")]
        [Authorize]
        public IActionResult DeleteSession(int id)
        {
            _service.DeleteSessionById(id);
            return Ok();
        }

        // INSCRIPTIONS
        [HttpPost("sessions/{idsession}/users/{iduser}")]
        [Authorize]
        public IActionResult AddInscription(int idsession, int iduser)
        {
            _service.AddInscription(idsession, iduser);
            return Ok();
        }

        [HttpDelete("sessions/{idsession}/users/{iduser}")]
        [Authorize]
        public IActionResult DeleteInscription(int idsession, int iduser)
        {
            _service.DeleteInscription(idsession, iduser);
            return Ok();
        }

        [HttpGet("sessions/{idsession}/users")]
        public IActionResult GetUsersInSession(int idsession)
            => Ok(_service.GetAllUsersInSession(idsession));

        [HttpGet("users/{iduser}/sessions")]
        [Authorize]
        public IActionResult GetSessionsForUser(int iduser)
        {
            var sessions = _service.GetAllSessionsForUser(iduser);
            var result = sessions.Select(s => new
            {
                s.Id,
                s.DateDebut,
                s.Duree,
                s.IdFormation,
                Formation = s.Formation != null ? new { s.Formation.Id, s.Formation.Titre, s.Formation.Description } : null
            });
            return Ok(result);
        }
    }
}
