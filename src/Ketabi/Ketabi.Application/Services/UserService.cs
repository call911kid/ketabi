using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Users;
using Ketabi.Application.Exceptions;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces;

namespace Ketabi.Application.Services
{
    internal class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreatedUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {

            var user = new User
            {
                Id = createUserDto.Id,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Bio = createUserDto.Bio,
                City = createUserDto.City,
                Governorate = createUserDto.Governorate,
                ReputationScore = 0,
                ProfilePictureUrl = createUserDto.ProfilePictureUrl

            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new CreatedUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                City = user.City,
                Governorate = user.Governorate,
                ReputationScore = user.ReputationScore,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }

        public async Task<UserSummaryDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("User not found.");

            var reviewCount = await _unitOfWork.Reviews.CountReviewsForUserAsync(user.Id);
            var tradesCount = await _unitOfWork.Requests.CountCompletedTradesForUserAsync(user.Id);

            return new UserSummaryDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = string.IsNullOrWhiteSpace(user.Email) ? string.Empty : user.Email.Split('@')[0],
                AvatarUrl = user.ProfilePictureUrl ?? string.Empty,
                Location = user.City ?? user.Governorate ?? string.Empty,
                ReputationScore = user.ReputationScore,
                ReviewCount = reviewCount,
                TradesCount = tradesCount
            };
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updateUserProfileDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
                throw new NotFoundException("User not found.");

            user.FirstName = updateUserProfileDto.FirstName ?? user.FirstName;
            user.LastName = updateUserProfileDto.LastName ?? user.LastName;
            user.Bio = updateUserProfileDto.Bio ?? user.Bio;
            user.City = updateUserProfileDto.City ?? user.City;
            user.Governorate = updateUserProfileDto.Governorate ?? user.Governorate;
            user.ProfilePictureUrl = updateUserProfileDto.ProfilePictureUrl ?? user.ProfilePictureUrl;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserProfileDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                AvatarUrl = user.ProfilePictureUrl
            };
        }
    }
}
