using MatrixResponsibility.Common;
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
        public async Task<IEnumerable<User>> Get()
        {
            return await appDbContext.Users.ToListAsync(Context.ConnectionAborted);
        }
    }
}
