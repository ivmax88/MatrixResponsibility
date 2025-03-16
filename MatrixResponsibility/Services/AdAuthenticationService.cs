using MatrixResponsibility.Common;
using MatrixResponsibility.Common.Interafaces;
using System.Formats.Asn1;
using System.Text.Json;

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

        public async Task<(string email, string fio)> GetFioAndEmail(string username)
        {
            var request = await httpClient.GetAsync($"LDAPService/GetEmployee_cn_mail_BySAMAccountName?sAMAccountName={username}");
            if (request.IsSuccessStatusCode)
            {
                var result = await request.Content.ReadFromJsonAsync<userinfo>()
                    ??throw new Exception($"Не получилось получить данные по логину {username}");

                return (result.email, result.commonName);
            }
            else
            {
                throw new Exception($"Не получилось получить данные по логину {username}");
            }
        }
        record userinfo (string commonName, string email);
    }
}
