namespace DimSoft.Http.Models
{
    public class InternalResponse<T>
    {
        public T Content { get; set; }

        public InternalError Error { get; set; }

        public bool IsError { get; set; } = false;
    }
}
