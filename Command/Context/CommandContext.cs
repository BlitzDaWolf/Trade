using Command.Models;
using Microsoft.EntityFrameworkCore;

namespace Command.Context
{
    public class CommandContext : DbContext
    {
        public CommandContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Models.Price> Prices { get; set; }
        public DbSet<StrategyArgumentModel> StrategyArguments { get; set; }
        public DbSet<ResultModel> Results { get; set; }
    }
}
