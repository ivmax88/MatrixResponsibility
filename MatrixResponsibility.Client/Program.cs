using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MatrixResponsibility.Client;
using Blazored.LocalStorage;
using MatrixResponsibility.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddLogging(l =>
{
    l.SetMinimumLevel(LogLevel.Information);
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<MainHubService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddTransient<AuthDelegatingHandler>();
// Настройка HttpClient для API
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5102/api/");
})
.AddHttpMessageHandler<AuthDelegatingHandler>();

// Регистрация HttpClient для использования в компонентах
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

// Настройка аутентификации
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();


await builder.Build().RunAsync();
