﻿using Blazored.LocalStorage;
using MatrixResponsibility.Common.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MatrixResponsibility.Client.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        protected readonly ILocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage=localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>(str.jwttoken);
            if (string.IsNullOrEmpty(savedToken) || string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity()));
            }

            var authenticatedUser = CreateClaimsPrincipalFromToken(savedToken);

            return new AuthenticationState(authenticatedUser);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.SetItemAsStringAsync(str.jwttoken, "");
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorage.SetItemAsStringAsync(str.jwttoken, token);

            var authenticatedUser = CreateClaimsPrincipalFromToken(token);

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
        }

        protected ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = new ClaimsIdentity(); // Изначально пустая идентичность

            try
            {
                if (tokenHandler.CanReadToken(token))
                {
                    var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
                    // Создаем ClaimsIdentity с указанием AuthenticationType
                    identity = new ClaimsIdentity(jwtSecurityToken.Claims, "jwt");
                    // Проверяем, есть ли минимальные claims для аутентификации
                    if (!identity.Claims.Any(c => c.Type == ClaimTypes.Name))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Name, "Unknown")); // Минимальный claim
                    }
                }
                else
                {
                    Console.WriteLine("Token cannot be read.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JWT: {ex.Message}");
            }

            return new ClaimsPrincipal(identity);
        }
    }
}
