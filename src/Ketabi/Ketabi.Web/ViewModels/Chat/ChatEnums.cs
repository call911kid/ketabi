namespace Ketabi.Web.ViewModels.Chat;

public enum TransactionStatus
{
    Active,
    MeetupPending,
    HandoffConfirmedRequester,
    HandoffConfirmedOwner,
    Completed
}

public enum RequestType { Borrow, Exchange }
