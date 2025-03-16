using Blazored.LocalStorage;
using MatrixResponsibility.Common;
using MatrixResponsibility.Common.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace MatrixResponsibility.Client.Pages
{
    public partial class Home
    {
        private HubConnection connection;
        string message;
        private IEnumerable<User> users;

        [Inject] public ILocalStorageService? localStorage { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (localStorage==null)
                return;

            var token = await localStorage.GetItemAsync<string>(str.jwttoken);
            connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:5102/hubs/main?{str.access_token}={token}", async opt =>
                {

                })
                .Build();

            // регистрируем функцию Receive для получения данных
            connection.On<string>("all", (message) =>
            {
                this.message = message;
            });

            await connection.StartAsync();
            //users = await connection.InvokeAsync<IEnumerable<User>>("Get");
        }
    }
}
