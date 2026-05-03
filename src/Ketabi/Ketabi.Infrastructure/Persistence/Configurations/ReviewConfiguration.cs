using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating).IsRequired();

        builder.Property(r => r.Comment).HasMaxLength(2000);

        builder.Property(r => r.TargetUserId).IsRequired();

        builder.Property(r => r.ReviewerId).IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Property(u => u.DeletedAt);
    }
}
