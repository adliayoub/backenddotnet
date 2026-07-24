using g2soir.Models;
using Microsoft.EntityFrameworkCore;

namespace g2soir.Infra
{
    public class DaoImpl : IDao
    {
        private readonly MyContext _context;
        public DaoImpl(MyContext context) { _context = context; }

        // USER
        public void Add(User entity) { _context.Users.Add(entity); _context.SaveChanges(); }
        public void Update(User entity) { _context.Users.Update(entity); _context.SaveChanges(); }
        public User? GetUserById(int id) => _context.Users.FirstOrDefault(u => u.Id == id);
        public List<User> GetAllUsers() => _context.Users.ToList();
        public User? GetUserByName(string name) => _context.Users.FirstOrDefault(u => u.Nom == name);
        public User? GetUserByEmail(string email) => _context.Users.FirstOrDefault(u => u.Email == email);

        // CATEGORIE
        public void AddCat(Categorie entity) { _context.Categories.Add(entity); _context.SaveChanges(); }
        public void UpdateCat(int id, Categorie entity)
        {
            var cat = _context.Categories.Find(id);
            if (cat != null) { cat.Nom = entity.Nom; cat.Description = entity.Description; _context.SaveChanges(); }
        }
        public void DeleteCatById(int id)
        {
            var cat = _context.Categories.Find(id);
            if (cat != null) { _context.Categories.Remove(cat); _context.SaveChanges(); }
        }
        public Categorie? GetCatById(int id) => _context.Categories.Include(c => c.Formations).FirstOrDefault(c => c.Id == id);
        public List<Categorie> GetAll() => _context.Categories.Include(c => c.Formations).ToList();

        // FORMATION
        public void AddFormation(int idcat, Formation entity)
        {
            entity.IdCategorie = idcat; _context.Formations.Add(entity); _context.SaveChanges();
        }
        public void UpdateFormation(int id, Formation entity)
        {
            var f = _context.Formations.Find(id);
            if (f != null) { f.Titre = entity.Titre; f.Description = entity.Description; _context.SaveChanges(); }
        }
        public void DeleteFormationById(int id)
        {
            var f = _context.Formations.Find(id);
            if (f != null) { _context.Formations.Remove(f); _context.SaveChanges(); }
        }
        public Formation? GetFormationById(int id) => _context.Formations.Include(f => f.Categorie).Include(f => f.Sessions).FirstOrDefault(f => f.Id == id);
        public List<Formation> GetAllFormations() => _context.Formations.Include(f => f.Categorie).Include(f => f.Sessions).ToList();
        public List<Formation> GetFormationByName(string name) => _context.Formations.Where(f => f.Titre.Contains(name)).ToList();

        // SESSION
        public void AddSession(int idformation, Sessionf entity)
        {
            entity.IdFormation = idformation; _context.Sessions.Add(entity); _context.SaveChanges();
        }
        public void UpdateSession(int id, Sessionf entity)
        {
            var s = _context.Sessions.Find(id);
            if (s != null) { s.DateDebut = entity.DateDebut; s.Duree = entity.Duree; _context.SaveChanges(); }
        }
        public void DeleteSessionById(int id)
        {
            var s = _context.Sessions.Find(id);
            if (s != null) { _context.Sessions.Remove(s); _context.SaveChanges(); }
        }
        public Sessionf? GetSessionById(int id) => _context.Sessions.Include(s => s.Formation).Include(s => s.Users).FirstOrDefault(s => s.Id == id);
        public List<Sessionf> GetAllSessions() => _context.Sessions.Include(s => s.Formation).ToList();
        public List<Sessionf> GetAllSessions(int idformation) => _context.Sessions.Where(s => s.IdFormation == idformation).ToList();
        public List<Sessionf> GetAllSessions(DateTime debut, DateTime fin) => _context.Sessions.Where(s => s.DateDebut >= debut && s.DateDebut <= fin).ToList();

        // INSCRIPTION
        public void AddInscription(int idsession, int iduser)
        {
            var session = _context.Sessions.Include(s => s.Users).FirstOrDefault(s => s.Id == idsession);
            var user = _context.Users.Find(iduser);
            if (session != null && user != null) { session.Users.Add(user); _context.SaveChanges(); }
        }
        public void DeleteInscription(int idsession, int iduser)
        {
            var session = _context.Sessions.Include(s => s.Users).FirstOrDefault(s => s.Id == idsession);
            if (session != null)
            {
                var user = session.Users.FirstOrDefault(u => u.Id == iduser);
                if (user != null) { session.Users.Remove(user); _context.SaveChanges(); }
            }
        }
        public List<User> GetAllUsersInSession(int idsession)
        {
            var session = _context.Sessions.Include(s => s.Users).FirstOrDefault(s => s.Id == idsession);
            return session?.Users ?? new List<User>();
        }
        public List<Sessionf> GetAllSessionsForUser(int iduser)
        {
            var user = _context.Users.Include(u => u.Sessions).ThenInclude(s => s.Formation).FirstOrDefault(u => u.Id == iduser);
            return user?.Sessions ?? new List<Sessionf>();
        }
    }
}
