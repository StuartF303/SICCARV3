using Siccar.Application.Models;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ActionService.V1.Controllers;
using Microsoft.Extensions.Logging;
using Siccar.Common.Adaptors;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using ActionService.ActionConstants;

namespace ActionService.V1.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ILogger<FileRepository> _logger;
        private readonly IDaprClientAdaptor _client;

        public FileRepository(IDaprClientAdaptor client, ILogger<FileRepository> logger)
        {
            _logger = logger;
            _client = client;
        }

        public async Task DeleteFile(string fileName)
        {
            try
            {
                var metadata = new Dictionary<string, string> {
                                { "fileName", fileName },
                                { "blobName", fileName },
                                { "key", fileName }
                            };
                await _client.InvokeBindingAsync<byte[]>(StringConstants.FileBindingActionName, StringConstants.FileDeleteActionName, null, metadata);
            }
            catch (Exception e)
            {
                _logger.LogError("Could not delete file: {id}, {exception}", fileName, e.Message);
                throw;
            }
        }

        public async Task<Stream> GetFile(string fileName)
        {
            try
            {
                var client = new HttpClient();
                var requestUri = $"http://localhost:3500/v1.0/bindings/{StringConstants.FileBindingActionName}";
                var content = new StringContent($$"""{ "operation": "{{StringConstants.FileGetActionName}}", "metadata": { "fileName": "{{fileName}}", "blobName": "{{fileName}}", "key": "{{fileName}}" } }""");
                var response = await client.PostAsync(requestUri, content);
                return response.Content.ReadAsStream();
            }
            catch (Exception e)
            {
                _logger.LogError("Could not download file: {id}, {exception}", fileName, e.Message);
                throw;
            }
        }

        public async Task<UploadResult> StoreFile(IFormFile file, long maxFileSize)
        {
            var uploadResult = new UploadResult();
            var untrustedFileName = file.FileName;
            var trustedFileNameForDisplay = Guid.NewGuid().ToString();
            uploadResult.FileName = untrustedFileName;

            if (file.Length == 0)
            {
                _logger.LogInformation("{FileName} length is 0 (Err: 1)",
                    untrustedFileName);
                uploadResult.ErrorReason = $"{untrustedFileName} length is 0 bytes";
            }
            else if (file.Length > maxFileSize)
            {
                _logger.LogInformation("{FileName} of {Length} bytes is " +
                    "larger than the limit of {Limit} bytes (Err: 2)",
                    untrustedFileName, file.Length, maxFileSize);
                uploadResult.ErrorReason = $"{untrustedFileName} of {file.Length} bytes is " +
                    "larger than the limit of {Limit} bytes (Err: 2)";
            }
            else
            {
                try
                {
                    byte[] fileBinary;

                    using (Stream SourceStream = file.OpenReadStream())
                    {
                        var memoryStream = new MemoryStream();
                        SourceStream.CopyTo(memoryStream);
                        fileBinary = memoryStream.ToArray();
                    }

                    var metadata = new Dictionary<string, string> {
                                { "fileName", trustedFileNameForDisplay },
                                { "blobName", trustedFileNameForDisplay },
                                { "key", trustedFileNameForDisplay },
                                { "contentType", file.ContentType }
                            };
                    await _client.InvokeBindingAsync(StringConstants.FileBindingActionName, StringConstants.FileCreateActionName, fileBinary, metadata);

                    _logger.LogInformation("{FileName} saved.",
                        untrustedFileName);
                    uploadResult.Uploaded = true;
                    uploadResult.StoredFileName = trustedFileNameForDisplay;
                }
                catch (Exception ex)
                {
                    _logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        untrustedFileName, ex.Message);
                    uploadResult.ErrorReason = ex.Message;
                }
            }
            return uploadResult;
        }
    }
}
