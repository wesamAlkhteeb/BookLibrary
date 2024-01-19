using DataAccessLayer.DatabaseContext;
using DataAccessLayer.Repository;
using Microsoft.Extensions.DependencyInjection;


namespace DataAccessLayer.DependencyInjectionDataAccesssLayer
{
    public static class DependencyInjection
    {
        public static void DIDataAccessLayer(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<DbContext>();
        }
    }
}
