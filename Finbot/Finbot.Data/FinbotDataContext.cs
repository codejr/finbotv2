using Finbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Finbot.Data
{
    public class FinbotDataContext : DbContext
    {
        public DbSet<Position> Positions { get; set; }

        public DbSet<Portfolio> Portfolios { get; set; }

        public DbSet<Trade> Trades { get; set; }

        public FinbotDataContext(DbContextOptions<FinbotDataContext> options) : base(options) { }
    }
}