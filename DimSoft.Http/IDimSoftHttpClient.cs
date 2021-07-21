using System.Threading.Tasks;
using System.Threading;
using DimSoft.Http.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace DimSoft.Http
{
    public interface IDimSoftHttpClient
    {
        Task<InternalResponse<T>> GetAsync<T>(string requestUri, IDictionary<string, string> requestHeaders = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default);
    }
}
