using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class BorrowRequestConfiguration : IEntityTypeConfiguration<BorrowRequest>
{
    public void Configure(EntityTypeBuilder<BorrowRequest> builder)
    {
        builder.ToTable("BorrowRequests");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.ReturnDate).IsRequired();
    }
}
