using Ketabi.Application.Common;
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
        private readonly AutoMapper.IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreatedUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            user.ReputationScore = 0;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var created = _mapper.Map<CreatedUserDto>(user);
            return created;
        }

        public async Task<UserSummaryDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) throw new NotFoundException("User not found.");

            var summary = _mapper.Map<UserSummaryDto>(user);

            // Stats - create a new DTO instance since properties are init-only
            var reviewCount = await _unitOfWork.Reviews.CountReviewsForUserAsync(user.Id);
            var tradesCount = await _unitOfWork.Requests.CountCompletedTradesForUserAsync(user.Id);

            return new UserSummaryDto
            {
                UserId = summary.UserId,
                FullName = summary.FullName,
                UserName = summary.UserName,
                AvatarUrl = summary.AvatarUrl,
                Location = summary.Location,
                ReputationScore = summary.ReputationScore,
                ReviewCount = reviewCount,
                TradesCount = tradesCount
            };
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId, Guid? currentUserId = null)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) throw new NotFoundException("User not found.");

            var profile = _mapper.Map<UserProfileDto>(user);

            // Populate stats
            var reviewCount = await _unitOfWork.Reviews.CountReviewsForUserAsync(user.Id);
            var booksListed = user.Listings?.Count ?? 0;
            var activeListings = user.Listings?.Count(l => l.IsAvailable) ?? 0;
            var completedTransactions = await _unitOfWork.Requests.CountCompletedTradesForUserAsync(user.Id);
            var reputation = await _unitOfWork.Reviews.CalculateAverageRatingForUserAsync(user.Id);

            profile.Stats = new UserStatsDto
            {
                ReputationScore = reputation,
                ReviewCount = reviewCount,
                BooksListed = booksListed,
                CompletedTransactions = completedTransactions,
                UnreadNotifications = user.Notifications?.Count(n => !n.IsRead) ?? 0
            };

            // IsOwnProfile
            profile.IsOwnProfile = currentUserId.HasValue && currentUserId.Value == user.Id;

            return profile;
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updateUserProfileDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) throw new NotFoundException("User not found.");

            // Map non-null fields onto existing user
            _mapper.Map(updateUserProfileDto, user);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var profile = _mapper.Map<UserProfileDto>(user);
            profile.Stats = new UserStatsDto
            {
                ReputationScore = user.ReputationScore,
                ReviewCount = await _unitOfWork.Reviews.CountReviewsForUserAsync(user.Id),
                BooksListed = user.Listings?.Count ?? 0,
                CompletedTransactions = await _unitOfWork.Requests.CountCompletedTradesForUserAsync(user.Id),
                UnreadNotifications = user.Notifications?.Count(n => !n.IsRead) ?? 0
            };

            return profile;
        }
    }
}
