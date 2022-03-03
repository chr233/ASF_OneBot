using System;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;

namespace ASF_OneBot.API.Integration
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    [PublicAPI]
    public abstract class CustomSwaggerAttribute : Attribute
    {
        public abstract void Apply(OpenApiSchema schema);
    }
}
