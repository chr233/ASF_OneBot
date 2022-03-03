using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ASF_OneBot.API.Requests
{

    [SuppressMessage("ReSharper", "ClassCannotBeInstantiated")]
    public sealed class ASFRequest
    {
        /// <summary>
        ///     ASF's global config structure.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        [Required]
        public string test { get; private set; } = "";

        [JsonConstructor]
        private ASFRequest() { }
    }
}
