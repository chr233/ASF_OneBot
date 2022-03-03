using System.ComponentModel.DataAnnotations;
using System.Net;
using Newtonsoft.Json;

namespace ASF_OneBot.API.Responses;

public sealed class StatusCodeResponse
{
    /// <summary>
    ///     Value indicating whether the status is permanent. If yes, retrying the request with exactly the same payload doesn't make sense due to a permanent problem (e.g. ASF misconfiguration).
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [Required]
    public bool Permanent { get; private set; }

    /// <summary>
    ///     Status code transmitted in addition to the one in HTTP spec.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [Required]
    public HttpStatusCode StatusCode { get; private set; }

    internal StatusCodeResponse(HttpStatusCode statusCode, bool permanent)
    {
        StatusCode = statusCode;
        Permanent = permanent;
    }
}
