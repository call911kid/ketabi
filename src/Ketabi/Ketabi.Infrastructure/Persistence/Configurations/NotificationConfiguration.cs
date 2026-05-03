using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);

        builder.Property(n => n.Content).IsRequired().HasMaxLength(2000);

        builder.Property(n => n.IsRead).HasDefaultValue(false);

        builder.Property(n => n.NotificationType).IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Property(u => u.DeletedAt);
    }
}
