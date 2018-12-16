using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Strathweb.AspNetCore.AzureBlobFileProvider;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class TrainingsController : ControllerBase
    {

        AzureBlobFileProvider tracksBlobProvider;
        AzureBlobFileProvider imagesBlobProvider;
        //const string SessionName = "_Name";
        //const string SessionAge = "_Age";

        public TrainingsController(AzureBlobFileProvider tracks, AzureBlobFileProvider images)
        {
            tracksBlobProvider = tracks;
            imagesBlobProvider = images;
        }
        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public HttpResponseMessage Upload()
        {
            
            HttpResponseMessage result = null;
            if (Request.Form.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (var postedFile in Request.Form.Files)
                {
                    Console.WriteLine(postedFile.FileName);
                }
                //result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                //result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }
    }
}