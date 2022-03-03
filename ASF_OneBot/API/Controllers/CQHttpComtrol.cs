using ASF_OneBot.API.Requests;
using ASF_OneBot.API.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ASF_OneBot.API.Controllers
{
    [Route("Api")]
    internal class CQHttpComtrol : BotControllerBase
    {
        /// <summary>
        ///     Updates ASF's global config.
        /// </summary>
        [Consumes("application/json")]
        [HttpGet("test")]
        [ProducesResponseType(typeof(GenericResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(GenericResponse), (int)HttpStatusCode.BadRequest)]
        public ActionResult<GenericResponse> ASFPost([FromHeader] ASFRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            bool result = request.test == "123";

            return Ok(new GenericResponse(result, request.test));
        }

    }
}
