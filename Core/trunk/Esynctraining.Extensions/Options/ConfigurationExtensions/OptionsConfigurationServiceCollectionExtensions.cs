using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Esynctraining.Extensions
{
    public static class OptionsConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureSingleton<ITOptions, TOptionsDto>(this IServiceCollection services,
            IConfiguration config,
            Func<TOptionsDto, ITOptions> builder)
            where TOptionsDto : class, new()
            where ITOptions : class
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            services
                .Configure<TOptionsDto>(config)
                .AddSingleton<ITOptions>((sp) => builder(sp.GetService<IOptions<TOptionsDto>>().Value));

            return services;
        }

        public static IServiceCollection ConfigureSingleton<TOptions>(this IServiceCollection services, IConfiguration config)
            where TOptions : class, new()
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            services
                .Configure<TOptions>(config)
                .AddSingleton((sp) => sp.GetService<IOptions<TOptions>>().Value);

            return services;
        }

    }

}
