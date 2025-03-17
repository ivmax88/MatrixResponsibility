using MatrixResponsibility.Common.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using MatrixResponsibility.Client.Services;

namespace MatrixResponsibility.Client.Pages
{
    public partial class Login
    {
        private string? login;
        private string? password;
        private bool showpassword = true;
        [Inject] public AuthService AuthService { get; set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var l = await LocalStorage.GetItemAsync<string>(str.login, cancellationToken);
            if (string.IsNullOrEmpty(l)==false)
            {
                login = l;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void TogglePassword()
        {
            showpassword = !showpassword;
        }

        private async Task LoginBtn()
        {
            await LocalStorage.SetItemAsync<string>(str.login, login, CancellationToken);

            if (await AuthService.Login(login, password, CancellationToken))
            {
                Navigation.NavigateTo($"/");
            }
            else
            {
                await DialogService.Alert("Неверный логин или пароль", "Ошибка", AlertOptions);
            }
        }

        private async Task PasswordInputKeyPressed(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await LoginBtn();
            }
        }
    }
}
