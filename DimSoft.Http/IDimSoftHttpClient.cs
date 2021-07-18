using System.Threading.Tasks;
using System.Threading;
using DimSoft.Http.Models;
using System.Collections.Generic;

namespace DimSoft.Http
{
    public interface IDimSoftHttpClient
    {
        Task<InternalResponse<T>> GetAsync<T>(string requestUri, IDictionary<string, string> headers = default, IDictionary<string, object> options = default, CancellationToken cancellationToken = default);
    }
}