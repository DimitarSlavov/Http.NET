using DimSoft.Http.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DimSoft.Http
{
    internal class DimSoftClient : IDimSoftClient
    {
        private readonly HttpClient httpClient;

        public DimSoftClient(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            if (Uri.TryCreate("", UriKind.Absolute, out var uri))
            {
                httpClient.BaseAddress = uri;
            }
        }

        public async Task<InternalResponse<T>> GetAsync<T>(string requestUri, IDictionary<string, string> headers, IDictionary<string, object> options, CancellationToken cancellationToken)
        {
            Func<Task<InternalResponse<T>>> internalMethod = async () =>
            {
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                foreach (var header in headers)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }

                foreach (var option in options)
                {
                    httpRequestMessage.Options.Set(new HttpRequestOptionsKey<string>(option.Key), option.Value.ToString());
                }

                using var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
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
    }
}
