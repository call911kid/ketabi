using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Auth;
using Ketabi.Application.Exceptions;
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

                var roleResult=await _userManager.AddToRoleAsync(ketabiUser, RolesConstants.User);
                if (!roleResult.Succeeded)
                {
                    throw new NotFoundException("User Role not found");
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

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task AddToRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? await _userManager.FindByNameAsync(email) //ambigious but works for now
                ?? throw new NotFoundException("User not found");

            if(!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new NotFoundException($"Role {roleName} does not exist");
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded) {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(error => error.Description)));
            }

        }

        public async Task<IReadOnlyCollection<string>> GetRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Array.Empty<string>();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return (IReadOnlyCollection<string>)roles;
        }

        public async Task RemoveFromRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? await _userManager.FindByNameAsync(email) //ambigious but works for now
                ?? throw new NotFoundException("User not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new NotFoundException($"Role {roleName} does not exist");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(error => error.Description)));
            }
        }

    }
}
