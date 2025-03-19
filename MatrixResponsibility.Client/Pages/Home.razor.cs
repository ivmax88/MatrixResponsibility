using MatrixResponsibility.Client.Services;
using MatrixResponsibility.Common.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MatrixResponsibility.Client.Pages
{
    public partial class Home
    {
        [Inject] public MainHubService MainHubService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private ObservableCollection<ProjectDTO>? projects;
        private List<UserDTO> users;


        private RadzenDataGrid<ProjectDTO> grid;
        private string columnEditing;
        private ProjectDTO projectEditingNew;
        private ProjectDTO projectEditingOld;
        private IRadzenFormComponent editor;

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

        private async Task OnUpdateRow(ProjectDTO project)
        {
            await MainHubService.ChangeProjectInfo(project);
        }

        async Task OnCellDblClick(DataGridCellMouseEventArgs<ProjectDTO> args)
        {
            columnEditing = args.Column.Property;
            projectEditingNew = args.Data;
            projectEditingOld = JsonSerializer.Deserialize<ProjectDTO>(JsonSerializer.Serialize(projectEditingNew));
            await grid.EditRow(args.Data);
            await Task.Yield();
            await editor.FocusAsync();

            // Добавляем слушатель Escape через JS
            await JSRuntime.InvokeVoidAsync("addEscapeListener", DotNetObjectReference.Create(this));
            await JSRuntime.InvokeVoidAsync("addEnterListener", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async Task OnEscapePressed()
        {
            await Cancel();
        }
        [JSInvokable]
        public async Task OnEnterPressed()
        {
            await Update();
        }

        bool IsEditing(string columnName, ProjectDTO project)
        {
            return columnEditing == columnName && projectEditingNew==project;
        }

        async Task Update()
        {
            await grid.UpdateRow(projectEditingNew);
        }

        async Task Cancel()
        {
            var existProject = projects?.FirstOrDefault(p => p.Id == projectEditingOld?.Id);
            if (existProject==null) return;
            var index = projects?.IndexOf(existProject) ?? -1;
            if (index == -1) return;
            if (projects==null) return;
            projects[index] = projectEditingOld;
            grid.CancelEditRow(projectEditingOld);
        }

//        async Task KeyPressed(KeyboardEventArgs args)
//        {
//#if DEBUG
//            Console.WriteLine($"Key pressed: {args.Key}"); // Для отладки
//#endif
//            if (args.Key == "Enter")
//                await Update();
//            else if (args.Key == "Escape")
//                await Cancel();
//        }

        protected override async Task DisposeResourcesAsync()
        {
            MainHubService.OnProjectChanged -= HandleProjectChangedAsync;
            await MainHubService.StopAsync(CancellationToken);
            await MainHubService.DisposeAsync();
        }
    }
}