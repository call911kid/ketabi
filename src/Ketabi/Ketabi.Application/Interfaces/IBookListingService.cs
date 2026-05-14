using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Users;
using Ketabi.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ketabi.Application.Interfaces
{
    public interface IBookListingService
    { 
        Task<IEnumerable<BookSummaryDto>> GetAllBooksAsync(int pageNumber, int pageSize);
        Task<IEnumerable<BookSummaryDto>> GetListingsByStatusAsync(ListingStatus listingStatus, int pageNumber, int pageSize);

        Task<IEnumerable<BookSummaryDto>> SearchBooksAsync(string query, int pageNumber, int pageSize);

        Task<IEnumerable<BookSummaryDto>> GetBooksByUserIdAsync(Guid userId, int pageNumber, int pageSize);

        Task<IEnumerable<BookSummaryDto>> GetRelatedBooksAsync(Guid bookId, int pageNumber, int pageSize);

        Task<IEnumerable<BookSummaryDto>> GetBooksByCategoryAsync(string category, int pageNumber, int pageSize);

        Task<IEnumerable<BookSummaryDto>> GetBooksByAuthorAsync(string author, int pageNumber, int pageSize);

        Task<IEnumerable<BookSummaryDto>> GetFilteredBooksAsync(BookFilterDto filter);

        Task<BookDetailDto> GetBookByIdAsync(Guid bookId);

        Task<BookDetailDto> CreateBookAsync(CreateBookDto createDto, Guid userId);

        Task<BookDetailDto> UpdateBookAsync(Guid bookId, UpdateBookDto updateDto, Guid userId);

        Task<UserSummaryDto> GetOwnerProfileAsync(Guid userId);
        Task ApproveListingAsync(Guid listingId);
        Task RejectListingAsync(Guid listingId, string reasonForRejection);
        Task RestoreToPendingAsync(Guid listingId);

        Task DeleteBookAsync(Guid bookId, Guid userId);


        
    }
}
