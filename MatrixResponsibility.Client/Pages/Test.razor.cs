using MatrixResponsibility.Client.Services;
using MatrixResponsibility.Common.DTOs;
using MatrixResponsibility.Common;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;

namespace MatrixResponsibility.Client.Pages
{
    public partial class Test
    {
        [Inject] public MainHubService MainHubService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private ObservableCollection<ProjectDTO>? projects;
        private List<UserDTO> users;
        protected override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await MainHubService.InitializeAsync(cancellationToken);
            MainHubService.OnProjectChanged += HandleProjectChangedAsync;

            var tempProjects = await MainHubService.GetAllProjects(cancellationToken);
            projects = new ObservableCollection<ProjectDTO>(tempProjects);

            users = await MainHubService.GetAllUsers(cancellationToken);

        }

        private async Task HandleProjectChangedAsync(ProjectDTO updatedProject)
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
            }
        }

        protected override async Task DisposeResourcesAsync()
        {
            MainHubService.OnProjectChanged -= HandleProjectChangedAsync;
            await MainHubService.StopAsync(CancellationToken);
            await MainHubService.DisposeAsync();
        }
    }
}
