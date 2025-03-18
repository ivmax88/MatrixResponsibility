using Blazored.LocalStorage;
using MatrixResponsibility.Client;
using MatrixResponsibility.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var name = builder.HostEnvironment.Environment;
builder.Configuration.AddJsonFile($"appsettings.{name}.json", optional: true);
var apiUrl = builder.Configuration["ApiUrl"];
Console.WriteLine(apiUrl);

builder.Services.AddLogging(l =>
{
    l.SetMinimumLevel(LogLevel.Information);
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<MainHubService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddTransient<AuthDelegatingHandler>();

// Настройка HttpClient для API
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri($@"{apiUrl}/api/");
})
.AddHttpMessageHandler<AuthDelegatingHandler>();

// Регистрация HttpClient для использования в компонентах
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

builder.Services.AddRadzenComponents();

// Настройка аутентификации
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();


await builder.Build().RunAsync();
