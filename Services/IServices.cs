using g2soir.Models;

namespace g2soir.Services
{
    public interface IServices
    {
        void Add(User entity);
        void Update(User entity);
        User? GetUserById(int id);
        List<User> GetAllUsers();
        User? GetUserByEmail(string email);
        User? authentificat(string email, string password);

        void AddCat(Categorie entity);
        void UpdateCat(int id, Categorie entity);
        void DeleteCatById(int id);
        Categorie? GetCatById(int id);
        List<Categorie> GetAll();

        void AddFormation(int idcat, Formation entity);
        void UpdateFormation(int id, Formation entity);
        void DeleteFormationById(int id);
        Formation? GetFormationById(int id);
        List<Formation> GetAllFormations();
        List<Formation> GetFormationByName(string name);

        void AddSession(int idformation, Sessionf entity);
        void UpdateSession(int id, Sessionf entity);
        void DeleteSessionById(int id);
        Sessionf? GetSessionById(int id);
        List<Sessionf> GetAllSessions();
        List<Sessionf> GetAllSessions(int idformation);
        List<Sessionf> GetAllSessions(DateTime debut, DateTime fin);

        void AddInscription(int idsession, int iduser);
        void DeleteInscription(int idsession, int iduser);
        List<User> GetAllUsersInSession(int idsession);
        List<Sessionf> GetAllSessionsForUser(int iduser);
    }
}
