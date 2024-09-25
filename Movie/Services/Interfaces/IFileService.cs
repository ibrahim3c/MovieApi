namespace Movie.Services.Interfaces
{
    public interface IFileService
    {
        public Task<string> UploadFileAsync(IFormFile file, string folder);
        public Task DeleteFileAsync(string FilePath);
    }
}
