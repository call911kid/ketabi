using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.RequesterConfirmedHandoff)
            .HasDefaultValue(false);
        builder.Property(c => c.OwnerConfirmedHandoff)
            .HasDefaultValue(false);

        builder.Property(c => c.OwnerId)
            .IsRequired();
        builder.Property(c => c.RequesterId)
            .IsRequired();
        builder.Property(c => c.RequestId)
            .IsRequired();

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder.Property(c => c.DeletedAt);

        builder.HasOne(c => c.Request)
            .WithOne()
            .HasForeignKey<Conversation>(c => c.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
