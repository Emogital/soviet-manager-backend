using Microsoft.EntityFrameworkCore;
using DataService.Models;

namespace DataService.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<LobbySettings> LobbySettings { get; set; }
    }
}
