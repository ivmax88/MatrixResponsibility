using MatrixResponsibility.Client.Services;
using MatrixResponsibility.Common;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace MatrixResponsibility.Client.Pages
{
    public partial class Home
    {
        [Inject] public MainHubService MainHubService { get; set; }

        private List<Project>? projects;

        protected override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            MainHubService.OnProjectChangedAsync += HandleProjectChangedAsync;
            await MainHubService.InitializeAsync(cancellationToken);

            projects = await MainHubService.GetAllProjects(cancellationToken);
        }

        private async Task HandleProjectChangedAsync(Project updatedProject)
        {
            if (projects != null)
            {
                var existingProject = projects.FirstOrDefault(p => p.Id == updatedProject.Id);
                if (existingProject != null)
                {
                    var index = projects.IndexOf(existingProject);
                    projects[index] = updatedProject;
                }
                else
                {
                    projects.Add(updatedProject);
                }
                await Task.Delay(1); // Даем UI возможность обновиться
                await InvokeAsync(StateHasChanged);
            }
        }

        protected override async Task DisposeResourcesAsync()
        {
            MainHubService.OnProjectChangedAsync -= HandleProjectChangedAsync;
            await MainHubService.StopAsync(CancellationToken);
        }
    }
}
