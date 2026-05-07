using Ketabi.Application.DTOs.Auth;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces;
using Ketabi.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Ketabi.Infrastructure.Authentication
{
    internal class AuthService : IAuthService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<KetabiUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthService(IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork,
            UserManager<KetabiUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
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
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
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
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var ketabiUser = new KetabiUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                };

                var result = await _userManager.CreateAsync(ketabiUser, request.Password);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", result.Errors.Select(error => error.Description)));
                }

                var user = new User
                {
                    Id = ketabiUser.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Bio = request.Bio,
                    City = request.City,
                    Governorate = request.Governorate,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    ReputationScore = 0
                };

                await _unitOfWork.Users.AddAsync(user);

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
                    if (!createRoleResult.Succeeded)
                    {
                        throw new InvalidOperationException(string.Join(", ", createRoleResult.Errors.Select(error => error.Description)));
                    }
                }

                var roleResult = await _userManager.AddToRoleAsync(ketabiUser, "User");
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(error => error.Description)));
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

    }
}
