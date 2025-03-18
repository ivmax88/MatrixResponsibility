using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MatrixResponsibility.Client.Services
{
    public class UserService
    {
        private readonly AuthenticationStateProvider authStateProvider;

        public UserService(AuthenticationStateProvider authStateProvider)
        {
            this.authStateProvider = authStateProvider;
        }

        public async Task<ClaimsPrincipal> GetCurrentUserAsync()
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            return authState.User;
        }

        public async Task<string> GetUserNameAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Identity?.Name ?? "Anonymous";
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Identity?.IsAuthenticated == true;
        }
    }
}