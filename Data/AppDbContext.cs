using Microsoft.EntityFrameworkCore;
using Referralcode.Models;

namespace Referralcode.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ReferralApplication> ReferralApplications { get; set; }
        public DbSet<SystemAccount> SystemAccounts { get; set; }
    }
}
