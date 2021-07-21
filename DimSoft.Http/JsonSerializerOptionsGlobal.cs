using System.Text.Json;

namespace DimSoft.Http
{
    internal sealed class JsonSerializerOptionsGlobal
    {
        private readonly JsonSerializerOptions options;
        
        public JsonSerializerOptionsGlobal(JsonSerializerOptions options)
        {
            this.options = options;
        }

        public JsonSerializerOptions Value => options;
    }
}
