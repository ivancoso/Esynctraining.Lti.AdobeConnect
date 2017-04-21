using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Esynctraining.AspNetCore
{
    // NOTE: https://github.com/dotnetjunkie/Missing-Core-DI-Extensions/blob/master/src/MissingDIExtensions/AspNetCoreExtensions.cs
    public static class AspNetCoreExtensions
    {
        public static void AddRequestScopingMiddleware(this IServiceCollection services,
            Func<IDisposable> requestScopeProvider)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (requestScopeProvider == null)
            {
                throw new ArgumentNullException(nameof(requestScopeProvider));
            }

            services.AddSingleton<IStartupFilter>(new RequestScopingStartupFilter(requestScopeProvider));
        }

        private sealed class RequestScopingStartupFilter : IStartupFilter
        {
            private readonly Func<IDisposable> requestScopeProvider;


            public RequestScopingStartupFilter(Func<IDisposable> requestScopeProvider)
            {
                this.requestScopeProvider = requestScopeProvider ?? throw new ArgumentNullException(nameof(requestScopeProvider));
            }


            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextFilter)
            {
                return builder =>
                {
                    ConfigureRequestScoping(builder);

                    nextFilter(builder);
                };
            }

            private void ConfigureRequestScoping(IApplicationBuilder builder)
            {
                builder.Use(async (context, next) =>
                {
                    using (var scope = this.requestScopeProvider())
                    {
                        await next();
                    }
                });
            }

        }

    }

}
