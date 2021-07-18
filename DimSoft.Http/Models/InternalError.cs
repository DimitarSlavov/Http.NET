using System;
using System.Net;

namespace DimSoft.Http.Models
{
    public class InternalError
    {
        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public Exception Exception { get; set; }
    }
}