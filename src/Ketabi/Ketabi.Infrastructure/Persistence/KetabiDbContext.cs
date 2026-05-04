using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ketabi.Core.Domain.Entities;
using Ketabi.Infrastructure.Persistence.Identity;

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
        public DbSet<ExchangeRequest> ExchangeRequests { get; set; }
        public DbSet<BorrowRequest> BorrowRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KetabiDbContext).Assembly);
        }

    }
}

