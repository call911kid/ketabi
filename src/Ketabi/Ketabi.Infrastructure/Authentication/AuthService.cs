using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Auth;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Ketabi.Infrastructure.Authentication
{
    internal class AuthService : IAuthService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<KetabiUser> _userManager;

        public AuthService(IJwtTokenService jwtTokenService,
            IUserRepository userRepository,
            UserManager<KetabiUser> userManager
            )
        {
            _jwtTokenService = jwtTokenService;
            _userRepository = userRepository;
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
            throw new NotImplementedException();
        }
    }
}
