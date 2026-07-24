using g2soir.Infra;
using g2soir.Models;

namespace g2soir.Services
{
    public class VService : IServices
    {
        IDao dao;
        public VService(IDao dao) { this.dao = dao; }

        public void Add(User entity) => dao.Add(entity);
        public void Update(User entity) => dao.Update(entity);
        public User? GetUserById(int id) => dao.GetUserById(id);
        public List<User> GetAllUsers() => dao.GetAllUsers();
        public User? GetUserByEmail(string email) => dao.GetUserByEmail(email);

        public User? authentificat(string email, string password)
        {
            var user = dao.GetUserByEmail(email);
            if (user != null && user.Password == password) return user;
            return null;
        }

        public void AddCat(Categorie entity) => dao.AddCat(entity);
        public void UpdateCat(int id, Categorie entity) => dao.UpdateCat(id, entity);
        public void DeleteCatById(int id) => dao.DeleteCatById(id);
        public Categorie? GetCatById(int id) => dao.GetCatById(id);
        public List<Categorie> GetAll() => dao.GetAll();

        public void AddFormation(int idcat, Formation entity) => dao.AddFormation(idcat, entity);
        public void UpdateFormation(int id, Formation entity) => dao.UpdateFormation(id, entity);
        public void DeleteFormationById(int id) => dao.DeleteFormationById(id);
        public Formation? GetFormationById(int id) => dao.GetFormationById(id);
        public List<Formation> GetAllFormations() => dao.GetAllFormations();
        public List<Formation> GetFormationByName(string name) => dao.GetFormationByName(name);

        public void AddSession(int idformation, Sessionf entity)
        {
            var f = dao.GetFormationById(idformation);
            if (f == null) throw new Exception("Formation introuvable");
            dao.AddSession(idformation, entity);
        }
        public void UpdateSession(int id, Sessionf entity) => dao.UpdateSession(id, entity);
        public void DeleteSessionById(int id) => dao.DeleteSessionById(id);
        public Sessionf? GetSessionById(int id) => dao.GetSessionById(id);
        public List<Sessionf> GetAllSessions() => dao.GetAllSessions();
        public List<Sessionf> GetAllSessions(int idformation) => dao.GetAllSessions(idformation);
        public List<Sessionf> GetAllSessions(DateTime debut, DateTime fin) => dao.GetAllSessions(debut, fin);

        public void AddInscription(int idsession, int iduser)
        {
            var s = dao.GetSessionById(idsession);
            var u = dao.GetUserById(iduser);
            if (s == null) throw new Exception("Session introuvable");
            if (u == null) throw new Exception("Utilisateur introuvable");
            dao.AddInscription(idsession, iduser);
        }
        public void DeleteInscription(int idsession, int iduser) => dao.DeleteInscription(idsession, iduser);
        public List<User> GetAllUsersInSession(int idsession) => dao.GetAllUsersInSession(idsession);
        public List<Sessionf> GetAllSessionsForUser(int iduser) => dao.GetAllSessionsForUser(iduser);
    }
}
