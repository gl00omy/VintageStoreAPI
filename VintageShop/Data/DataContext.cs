using Microsoft.EntityFrameworkCore;
namespace VintageStore.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<VintageStore> VintageStores => Set<VintageStore>();
    }
}
