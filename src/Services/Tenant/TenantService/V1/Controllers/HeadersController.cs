using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Siccar.Platform.Tenants.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeadersController : ControllerBase
    {

        /// <returns>Status code for operation and specified tenant</returns>

        [HttpGet]
        public ActionResult Get()
        {
            string lists = "**** \nHeaders Configuration \n";
            foreach(var h in Request.Headers)
            {
                lists += "\t" + h.Key + " : " + h.Value + "\n";
            }


            return Ok(lists);
        }
    }
}
