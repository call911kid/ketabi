namespace Ketabi.Core.Domain.Entities;

public sealed class ExchangeRequest : Request
{
    public ExchangeRequest(Guid id) : base(id) { }

    public Guid OfferedBookId { get; set; }

    public BookListing? OfferedBook { get; set; }
}

