using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JacobDixon.AspNetCore.LiveWebTasks.Exceptions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddLiveSassCompile(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            services.Configure<LiveWebTasksOptions>(configuration.GetSection(LiveWebTasksOptions.OptionsName));
            services.AddHostedService<LiveSassCompileBackgroundService>();
            return services;
        }
    }
}
