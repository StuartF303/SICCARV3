using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siccar.Common;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System;
using Microsoft.Extensions.Logging;
using Siccar.Application.Models;
using System.Linq;
using ActionService.V1.Repositories;
using Siccar.Common.ServiceClients;
using Siccar.Common.Exceptions;
using Siccar.Platform;
using Asp.Versioning;

namespace ActionService.V1.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route(Constants.FilesAPIURL)]
    [Produces("application/json")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IFileRepository _fileRepository;
        private readonly IRegisterServiceClient _registerServiceClient;
        private readonly IWalletServiceClient _walletServiceClient;

        public FilesController(ILogger<FilesController> logger, IFileRepository fileRepository, IRegisterServiceClient registerServiceClient, IWalletServiceClient walletServiceClient)
        {
            _logger = logger;
            _fileRepository = fileRepository;
            _registerServiceClient = registerServiceClient;
            _walletServiceClient = walletServiceClient;
        }

        [Authorize(Roles = Constants.WalletUserRole)]
        [HttpPost]
        public async Task<ActionResult<IList<UploadResult>>> PostFile(
         [FromForm] IEnumerable<IFormFile> files)
        {
            long maxFileSize = 2097152; //2Mb
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<UploadResult> uploadResults = new();
            //We only allow one file for now. 
            var file = files.First();

            var uploadResult = await _fileRepository.StoreFile(file, maxFileSize);

            uploadResults.Add(uploadResult);

            return new CreatedResult(resourcePath, uploadResults);
        }

        [Authorize(Roles = Constants.WalletUserRole)]
        [HttpDelete("{fileName}")]
        public async Task<ActionResult<string>> DeleteFile([FromRoute] string fileName)
        {
            await _fileRepository.DeleteFile(fileName);
            return Ok();
        }

        [Authorize(Roles = Constants.WalletUserRole)]
        [HttpGet("transactions/{walletAddress}/{registerId}/{txId}")]
        public async Task<FileStreamResult> GetFileFromTransaction([FromRoute] string walletAddress, [FromRoute] string registerId, [FromRoute] string txId)
        {
            var transaction = await _registerServiceClient.GetTransactionById(registerId, txId);
            if (!transaction.MetaData.TrackingData.TryGetValue("fileType", out string value))
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Transaction must have a fileType in the tracking data.");
            }

            if (!transaction.MetaData.TrackingData.TryGetValue("fileName", out var fileName))
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Transaction must have a fileName in the tracking data.");
            }

            if (!transaction.MetaData.TrackingData.TryGetValue("fileExtension", out var fileExtension))
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Transaction must have a file extension in the tracking data.");
            }

            if (!(transaction.MetaData.TransactionType == TransactionTypes.File))
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Transaction must be of type file.");
            }

            var decryptedPayloads = await _walletServiceClient.DecryptTransaction(transaction, walletAddress);

            return File(new MemoryStream(decryptedPayloads[0]), value, $"{fileName}{fileExtension}");
        }
    }
}
