using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.User;

namespace Ketabi.Application.Interfaces
{
    public interface IUserService
    {
        Task<CreatedUserDto> CreateUserAsync(CreateUserDto createUserDto);
    }
}
