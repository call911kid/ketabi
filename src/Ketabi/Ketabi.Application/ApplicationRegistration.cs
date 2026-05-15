using Ketabi.Application.Interfaces;
using Ketabi.Application.Mappings;
using Ketabi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ketabi.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
             cfg.AddMaps(typeof(MappingProfile).Assembly));
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IBookListingService, BookListingService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IBookListingService, BookListingService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}
