using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.Interfaces
{
    public interface IUserService
    {
        Task<CreatedUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserSummaryDto> GetUserByIdAsync(Guid userId);
        Task<UserProfileDto> GetUserProfileAsync(Guid userId, Guid? currentUserId = null);
        Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updateUserProfileDto);
    }
}
