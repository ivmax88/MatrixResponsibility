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

        public async Task<bool> Login(string login, string password)
        {
            var token = await LoginJwt(login, password);
            if (string.IsNullOrEmpty(token) == false)
            {
                await _stateProvider.MarkUserAsAuthenticated(token);
                return true;
            }

            return false;
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


        protected async Task<string?> LoginJwt(string login, string password)
        {
            var content = JsonContent.Create(new LoginRequest(login, password));
            var request = await _httpClient.PostAsync($"{Controllers.AuthService}/{nameof(IAuthService.Login)}", content, CancellationToken.None);
            if (request.IsSuccessStatusCode)
            {
                var result = await request.Content.ReadFromJsonAsync<LoginResponse>(CancellationToken.None);
                return result?.Success==true ?  result?.Token : null;
            }
            else
            {
                var m = await request.Content.ReadAsStringAsync();
                return m;
            }
        }
    }
}
