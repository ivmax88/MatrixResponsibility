using MatrixResponsibility.Common;
using MatrixResponsibility.Common.Constants;
using MatrixResponsibility.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MatrixResponsibility.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        private readonly AppDbContext appDbContext;
        private static int connectedClients = 0; // Статическая переменная для подсчёта подключений

        public MainHub(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        // Вызывается при подключении клиента
        public override async Task OnConnectedAsync()
        {
            connectedClients++;
            await UpdateConnectedClientsCount();
            await base.OnConnectedAsync();
        }

        // Вызывается при отключении клиента
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            connectedClients--;
            if (connectedClients < 0) connectedClients = 0; // Предотвращаем отрицательное значение
            await UpdateConnectedClientsCount();
            await base.OnDisconnectedAsync(exception);
        }

        // Метод для получения текущего количества подключений (по запросу)
        public async Task<int> GetConnectedClientsCount()
        {
            return await Task.FromResult(connectedClients);
        }

        // Метод для уведомления всех клиентов об изменении количества
        private async Task UpdateConnectedClientsCount()
        {
            await Clients.All.SendAsync(str.ConnectedClientsCount, connectedClients);
        }

        public async Task<List<Project>?> GetAllProjects()
        {
            var r = await appDbContext.Projects
                .Include(x => x.GIP)
                .Include(x => x.AssistantGIP)
                .Include(x => x.GAP)
                .Include(x => x.GKP)
                .Include(x => x.GP)
                .Include(x => x.EOM)
                .Include(x => x.SS)
                .Include(x => x.AK)
                .Include(x => x.Responsible)
                .Include(x => x.BKP)
                .Include(x => x.Corrections)
                .OrderBy(x => x.ProjectName)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync(Context.ConnectionAborted);

            return r;
        }

        public async Task ChangeProjectInfo(Project? project)
        {
            if (project == null) return;

            var flag = await appDbContext.Projects.AnyAsync(x => x.Id == project.Id, Context.ConnectionAborted);
            if (flag)
            {
                appDbContext.Projects.Update(project);
                await appDbContext.SaveChangesAsync(Context.ConnectionAborted);
            }
            await Clients.Others.SendAsync(str.ProjectChanged, project, Context.ConnectionAborted);
        }
    }
}