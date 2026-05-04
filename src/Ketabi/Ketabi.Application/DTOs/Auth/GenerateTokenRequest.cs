using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Application.DTOs.Auth
{
    public sealed class GenerateTokenRequest
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public IReadOnlyCollection<string> Roles { get; init; } = new List<string>();
    }
}
