using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.User;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces;

namespace Ketabi.Application.Services
{
    internal class UserService:IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<CreatedUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            
            var user = new User
            {
                Id=createUserDto.Id,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Bio = createUserDto.Bio,
                City = createUserDto.City,
                Governorate = createUserDto.Governorate,
                ReputationScore = 0,
                ProfilePictureUrl = createUserDto.ProfilePictureUrl

            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new CreatedUserDto
            {
                Id = user.Id,
                FirstName= user.FirstName,
                LastName= user.LastName,
                Bio= user.Bio,
                City= user.City,
                Governorate= user.Governorate,
                ReputationScore= user.ReputationScore,
                ProfilePictureUrl= user.ProfilePictureUrl
            };
        }
    }
}
