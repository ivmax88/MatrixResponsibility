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

        public MainHub(AppDbContext appDbContext)
        {
            this.appDbContext=appDbContext;
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
                .Include(x=>x.Corrections)
                .AsSplitQuery()
                .ToListAsync(Context.ConnectionAborted);

            return r;
        }

        public async Task ChangeProjectInfo(Project? project)
        {
            if (project == null) return;

            var dbProject = await appDbContext.Projects.FirstOrDefaultAsync(x => x.Id == project.Id, Context.ConnectionAborted);
            if(dbProject!=null)
            {
                appDbContext.Projects.Update(project);
                await appDbContext.SaveChangesAsync(Context.ConnectionAborted);
            }
            await Clients.All.SendAsync(str.ProjectChanged, project, Context.ConnectionAborted);
        }
    }
}
