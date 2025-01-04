using Microsoft.EntityFrameworkCore;
using DataService.Models;

namespace DataService.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<LobbySettingsData> LobbySettings { get; set; }
    }
}
