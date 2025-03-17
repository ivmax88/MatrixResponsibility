using MatrixResponsibility.Common.Constants;
using MatrixResponsibility.Common.DTOs;
using MatrixResponsibility.Common.DTOs.Response;
using MatrixResponsibility.Common.Interafaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace MatrixResponsibility.Client.Services
{
    public class AuthService
    {
        private readonly CustomAuthenticationStateProvider _stateProvider;
        private readonly HttpClient _httpClient;

        public AuthService
            (
            HttpClient httpClient,
            AuthenticationStateProvider stateProvider
            )
        {
            _stateProvider = (CustomAuthenticationStateProvider)stateProvider;
            _httpClient = httpClient;
        }

        public async Task<bool> Login(string login, string password, CancellationToken ct)
        {
            var result = await LoginJwt(login, password, ct);
            if (result == null || result.Success == false || result.Token==null) return false;

            await _stateProvider.MarkUserAsAuthenticated(result.Token);

            return true;
        }

        public async Task<bool> Logout()
        {
            try
            {
                await _stateProvider.MarkUserAsLoggedOut();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        protected async Task<LoginResponse?> LoginJwt(string login, string password, CancellationToken ct)
        {
            var content = JsonContent.Create(new LoginRequest(login, password));
            var request = await _httpClient.PostAsync($"{Controllers.AuthService}/{nameof(IAuthService.Login)}", content, ct);
            if (request.IsSuccessStatusCode)
            {
                var result = await request.Content.ReadFromJsonAsync<LoginResponse>(ct);
                return result;
            }
            else
            {
                return new LoginResponse(false, null);
            }
        }
    }
}
