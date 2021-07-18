using System.Threading.Tasks;
using System.Threading;
using DimSoft.Http.Models;
using System.Collections.Generic;

namespace DimSoft.Http
{
    public interface IDimSoftClient
    {
        Task<InternalResponse<T>> GetAsync<T>(string requestUri, IDictionary<string, string> headers, IDictionary<string, object> options, CancellationToken cancellationToken);
    }
}