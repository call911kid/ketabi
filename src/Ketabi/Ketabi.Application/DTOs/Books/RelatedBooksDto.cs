using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Application.DTOs.Books
{
    public class RelatedBooksDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}
