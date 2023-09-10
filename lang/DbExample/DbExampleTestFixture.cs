using DbExample.Context;

namespace DbExample
{
    public class DbExampleTestFixture
    {
        public DbExampleTestFixture()
        {
            using var context = new AppDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
