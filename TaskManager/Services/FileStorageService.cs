using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskManager.Services
{
    public class FileStorageService : IFileStorageService
    {
        private static readonly string[] BlockedExtensions =
        {
            ".exe", ".dll", ".bat", ".cmd", ".com", ".msi", ".ps1", ".sh"
        };

        public async Task<StoredFile> SaveAsync(string originalName, string contentType, Stream content)
        {
            if (string.IsNullOrWhiteSpace(originalName)) throw new ArgumentException("file name required");
            if (content == null) throw new ArgumentNullException(nameof(content));

            var ext = (Path.GetExtension(originalName) ?? string.Empty).ToLowerInvariant();
            if (Array.IndexOf(BlockedExtensions, ext) >= 0)
                throw new InvalidOperationException($"File extension {ext} is not allowed");

            var maxSize = long.Parse(ConfigurationManager.AppSettings["App:MaxUploadSizeBytes"] ?? "10485760");
            if (content.CanSeek && content.Length > maxSize)
                throw new InvalidOperationException("File too large");

            var rootCfg = ConfigurationManager.AppSettings["App:UploadRoot"] ?? "~/Uploads";
            var root = rootCfg.StartsWith("~/") ? HostingEnvironment.MapPath(rootCfg) : rootCfg;

            var dayFolder = Path.Combine(root ?? Path.GetTempPath(), DateTime.UtcNow.ToString("yyyy-MM-dd"));
            Directory.CreateDirectory(dayFolder);

            var safeName = Path.GetFileName(originalName);
            var storedName = $"{Guid.NewGuid():N}_{safeName}";
            var storedPath = Path.Combine(dayFolder, storedName);

            long bytesCopied = 0;
            using (var fs = new FileStream(storedPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await content.CopyToAsync(fs);
                bytesCopied = fs.Length;
            }

            AppLogger.Create<FileStorageService>()?.LogInformation("Stored upload {File} -> {Path} ({Bytes}b)", safeName, storedPath, bytesCopied);

            return new StoredFile
            {
                FileName = safeName,
                ContentType = contentType ?? "application/octet-stream",
                StoredPath = storedPath,
                SizeBytes = bytesCopied
            };
        }

        public Stream OpenRead(string storedPath)
        {
            if (!File.Exists(storedPath))
                throw new FileNotFoundException("Attachment not found", storedPath);
            return new FileStream(storedPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        }

        public bool Delete(string storedPath)
        {
            try
            {
                if (File.Exists(storedPath))
                {
                    File.Delete(storedPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Create<FileStorageService>()?.LogWarning(ex, "Failed to delete file {Path}", storedPath);
            }
            return false;
        }
    }
}
