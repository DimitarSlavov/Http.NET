using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace DimSoft.Http.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDimSoftHttpClient(this IServiceCollection services, Action<HttpClient> configureClient = default)
        {
            services.AddHttpClient(DimSoftHttpClientConstants.HttpClientName, configureClient);
            services.AddTransient<IDimSoftHttpClient, DimSoftHttpClient>();

            return services;
        }
    }
}
