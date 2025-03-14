using MatrixResponsibility.Common.Interafaces;

namespace MatrixResponsibility.Services
{
    public class LDAPAuthenticationService : ILDAPAuthenticationService
    {
        private readonly HttpClient httpClient;

        public LDAPAuthenticationService(HttpClient httpClient)
        {
            this.httpClient=httpClient;
        }
        public async Task<bool> Validate(string username, string password)
        {
            var request = await httpClient.PostAsJsonAsync("LDAPService/Login", new { username, password });
            if (request.IsSuccessStatusCode)
                return await request.Content.ReadFromJsonAsync<bool>();

            return false;
        }
    }
}
