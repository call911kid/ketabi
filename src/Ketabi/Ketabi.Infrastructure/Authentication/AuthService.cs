using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Auth;
using Ketabi.Application.DTOs.User;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Ketabi.Infrastructure.Authentication
{
    internal class AuthService : IAuthService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserService _userService;
        private readonly UserManager<KetabiUser> _userManager;

        public AuthService(IJwtTokenService jwtTokenService,
            IUserService userService,
            UserManager<KetabiUser> userManager
            )
        {
            _jwtTokenService = jwtTokenService;
            _userService = userService;
            _userManager = userManager;
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password."); // change ltr w custom exception
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _jwtTokenService.GenerateTokenAsync(new GenerateTokenRequest
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles
            });

            
            return new AuthResponse
            {
                Token = token,
                Roles = roles.ToList()
            };
        }
        public async Task RegisterAsync(RegisterRequest request)
        {
            var ketabiUser = new KetabiUser
            {
                UserName = request.UserName,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(ketabiUser, request.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to register user."); // change later with custom exception
            }
            ketabiUser = await _userManager.FindByEmailAsync(request.Email);
            var createUserDto= new CreateUserDto
            {
                Id= ketabiUser.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Bio = request.Bio,
                City = request.City,
                Governorate = request.Governorate,
                ProfilePictureUrl = request.ProfilePictureUrl
            };

            var createdUser =await _userService.CreateUserAsync(createUserDto);

            await _userManager.AddToRoleAsync(ketabiUser, "User");


        }
    }
}
