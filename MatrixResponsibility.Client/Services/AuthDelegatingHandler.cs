using Blazored.LocalStorage;
using MatrixResponsibility.Common.Constants;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace MatrixResponsibility.Client.Services
{
    public class AuthDelegatingHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _nav;

        public AuthDelegatingHandler(ILocalStorageService localStorage, NavigationManager nav)
        {
            this._localStorage=localStorage;
            this._nav=nav;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var jwtToken = await _localStorage.GetItemAsync<string>(str.jwttoken, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var result = await base.SendAsync(request, cancellationToken);

            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _nav.NavigateTo("/logout");
            }
            return result;
        }
    }
}
