using Ketabi.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ketabi.Infrastructure.Persistence.Configurations;

public class ExchangeRequestConfiguration : IEntityTypeConfiguration<ExchangeRequest>
{
    public void Configure(EntityTypeBuilder<ExchangeRequest> builder)
    {
        builder.ToTable("ExchangeRequests");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OfferedBookId).IsRequired();
    }
}
