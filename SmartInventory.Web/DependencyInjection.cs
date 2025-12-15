using SmartInventory.BLL.Implementations;
using SmartInventory.BLL.Interfaces;
using SmartInventory.DAL.Implementation;
using SmartInventory.DAL.Interfaces;

namespace SmartInventory.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductUnitOfWork, ProductUnitOfWork>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
