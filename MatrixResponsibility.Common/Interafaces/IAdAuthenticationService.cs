using System.Threading.Tasks;

namespace MatrixResponsibility.Common.Interafaces
{
    public interface ILDAPAuthenticationService
    {
        Task<bool> Validate(string username, string password);
    }
}
