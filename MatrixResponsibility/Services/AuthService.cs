using MatrixResponsibility.Common.DTOs;
using MatrixResponsibility.Common.DTOs.Response;
using MatrixResponsibility.Common.Interafaces;
using MatrixResponsibility.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MatrixResponsibility.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILDAPAuthenticationService ldapService;
        private readonly AppDbContext dbContext;
        private readonly IConfiguration configuration;

        public AuthService(ILDAPAuthenticationService ldapService,
            AppDbContext dbContext,
            IConfiguration configuration)
        {
            this.ldapService=ldapService;
            this.dbContext=dbContext;
            this.configuration=configuration;
        }
        public async Task<LoginResponse> Login(LoginRequest model, CancellationToken ct)
        {
            // Проверка учетных данных через AD
            var isValid = await ldapService.Validate(model.username, model.password);
            if (!isValid)
            {
                return new LoginResponse(false, null);
            }

            // Поиск пользователя в локальной базе по логину
            var localUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Login == model.username);

            if (localUser == null)
            {
                return new LoginResponse(false, null);
            }

            // Получение ролей из UserRole
            var userRoles = await dbContext.UserRoles
                .Where(ur => ur.UserId == localUser.Id)
                .Join(dbContext.Roles,
                      ur => ur.RoleId,
                      r => r.Id,
                      (ur, r) => r.Name)
                .ToListAsync();

            // Создание claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, localUser.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Проверка значения ExpiryInDays
            if (!double.TryParse(configuration["JwtSettings:ExpiryInDays"], out double expiryInDays))
            {
                expiryInDays = 1; // Значение по умолчанию (1 день)
            }

            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new NullReferenceException("jwt key is null"));

            // Генерация JWT-токена
            var authSigningKey = new SymmetricSecurityKey(key);
            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(expiryInDays),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new LoginResponse(true, new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
