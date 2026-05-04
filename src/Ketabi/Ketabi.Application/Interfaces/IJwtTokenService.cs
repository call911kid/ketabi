using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Auth;

namespace Ketabi.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(GenerateTokenRequest request);
    }
}
