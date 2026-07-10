using System.IO;
using System.Threading.Tasks;

namespace TaskManager.Services
{
    public class StoredFile
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string StoredPath { get; set; }
        public long SizeBytes { get; set; }
    }

    public interface IFileStorageService
    {
        Task<StoredFile> SaveAsync(string originalName, string contentType, Stream content);
        Stream OpenRead(string storedPath);
        bool Delete(string storedPath);
    }
}
