using DimSoft.Http.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DimSoft.Http
{
    internal class DimSoftHttpClient : IDimSoftHttpClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptionsGlobal globalOptions;

        public DimSoftHttpClient(IHttpClientFactory httpClientFactory,
            JsonSerializerOptionsGlobal globalOptions)
        {
            this.httpClientFactory = httpClientFactory;
            this.globalOptions = globalOptions;
        }

        public async Task<InternalResponse<T>> GetAsync<T>(string requestUri, IDictionary<string, string> requestHeaders = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
        {
            Func<Task<InternalResponse<T>>> internalMethod = async () =>
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                if (requestHeaders is not null)
                {
                    foreach (var header in requestHeaders)
                    {
                        httpRequestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await GetHttpClient().SendAsync(httpRequestMessage, cancellationToken);
                using var responseContent = response.Content;
                using var stream = await responseContent.ReadAsStreamAsync();
                var content = await JsonSerializer.DeserializeAsync<T>(stream, globalOptions.Value ?? options, cancellationToken: cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var internalResponse = new InternalResponse<T>()
                    {
                        Error = new InternalError
                        {
                            Message = content.ToString(),
                            StatusCode = response.StatusCode
                        },
                        IsError = true
                    };

                    return internalResponse;
                }

                return new InternalResponse<T>()
                {
                    Content = content
                };
            };

            return await ExceptionWrapperAsync(internalMethod);
        }

        private async Task<InternalResponse<T>> ExceptionWrapperAsync<T>(Func<Task<InternalResponse<T>>> method)
        {
            try
            {
                return await method.Invoke();
            }
            catch (Exception ex)
            {
                return new InternalResponse<T>()
                {
                    Error = new InternalError
                    {
                        Message = ex.Message,
                        Exception = ex
                    },
                    IsError = true
                };
            }
        }

        private HttpClient GetHttpClient()
        {
            return httpClientFactory.CreateClient(DimSoftHttpClientConstants.HttpClientName);
        }
    }
}
