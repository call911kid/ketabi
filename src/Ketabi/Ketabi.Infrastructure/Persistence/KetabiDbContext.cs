using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Persistence
{
    internal class KetabiDbContext : DbContext
    {
        public KetabiDbContext(DbContextOptions options) : base(options)
        {
        }

        protected KetabiDbContext()
        {
        }
    }
}
