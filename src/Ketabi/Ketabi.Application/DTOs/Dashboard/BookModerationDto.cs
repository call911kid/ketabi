using System.Collections.Generic;
using Ketabi.Application.DTOs.Books;

namespace Ketabi.Application.DTOs.Dashboard
{
    public class BookModerationDto
    {
        public int TotalPendingBooks { get; set; }
        public int TotalApprovedBooks { get; set; }
        public int TotalRejectedBooks { get; set; }
        public int BooksApprovedThisWeek { get; set; }
        public int BooksRejectedThisWeek { get; set; }
        public IEnumerable<PendingBookDto> PendingBooks { get; set; }

        public BookModerationDto()
        {
            PendingBooks = new List<PendingBookDto>();
        }
    }
}