using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Dashboard
{
    public class UserOverviewDto
    {
        public int NumberOfUsers { get; set; }

        public string CurrentUserRole { get; set; } = string.Empty;

        public PagedResponseDto<UserSummaryDto> Users { get; set; } = new();
    }
}
