

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ASF_OneBot.API.Integration
{

    [UsedImplicitly]
    internal sealed class CustomAttributesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            ArgumentNullException.ThrowIfNull(schema);
            ArgumentNullException.ThrowIfNull(context);

            ICustomAttributeProvider attributesProvider;

            if (context.MemberInfo != null)
            {
                attributesProvider = context.MemberInfo;
            }
            else if (context.ParameterInfo != null)
            {
                attributesProvider = context.ParameterInfo;
            }
            else
            {
                return;
            }

            foreach (CustomSwaggerAttribute customSwaggerAttribute in attributesProvider.GetCustomAttributes(typeof(CustomSwaggerAttribute), true))
            {
                customSwaggerAttribute.Apply(schema);
            }
        }
    }
}
