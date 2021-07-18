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

        public DimSoftHttpClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<InternalResponse<T>> GetAsync<T>(string requestUri, IDictionary<string, string> headers = default, IDictionary<string, object> options = default, CancellationToken cancellationToken = default)
        {
            Func<Task<InternalResponse<T>>> internalMethod = async () =>
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                if (headers is not null)
                {
                    foreach (var header in headers)
                    {
                        httpRequestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                if (options is not null)
                {
                    foreach (var option in options)
                    {
                        httpRequestMessage.Options.Set(new HttpRequestOptionsKey<string>(option.Key), option.Value.ToString());
                    }
                }

                using var response = await GetHttpClient().SendAsync(httpRequestMessage, cancellationToken);
                using var responseContent = response.Content;
                using var stream = await responseContent.ReadAsStreamAsync();
                var content = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
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
