using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DogsServer.Services;
using Microsoft.Extensions.Configuration;

namespace DogsServer.Controllers
{
    public abstract class BlobUploaderControllerBase : BaseController
    {
        protected readonly IBlobStorageService BlobStorageService;
        public BlobUploaderControllerBase(IUserService userService, IBlobStorageService blobService)
            : base(userService)
        {
            BlobStorageService = blobService;
        }
        protected async Task<IActionResult> Upload(string blobContainerName)
        {
            try
            {
                Request.EnableBuffering();
                var uploadSuccess = false;
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var formFile in Request.Form.Files)
                    {
                        if (formFile.Length <= 0)
                        {
                            continue;
                        }
                        //read directly from stream for blob upload      
                        using (var stream = formFile.OpenReadStream())
                        {
                            uploadSuccess = await BlobStorageService.UploadToBlob(formFile.FileName, stream, blobContainerName);
                        }
                    }
                }

                if (uploadSuccess)
                {
                    return Ok("UploadSuccess");
                }
                else
                    return BadRequest("UploadError");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
