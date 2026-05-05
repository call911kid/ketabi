using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class BookListingTagConfiguration : IEntityTypeConfiguration<BookListingTag>
{ 
    public void Configure(EntityTypeBuilder<BookListingTag> builder)
    {
        builder.ToTable("BookListingTags");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.BookListingId).IsRequired();
        builder.Property(t => t.Tag).IsRequired().HasMaxLength(50);
    }
}
