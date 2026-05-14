using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Dashboard
{
    public class UserOverviewDto
    {
        public int NumberOfUsers { get; set; }

        public PagedResponseDto<UserSummaryDto> Users { get; set; } = new();
    }
}
