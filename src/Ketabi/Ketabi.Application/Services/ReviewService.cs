using AutoMapper;
using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Reviews;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Application.Services
{
    internal class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        // Create
        public async Task<ServiceResultDto<ReviewDto>> CreateReviewAsync(Guid reviewerId, CreateReviewDto createReviewDto)
        {
            // Validate related request
            var request = await _unitOfWork.Requests
                .GetByIdAsync(createReviewDto.RelatedRequestId);
            if (request is null)
                return ServiceResultDto<ReviewDto>.Fail("Related request not found.");
            // Only participants of the request can review
            if (request.SenderId != reviewerId && request.ReceiverId != reviewerId)
                return ServiceResultDto<ReviewDto>.Fail("You are not a participant in the related request.");
            // Only completed requests can be reviewed
            if (request.Status != RequestStatus.Completed)
                return ServiceResultDto<ReviewDto>.Fail("You can only review completed requests.");
            // Prevent duplicate reviews by the same reviewer for the same request
            var existingReview = await _unitOfWork.Reviews
                .FindAsync(r => r.RelatedRequestId == createReviewDto.RelatedRequestId
                    && r.ReviewerId == reviewerId);
            if (existingReview is not null)
                return ServiceResultDto<ReviewDto>.Fail("You have already reviewed this request.");
            // Verify the reviewee exists.
            var reviewee = await _unitOfWork.Users.GetByIdAsync(createReviewDto.RevieweeId);
            if (reviewee is null)
                return ServiceResultDto<ReviewDto>.Fail("Reviewee user not found.");

            var reviewer = await _unitOfWork.Users.GetByIdAsync(reviewerId);
            if (reviewer is null)
                return ServiceResultDto<ReviewDto>.Fail("Reviewer user not found.");
            // Map DTO → entity, then set the server-side fields.
            var review = _mapper.Map<Review>(createReviewDto);
            review.ReviewerId = reviewerId;
            await _unitOfWork.Reviews.AddAsync(review);
            // Update the reviewee's reputation score
            reviewee.ReputationScore = await _unitOfWork.Reviews.CalculateAverageRatingForUserAsync(createReviewDto.RevieweeId);
            _unitOfWork.Users.Update(reviewee);
            await _unitOfWork.SaveChangesAsync();
            // Recalculate and persist the reviewee's reputation score.
            reviewee.ReputationScore = await _unitOfWork.Reviews.CalculateAverageRatingForUserAsync(createReviewDto.RevieweeId);
            _unitOfWork.Users.Update(reviewee);

            await _unitOfWork.SaveChangesAsync();

            // Attach navigations so the mapper can build the full ReviewDto.
            review.Reviewer = reviewer;
            review.TargetUser = reviewee;
            review.RelatedRequest = request;

            return ServiceResultDto<ReviewDto>.Ok(
                _mapper.Map<ReviewDto>(review),
                "Review submitted successfully.");

        }
        public async Task<ServiceResultDto<ReviewDto>> GetReviewByIdAsync(Guid reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review is null)
                return ServiceResultDto<ReviewDto>.Fail("Review not found.");

           // Attach navigations so the mapper can build the full ReviewDto
            review.Reviewer = await _unitOfWork.Users.GetByIdAsync(review.ReviewerId);
            review.TargetUser = await _unitOfWork.Users.GetByIdAsync(review.TargetUserId);

            
            review.RelatedRequest = await _unitOfWork.Requests.GetByIdAsync(review.RelatedRequestId);


            return ServiceResultDto<ReviewDto>.Ok(_mapper.Map<ReviewDto>(review));
        }

        public async Task<ServiceResultDto<PagedResponseDto<ReviewDto>>> GetReviewsForUserAsync(Guid userId, PagedRequestDto pagination)
        {
            if (await _unitOfWork.Users.GetByIdAsync(userId) is null)
                return ServiceResultDto<PagedResponseDto<ReviewDto>>.Fail("User not found.");

            var paged = await _unitOfWork.Reviews.GetReviewsForUserAsync(userId, pagination.Page, pagination.PageSize);

            return ServiceResultDto<PagedResponseDto<ReviewDto>>.Ok(new PagedResponseDto<ReviewDto>
            {
                Items = _mapper.Map<IReadOnlyList<ReviewDto>>(paged.Items),
                TotalCount = paged.TotalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
            });
        }
        public async Task<ServiceResultDto<bool>> DeleteReviewAsync(Guid reviewId, Guid requestingUserId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review is null)
                return ServiceResultDto<bool>.Fail("Review not found.");

            if (review.ReviewerId != requestingUserId)
                return ServiceResultDto<bool>.Fail("You can only delete your own reviews.");

            _unitOfWork.Reviews.Delete(review);

            // Recalculate reputation score after removal.
            var reviewee = await _unitOfWork.Users.GetByIdAsync(review.TargetUserId);
            if (reviewee is not null)
            {
                reviewee.ReputationScore = await _unitOfWork.Reviews
                    .CalculateAverageRatingForUserAsync(review.TargetUserId);
                _unitOfWork.Users.Update(reviewee);
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto<bool>.Ok(true, "Review deleted successfully.");
        }
    }
}
