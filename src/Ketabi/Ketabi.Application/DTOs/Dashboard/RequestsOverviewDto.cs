using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Requests;

namespace Ketabi.Application.DTOs.Dashboard
{
    public class RequestsOverviewDto
    {
        public int NumberOfPendingRequests { get; set; }
        public int NumberOfCompletedRequest { get; set; }
        public int NumberOfApprovedRequests { get; set; }
        public int NumberOfRejectedRequests { get; set; }
        public int NumberOfRequests => NumberOfPendingRequests+NumberOfCompletedRequest+NumberOfApprovedRequests+NumberOfRejectedRequests;


        public IEnumerable<RequestSummaryDto>Requests { get; set; }

        public RequestsOverviewDto()
        {
            Requests = new List<RequestSummaryDto>();
        }





    }
}
