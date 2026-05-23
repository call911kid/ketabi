namespace Ketabi.Web.ViewModels.Chat;

public record JourneyStep(string Key, string Label, string Icon); // Icon = Bootstrap Icon class

public static class ChatJourneyConfig
{
    // 5 steps matching the React JOURNEY_STEPS array
    public static readonly IReadOnlyList<JourneyStep> Steps = new[]
    {
        new JourneyStep("accepted", "Accepted",  "bi-check-circle"),
        new JourneyStep("chatting", "Chatting",  "bi-chat-dots"),
        new JourneyStep("meetup",   "Meetup",    "bi-geo-alt"),
        new JourneyStep("handoff",  "Handoff",   "bi-arrow-left-right"),
        new JourneyStep("complete", "Complete",  "bi-balloon-heart"),
    };

    /// <summary>
    /// Returns the 0-based index of the currently active step.
    /// Matches getJourneyStep() from the React source.
    /// </summary>
    public static int GetActiveIndex(TransactionStatus status) => status switch
    {
        TransactionStatus.Active                      => 1,
        TransactionStatus.MeetupPending               => 2,
        TransactionStatus.HandoffConfirmedRequester   => 3,
        TransactionStatus.HandoffConfirmedOwner       => 3,
        TransactionStatus.Completed                   => 4,
        _                                             => 1
    };
}
