using MatrixResponsibility.Common;
using MatrixResponsibility.Common.Constants;
using MatrixResponsibility.Common.DTOs;
using MatrixResponsibility.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
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

        public async Task<List<ProjectDTO>> GetAllProjects()
        {
            var r = await appDbContext.Projects
                .Select(x => new ProjectDTO
                {
                    Id = x.Id,
                    ProjectName = x.ProjectName,
                    IsActive = x.IsActive,
                    AB = x.AB,
                    InternalMeeting = x.InternalMeeting,
                    ReportStatus = x.ReportStatus,
                    Customer = x.Customer,
                    StartPermissionLetter = x.StartPermissionLetter,
                    MarketingName = x.MarketingName,
                    ObjectAddress = x.ObjectAddress,
                    GPZUDate = x.GPZUDate,
                    DateStartPD = x.DateStartPD,
                    DateFirstApproval = x.DateFirstApproval,
                    DateStartRD = x.DateStartRD,
                    DateEndRD = x.DateEndRD,
                    TotalArea = x.TotalArea,
                    SaleableArea = x.SaleableArea,
                    GIP = new UserDTO(x.GIP),
                    AssistantGIP = new UserDTO(x.AssistantGIP),
                    GAP = new UserDTO(x.GAP),
                    GKP = new UserDTO(x.GKP),
                    GP = new UserDTO(x.GP),
                    EOM = new UserDTO(x.EOM),
                    SS = new UserDTO(x.SS),
                    AK = new UserDTO(x.AK),
                    Responsible = new UserDTO(x.Responsible),
                    BKP = new BKPDTO {
                        Id= x.BKP.Id,
                        Director = new UserDTO(x.BKP.Director),
                        Name = x.BKP.Name
                    },
                    Corrections = x.Corrections == null ? new List<ProjectCorrectionDTO>() 
                        : x.Corrections.Select(c => new ProjectCorrectionDTO
                            {
                                Id = c.Id,
                                ApprovalDate = c.ApprovalDate,
                                StartDate = c.StartDate,
                                CorrectionNumber = c.CorrectionNumber
                            }).ToList()

                })
                .OrderBy(x => x.ProjectName)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync(Context.ConnectionAborted)
                ;

            return r??[];
        }

        public async Task ChangeProjectInfo(ProjectDTO? p)
        {
            if (p == null) return;

            var d = await appDbContext.Projects.Include(x => x.Corrections).FirstOrDefaultAsync(x => x.Id == p.Id, Context.ConnectionAborted);

            if (d==null) return;

            d.ProjectName = p.ProjectName;
            d.IsActive = p.IsActive;
            d.AB =p.AB;
            d.InternalMeeting = p.InternalMeeting;
            d.ReportStatus = p.ReportStatus;
            d.Customer = p.Customer;
            d.StartPermissionLetter = p.StartPermissionLetter;
            d.MarketingName = p.MarketingName;
            d.ObjectAddress = p.ObjectAddress;
            d.GPZUDate = p.GPZUDate?.ToUniversalTime().Date;
            d.DateStartPD = p.DateStartPD?.ToUniversalTime().Date;
            d.DateFirstApproval = p.DateFirstApproval?.ToUniversalTime().Date;
            d.DateStartRD = p.DateStartRD?.ToUniversalTime().Date;
            d.DateEndRD = p.DateEndRD?.ToUniversalTime().Date;
            d.TotalArea = p.TotalArea;
            d.SaleableArea = p.SaleableArea;
            d.GIPId = p.GIP.Id;
            d.AssistantGIPId = p.AssistantGIP.Id;
            d.GAPId = p.GAP.Id;
            d.GKPId = p.GKP.Id;
            d.GPId = p.GP.Id;
            d.EOMId = p.EOM.Id;
            d.SSId = p.SS.Id;
            d.AKId = p.AK.Id;
            d.ResponsibleId = p.Responsible.Id;
            d.BKPId = p.BKP.Id;


            //обновление корректировок

            // Обновление корректировок
            var dbCorrections = d.Corrections ?? new List<ProjectCorrection>();
            var dtoCorrectionIds = p.Corrections?.Select(x => x.Id).ToHashSet() ?? new HashSet<int>();
            var dbCorrectionIds = dbCorrections.Select(x => x.Id).ToHashSet();

            // Обновление существующих корректировок
            foreach (var correction in dbCorrections.Where(c => dtoCorrectionIds.Contains(c.Id)))
            {
                var dtoCorrection = p.Corrections!.First(x => x.Id == correction.Id);
                correction.CorrectionNumber = dtoCorrection.CorrectionNumber;
                correction.ApprovalDate = dtoCorrection.ApprovalDate;
                correction.StartDate = dtoCorrection.StartDate;
            }

            // Добавление новых корректировок
            var newCorrections = p.Corrections!
                .Where(x => !dbCorrectionIds.Contains(x.Id))
                .Select(x => new ProjectCorrection
                {
                    CorrectionNumber = x.CorrectionNumber,
                    ApprovalDate = x.ApprovalDate,
                    StartDate = x.StartDate,
                    ProjectId = d.Id
                });
            await appDbContext.ProjectCorrections.AddRangeAsync(newCorrections, Context.ConnectionAborted);

            // Удаление отсутствующих корректировок
            var correctionsToRemove = dbCorrections.Where(c => !dtoCorrectionIds.Contains(c.Id)).ToList();
            appDbContext.ProjectCorrections.RemoveRange(correctionsToRemove);


            await appDbContext.SaveChangesAsync(Context.ConnectionAborted);

            await Clients.Others.SendAsync(str.ProjectChanged, p!, Context.ConnectionAborted);
        }
    }
}