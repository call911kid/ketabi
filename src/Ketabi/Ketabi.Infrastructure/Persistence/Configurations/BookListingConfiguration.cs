using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class BookListingConfiguration : IEntityTypeConfiguration<BookListing>
{
    public void Configure(EntityTypeBuilder<BookListing> builder)
    {
        builder.ToTable(nameof(BookListing));

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(300);

        builder.Property(b => b.Author).IsRequired().HasMaxLength(200);

        builder.Property(b => b.ISBN).HasMaxLength(50);

        builder.Property(b => b.Description).HasMaxLength(2000);

        builder.Property(b => b.Language).HasMaxLength(50);

        builder.Property(b => b.Publisher).HasMaxLength(200);

        builder.Property(b => b.Condition).IsRequired();

        builder.Property(b => b.SharingMode).IsRequired();

        builder.Property(b => b.IsAvailable).HasDefaultValue(true);

        builder.Property(b => b.ImageUrl).HasMaxLength(500);

        builder.Property(b => b.LocationNote).HasMaxLength(500);

        builder.Property(b => b.CategoryId).IsRequired();

        builder.Property(b => b.UserId).IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        builder.Property(b => b.SharingDurationInDays);

        builder.HasMany(b => b.Tags)
            .WithOne(t => t.BookListing)
            .HasForeignKey(t => t.BookListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Property(u => u.DeletedAt);

        builder.HasMany(b => b.Requests)
            .WithOne(r => r.Listing)
            .HasForeignKey(r => r.ListingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
