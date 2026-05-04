namespace Ketabi.Application.DTOs.Queries;

/// <summary>Direction of a request relative to the authenticated user.</summary>
public enum RequestDirection
{
    /// <summary>Requests where the authenticated user is the requester (Outgoing tab).</summary>
    Outgoing = 0,

    /// <summary>Requests where the authenticated user is the book owner (Incoming tab).</summary>
    Incoming = 1
}
