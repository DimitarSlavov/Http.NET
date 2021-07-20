using System.Threading.Tasks;
using System.Threading;
using DimSoft.Http.Models;
using System.Collections.Generic;

namespace DimSoft.Http
{
    public interface IDimSoftHttpClient
    {
        Task<InternalResponse<T>> GetAsync<T>(string requestUri, IDictionary<string, string> requestHeaders = default, IDictionary<string, string> contentHeaders = default, IDictionary<string, object> options = default, CancellationToken cancellationToken = default);
    }
}
