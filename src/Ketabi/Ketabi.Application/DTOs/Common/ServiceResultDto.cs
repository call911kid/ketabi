using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Common;

/// <summary>Standard envelope for all service operation results.</summary>
public class ServiceResultDto<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static ServiceResultDto<T> Ok(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };

    public static ServiceResultDto<T> Fail(string error)
        => new() { Success = false, Errors = [error] };

    public static ServiceResultDto<T> Fail(IEnumerable<string> errors)
        => new() { Success = false, Errors = errors.ToList() };
}
