using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;

namespace DimSoft.Http.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDimSoftHttpClient(this IServiceCollection services, Action<HttpClient> configureClient = default, JsonSerializerOptions options = default)
        {
            services.AddHttpClient(DimSoftHttpClientConstants.HttpClientName, configureClient);
            services.AddSingleton(new JsonSerializerOptionsGlobal(options));
            services.AddTransient<IDimSoftHttpClient, DimSoftHttpClient>();

            return services;
        }
    }
}
