using Ketabi.Infrastructure.Persistence;
using Ketabi.Infrastructure.Persistence.Identity;
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

        //services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
        //    .AddEntityFrameworkStores<KetabiDbContext>()
        //    .AddDefaultTokenProviders();

        return services;
    }
}