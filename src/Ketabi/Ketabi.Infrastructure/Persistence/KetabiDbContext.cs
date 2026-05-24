using Ketabi.Core.Domain.Entities;
using Ketabi.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Persistence
{
    public class KetabiDbContext : IdentityDbContext<KetabiUser, IdentityRole<Guid>, Guid>
    {
        public KetabiDbContext(DbContextOptions<KetabiDbContext> options) : base(options) { }

        public DbSet<User> DomainUsers { get; set; }
        public DbSet<BookListing> BookListings { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KetabiDbContext).Assembly);
        }

    }
}

