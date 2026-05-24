using Ketabi.Core.Domain.Entities;
using Ketabi.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);

        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Bio).HasMaxLength(1000);

        builder.Property(u => u.City).HasMaxLength(100);

        builder.Property(u => u.Governorate).HasMaxLength(100);

        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(500);

        builder.Property(u => u.ReputationScore).HasDefaultValue(0.0);

        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Property(u => u.DeletedAt);

        builder.HasMany(u => u.Listings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.SentRequests)
            .WithOne(r => r.Sender)
            .HasForeignKey(r => r.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReceivedRequests)
            .WithOne(r => r.Receiver)
            .HasForeignKey(r => r.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReviewsWritten)
            .WithOne(r => r.Reviewer)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReviewsReceived)
            .WithOne(r => r.TargetUser)
            .HasForeignKey(r => r.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<KetabiUser>()
               .WithOne()
               .HasForeignKey<User>(u => u.Id)
               .OnDelete(DeleteBehavior.Cascade);

        //for chat 
        builder.HasMany(u => u.OwnedConversations)
            .WithOne(c => c.Owner)
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.RequestedConversations)
            .WithOne(c => c.Requester)
            .HasForeignKey(c => c.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
