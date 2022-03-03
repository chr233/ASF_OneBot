
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ASF_OneBot.API;

internal static class WebUtilities
{
    internal static string GetUnifiedName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.GenericTypeArguments.Length == 0 ? type.FullName : $"{type.Namespace}.{type.Name}{string.Join("", type.GenericTypeArguments.Select(static innerType => $"[{innerType.GetUnifiedName()}]"))}";
    }

    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode", Justification = "We don't care about trimmed assemblies, as we need it to work only with the known (used) ones")]
    internal static Type ParseType(string typeText)
    {
        if (string.IsNullOrEmpty(typeText))
        {
            throw new ArgumentNullException(nameof(typeText));
        }

        Type targetType = Type.GetType(typeText);

        if (targetType != null)
        {
            return targetType;
        }

        // We can try one more time by trying to smartly guess the assembly name from the namespace, this will work for custom libraries like SteamKit2
        int index = typeText.IndexOf('.', StringComparison.Ordinal);

        if (index <= 0 || index >= typeText.Length - 1)
        {
            return null;
        }

        return Type.GetType($"{typeText},{typeText[..index]}");
    }

    internal static async Task WriteJsonAsync<TValue>(this HttpResponse response, TValue? value, JsonSerializerSettings jsonSerializerSettings = null)
    {
        ArgumentNullException.ThrowIfNull(response);

        JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSerializerSettings);

        response.ContentType = "application/json; charset=utf-8";

        StreamWriter streamWriter = new(response.Body, Encoding.UTF8);

        await using (streamWriter.ConfigureAwait(false))
        {
            using JsonTextWriter jsonWriter = new(streamWriter) {
                CloseOutput = false
            };

            serializer.Serialize(jsonWriter, value);
        }
    }
}
