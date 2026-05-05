using AutoMapper;
using Ketabi.Application.DTOs.Users;
using Ketabi.Core.Domain.Entities;

namespace Ketabi.Application.Mappings;

public class EmailToUserNameResolver : IValueResolver<User, UserSummaryDto, string>
{
    public string Resolve(User source, UserSummaryDto destination, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.Email)) return string.Empty;
        var parts = source.Email.Split('@');
        return parts.Length > 0 ? parts[0] : source.Email;
    }
}
