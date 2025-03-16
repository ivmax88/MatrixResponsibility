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
        }

        public async Task<List<Project>?> GetAllProjects(CancellationToken ct)
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
                .AsSplitQuery()
                .ToListAsync(ct);

            return r;
        }

        public async Task ChangeProjectInfo(Project? project, CancellationToken ct)
        {
            if (project == null) return;

            var dbProject = await appDbContext.Projects.FirstOrDefaultAsync(x => x.Id == project.Id, ct);
            if(dbProject!=null)
            {
                dbProject = project;
                await appDbContext.SaveChangesAsync(ct);
            }
            await Clients.All.SendAsync(str.ProjectChanged, project, ct);
        }
    }
}
