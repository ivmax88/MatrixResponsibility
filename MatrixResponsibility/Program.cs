using MatrixResponsibility.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


#if DEBUG
var connection = "Host=localhost;Port=5432;Database=matrix_db_test;Username=user;Password=pass";
#else
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
#endif
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connection));

builder.Services.AddControllers();

// Добавляем поддержку Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", b =>
    {
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Применение миграций с логированием
updatedatabase();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
