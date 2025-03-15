using MatrixResponsibility.Common.Interafaces;
using MatrixResponsibility.Data;
using MatrixResponsibility.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region DbContext
#if DEBUG
var connection = "Host=localhost;Port=5432;Database=matrix_db_test;Username=user;Password=pass";
#else
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
#endif
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connection));
#endregion

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient<ILDAPAuthenticationService,LDAPAuthenticationService>(client =>
{
    client.BaseAddress = new Uri(builder?.Configuration["LDAPSettings:Url"] ?? throw new NullReferenceException("LDAP url is null [LDAPSettings:Url]"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();

#region jwt
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new NullReferenceException("jwt key is null"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //ClockSkew = TimeSpan.FromSeconds(1),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
#endregion

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите токен JWT."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
#endregion

#region Cors
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", b =>
    {
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
#endregion

var app = builder.Build();

// Применение миграций с логированием
updatedatabase();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void updatedatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (int i = 0; i < maxAttempts; i++)
        {
            try
            {
                logger.LogInformation("Applying migrations...");
                dbContext.Database.Migrate();
                logger.LogInformation("Migrations applied successfully.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to apply migrations, retrying in {Delay} seconds... ({Attempt}/{MaxAttempts})", delay.TotalSeconds, i + 1, maxAttempts);
                if (i == maxAttempts - 1) throw;
                Thread.Sleep(delay);
            }
        }
    }
}

