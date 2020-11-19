using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JacobDixon.AspNetCore.LiveWebTasks.Options;

namespace JacobDixon.AspNetCore.LiveWebTasks.Extensions
{
    /// <summary>
    /// Contains extension methods for IServiceCollectionExtensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Sets up LiveWebTasks into the <paramref name="services"/>.
        /// </summary>
        /// <param name="services">Auto passed in IServiceCollection.</param>
        /// <returns>IServiceCollection ready for chaining.</returns>
        public static IServiceCollection AddLiveWebTasks(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            services.Configure<LiveWebTasksOptions>(configuration.GetSection(LiveWebTasksOptions.OptionsName));
            services.AddHostedService<LiveWebTasksBackgroundService>();
            return services;
        }
    }
}
