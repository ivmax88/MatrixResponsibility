using MatrixResponsibility.Client.Services;
using MatrixResponsibility.Common;
using MatrixResponsibility.Common.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Collections.ObjectModel;

namespace MatrixResponsibility.Client.Pages
{
    public partial class Home
    {
        [Inject] public MainHubService MainHubService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private ObservableCollection<ProjectDTO>? projects;
        private List<UserDTO> users;
        private RadzenDataGrid<ProjectDTO> grid;
        private DotNetObjectReference<Home> dotNetObjRef;
        private ProjectDTO currentEditedProject; // Одиночный объект для текущего редактируемого проекта
        private string columnEditing;
        private IRadzenFormComponent editor; //  для работы с RadzenTextBox


        protected override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            dotNetObjRef = DotNetObjectReference.Create(this);
            await MainHubService.InitializeAsync(cancellationToken);
            MainHubService.OnProjectChanged += HandleProjectChangedAsync;

            var tempProjects = await MainHubService.GetAllProjects(cancellationToken);
            projects = new ObservableCollection<ProjectDTO>(tempProjects);

            users = await MainHubService.GetAllUsers(cancellationToken);

            await JSRuntime.InvokeVoidAsync("initializeDotNetHelper", dotNetObjRef);
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
            Reset(project); // Сбрасываем состояние
        }
        private bool IsEditing(string columnName, ProjectDTO order)
        {
            // Проверяем, что текущая колонка и редактируемый проект совпадают
            return columnEditing == columnName && currentEditedProject == order;
        }

        private async Task OnCellClick(DataGridCellMouseEventArgs<ProjectDTO> args)
        {
            if (!grid.IsValid || (currentEditedProject == args.Data && columnEditing == args.Column.Property))
            {
                return;
            }

            // Если уже редактируем другой проект, завершаем редактирование
            if (currentEditedProject != null)
            {
                await Update();
            }

            // This sets which column is currently being edited.
            columnEditing = args.Column.Property;

            // This sets the Item to be edited.
            await EditRow(args.Data);
        }

        private async Task Update()
        {
            if (currentEditedProject != null)
            {
                await grid.UpdateRow(currentEditedProject);
            }
        }

        private async Task EditRow(ProjectDTO project)
        {
            Reset();

            currentEditedProject = project; // Устанавливаем текущий редактируемый проект
            await grid.EditRow(project);

            // Добавляем небольшую задержку для завершения рендеринга
            await Task.Delay(1);

            // Устанавливаем курсор в конец текста в RadzenTextBox
            if (editor != null && editor is RadzenTextBox rc)
            {
                await JSRuntime.InvokeVoidAsync("setCursorToEnd", rc.Element);
            }
        }

        private void Reset(ProjectDTO project = null)
        {
            if (project != null)
            {
                if (currentEditedProject == project)
                {
                    currentEditedProject = null;
                }
            }
            else
            {
                currentEditedProject = null;
            }
        }

        #region Обработка отмены редактирования

        [JSInvokable]
        public static void InitializeDotNetHelper(DotNetObjectReference<Home> reference)
        {
        }

        [JSInvokable]
        public async Task OnEscapeKeyPressed()
        {
            if (currentEditedProject != null)
            {
                grid.CancelEditRow(currentEditedProject); // Синхронный вызов
                Reset(currentEditedProject); // Сбрасываем состояние
                //await InvokeAsync(StateHasChanged); // Обновляем UI
            }
        }
        #endregion

        protected override async Task DisposeResourcesAsync()
        {
            MainHubService.OnProjectChanged -= HandleProjectChangedAsync;
            await MainHubService.StopAsync(CancellationToken);
            await MainHubService.DisposeAsync();
            dotNetObjRef?.Dispose();
        }
    }
}