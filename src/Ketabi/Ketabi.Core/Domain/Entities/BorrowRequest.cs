namespace Ketabi.Core.Domain.Entities;

public sealed class BorrowRequest : Request
{
    public BorrowRequest() : base() { }
    public BorrowRequest(Guid id) : base(id) { }


    public required DateTime ReturnDate { get; set; }
}