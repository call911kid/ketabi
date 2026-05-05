using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Books;
namespace Ketabi.Application.Interfaces
{
    public interface IBookListingService
    { 
        Task<IEnumerable<BookSummaryDto>> GetAllBooksAsync(int pageNumber, int pageSize);

        Task<BookDetailDto> GetBookByIdAsync(Guid bookId);

        Task<IEnumerable<BookSummaryDto>> SearchBooksAsync(string query, int pageNumber, int pageSize);

        Task<IEnumerable<BookSummaryDto>> GetBooksByUserIdAsync(Guid userId, int pageNumber, int pageSize);


    }
}
