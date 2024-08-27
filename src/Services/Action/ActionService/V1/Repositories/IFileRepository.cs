using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Siccar.Application.Models;
using System.IO;
using System.Threading.Tasks;

namespace ActionService.V1.Repositories
{
    public interface IFileRepository
    {
        public Task<UploadResult> StoreFile(IFormFile file, long maxFileSize);

        public Task<Stream> GetFile(string fileName);
        public Task DeleteFile(string fileName);
    }
}
