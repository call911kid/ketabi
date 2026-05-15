namespace Ketabi.Application.DTOs.AuditLogs;

/// <summary>
/// Represents an audit log entry for admin dashboard.
/// </summary>
public class AuditLogDto
{
    public string Id { get; set; } = string.Empty;
    public string AdminName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
}