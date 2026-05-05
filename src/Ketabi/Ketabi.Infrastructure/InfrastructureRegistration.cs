using Ketabi.Application.Interfaces;
using Ketabi.Core.Interfaces;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Authentication;
using Ketabi.Infrastructure.Persistence;
using Ketabi.Infrastructure.Persistence.Identity;
using Ketabi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ketabi.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KetabiDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<KetabiUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<KetabiDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookListingRepository, BookListingRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();

        services.AddScoped<IAuthService, AuthService>();







        return services;
    }
}
