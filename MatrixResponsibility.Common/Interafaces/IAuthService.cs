using MatrixResponsibility.Common.DTOs;
using MatrixResponsibility.Common.DTOs.Response;
using olp.ControllerGenerator.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixResponsibility.Common.Interafaces
{
    [AutoGen]
    public interface IAuthService
    {
        [PostGen(nameof(Login))]
        Task<LoginResponse> Login(LoginRequest model, CancellationToken ct);
    }
}
