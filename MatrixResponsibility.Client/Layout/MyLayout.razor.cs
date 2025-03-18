using MatrixResponsibility.Client.Services;
using Microsoft.AspNetCore.Components;

namespace MatrixResponsibility.Client.Layout
{
    public partial class MyLayout
    {
        private bool sidebar1Expanded = true;
        private string userName;
        private int onlineCount;

        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public UserService UserService { get; set; }
        [Inject] public MainHubService MainHubService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            userName = await UserService.GetUserNameAsync();
            if (MainHubService!=null)
            {
                MainHubService.OnConnectedClientsCountChanged+=HandleCahngedChangedAsync;
            }
        }

        private async Task HandleCahngedChangedAsync(int count)
        {
            onlineCount = count;
            await InvokeAsync(StateHasChanged);
        }

    }
}
