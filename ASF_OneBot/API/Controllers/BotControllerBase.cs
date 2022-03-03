
using System.Net;
using ASF_OneBot.Storage;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ASF_OneBot.API.Responses;

namespace ASF_OneBot.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("Api")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, $"The request has failed, check {nameof(GenericResponse.Message)} from response body for actual reason. Most of the time this is ASF, understanding the request, but refusing to execute it due to provided reason.", typeof(GenericResponse))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, $"ASF has {nameof(Config.AccessToken)} set, but you've failed to authenticate.", typeof(GenericResponse<StatusCodeResponse>))]
    [SwaggerResponse((int)HttpStatusCode.Forbidden, $"ASF lacks {nameof(Config.AccessToken)} and you're not permitted to access the API, or {nameof(Config.AccessToken)} is set and you've failed to authenticate too many times (try again in an hour).", typeof(GenericResponse<StatusCodeResponse>))]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, "ASF has encountered an unexpected error while serving the request. The log may include extra info related to this issue.")]
    [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "ASF has encountered an error while requesting a third-party resource. Try again later.")]
    public abstract class BotControllerBase : ControllerBase { }

}
