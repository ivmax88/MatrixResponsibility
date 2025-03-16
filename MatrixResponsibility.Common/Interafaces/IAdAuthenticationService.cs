using System.Threading.Tasks;

namespace MatrixResponsibility.Common.Interafaces
{
    public interface ILDAPAuthenticationService
    {
        Task<bool> Validate(string username, string password);
        Task<(string email, string fio)> GetFioAndEmail(string username);
    }
}
