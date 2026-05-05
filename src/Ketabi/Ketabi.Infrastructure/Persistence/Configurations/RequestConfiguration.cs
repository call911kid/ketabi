using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type).IsRequired();

        builder.Property(r => r.Status).IsRequired();
      
        builder.Property(r => r.RequestDate).IsRequired();
     
        builder.Property(r => r.Note).HasMaxLength(2000);

        builder.Property(r => r.ReturnDate);

        builder.Property(r => r.SenderId).IsRequired();
       
        builder.Property(r => r.ReceiverId).IsRequired();
    
        builder.Property(r => r.ListingId).IsRequired();

        builder.Property(r => r.OfferedListingId);

        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Property(u => u.DeletedAt);

        builder.HasMany(rq => rq.Reviews)
            .WithOne(rv => rv.RelatedRequest)
            .HasForeignKey(rv => rv.RelatedRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.OfferedListing)
            .WithMany()
            .HasForeignKey(r => r.OfferedListingId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
