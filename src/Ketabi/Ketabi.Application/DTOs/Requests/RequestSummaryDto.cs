using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Requests
{
    public class RequestSummaryDto
    {
        public Guid RequestId { get; set; }
        public RequestType Type { get; set; }
        public RequestStatus Status { get; init; }
        public DateTime RequestDate { get; set; }
        public DateTime? ReturnDate { get; set; }


        public Guid ListingId { get; set; }
        public string ListingTitle { get; set; }
        public string ListingAuthor { get; set; }
        public string? ListingImageUrl { get; set; }

        public Guid RequesterId { get; set; }
        public string RequesterFullName { get; set; }
        public string RequsterEmail { get; set; }

        public Guid OwnerId { get; init; }
        public string OwnerFullName { get; init; }
        public string OwnerEmail { get; set; }

    }
}
