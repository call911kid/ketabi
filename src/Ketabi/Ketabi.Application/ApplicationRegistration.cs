using Ketabi.Application.Interfaces;
using Ketabi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ketabi.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
             cfg.AddMaps(typeof(ApplicationRegistration).Assembly));
        services.AddScoped<IReviewService, ReviewService>();
        return services;
    }
}