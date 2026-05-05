using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Application.Interfaces
{
    internal interface IReviewService
    {
        Task<ServiceResultDto<PagedResponseDto<ReviewDto>>> GetReviewsForUserAsync(Guid userId, PagedRequestDto pagination);
        Task<ServiceResultDto<ReviewDto>> CreateReviewAsync(Guid reviewerId, CreateReviewDto createReviewDto);
        Task<ServiceResultDto<ReviewDto>> GetReviewByIdAsync(Guid reviewId);
        Task<ServiceResultDto<bool>> DeleteReviewAsync(Guid reviewId , Guid requestingUserId);
    }
}
