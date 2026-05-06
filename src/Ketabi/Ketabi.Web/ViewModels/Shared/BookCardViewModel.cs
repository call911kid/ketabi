using Ketabi.Core.Domain.Enums;

namespace Ketabi.Web.ViewModels.Shared;

public class BookCardViewModel
{
    public Guid             BookId          { get; set; }
    public string           Title           { get; set; } = string.Empty;
    public string           Author          { get; set; } = string.Empty;
    public string           Category        { get; set; } = string.Empty;
    public string           ImageUrl        { get; set; } = string.Empty;
    public string           LocationNote    { get; set; } = string.Empty;
    public double           DistanceInKm    { get; set; }
    public ListingCondition Condition       { get; set; }
    public SharingMode      SharingMode     { get; set; }
    public bool             IsAvailable     { get; set; }

    // Owner info
    public Guid   OwnerId         { get; set; }
    public string OwnerName       { get; set; } = string.Empty;
    public string OwnerAvatarUrl  { get; set; } = string.Empty;
    public double OwnerReputation { get; set; }

    // True when this card is shown in the context of the owner's own profile page
    public bool ShowOwnerActions { get; set; }

    // Computed CSS badge classes — used directly in Razor views
    public string SharingModeBadgeCss => SharingMode switch
    {
        SharingMode.Borrow   => "badge-borrow",
        SharingMode.Exchange => "badge-exchange",
        SharingMode.Both     => "badge-both",
        _                    => string.Empty
    };

    public string ConditionBadgeCss => Condition switch
    {
        ListingCondition.New  => "badge-new",
        ListingCondition.Good => "badge-good",
        ListingCondition.Fair => "badge-fair",
        ListingCondition.Poor => "badge-worn",
        _                     => string.Empty
    };

    public string ConditionLabel    => Condition.ToString();

    public string SharingModeLabel  => SharingMode switch
    {
        SharingMode.Both => "Borrow & Exchange",
        _                => SharingMode.ToString()
    };

    // Bootstrap Icons class — bound directly in Razor, no if/else needed.
    public string IconClass => SharingMode switch
    {
        SharingMode.Exchange => "bi bi-arrow-left-right",
        SharingMode.Both     => "bi bi-arrow-left-right",
        _                    => "bi bi-book"
    };

    // Human-readable distance string (e.g. "220 km away" or "3.5 km away").
    public string Distance => DistanceInKm > 0
        ? (DistanceInKm < 10
            ? $"{DistanceInKm:F1} km away"
            : $"{Math.Round(DistanceInKm):F0} km away")
        : string.Empty;
}
