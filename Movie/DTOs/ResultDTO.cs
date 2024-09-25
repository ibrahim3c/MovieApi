namespace Movie.DTOs
{
    public class ResultDTO<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
