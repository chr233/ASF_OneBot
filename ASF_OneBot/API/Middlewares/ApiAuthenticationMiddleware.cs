﻿
//using ArchiSteamFarm.Core;
//using ArchiSteamFarm.IPC;
//using ArchiSteamFarm.Storage;
using ASF_OneBot.API.Responses;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ASF_OneBot.API.Middlewares;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class ApiAuthenticationMiddleware
{
    internal const string HeadersField = "Authentication";

    private const byte FailedAuthorizationsCooldownInHours = 1;
    private const byte MaxFailedAuthorizationAttempts = 5;

    private static readonly ConcurrentDictionary<IPAddress, Task> AuthorizationTasks = new();
    private static readonly Timer ClearFailedAuthorizationsTimer = new(ClearFailedAuthorizations);
    private static readonly ConcurrentDictionary<IPAddress, byte> FailedAuthorizations = new();

    private readonly ForwardedHeadersOptions ForwardedHeadersOptions;
    private readonly RequestDelegate Next;

    public ApiAuthenticationMiddleware(RequestDelegate next, IOptions<ForwardedHeadersOptions> forwardedHeadersOptions)
    {
        Next = next ?? throw new ArgumentNullException(nameof(next));

        ArgumentNullException.ThrowIfNull(forwardedHeadersOptions);

        ForwardedHeadersOptions = forwardedHeadersOptions.Value ?? throw new InvalidOperationException(nameof(forwardedHeadersOptions));

        lock (FailedAuthorizations)
        {
            ClearFailedAuthorizationsTimer.Change(TimeSpan.FromHours(FailedAuthorizationsCooldownInHours), TimeSpan.FromHours(FailedAuthorizationsCooldownInHours));
        }
    }

    [UsedImplicitly]
    public async Task InvokeAsync(HttpContext context, IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(jsonOptions);

        (HttpStatusCode statusCode, bool permanent) = await GetAuthenticationStatus(context).ConfigureAwait(false);

        if (statusCode == HttpStatusCode.OK)
        {
            await Next(context).ConfigureAwait(false);

            return;
        }

        context.Response.StatusCode = (int)statusCode;

        StatusCodeResponse statusCodeResponse = new(statusCode, permanent);

        await context.Response.WriteJsonAsync(new GenericResponse<StatusCodeResponse>(false, statusCodeResponse), jsonOptions.Value.SerializerSettings).ConfigureAwait(false);
    }

    private static void ClearFailedAuthorizations(object state = null) => FailedAuthorizations.Clear();

    private async Task<(HttpStatusCode StatusCode, bool Permanent)> GetAuthenticationStatus(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IPAddress clientIP = context.Connection.RemoteIpAddress;

        if (clientIP == null)
        {
            throw new InvalidOperationException(nameof(clientIP));
        }

        if (FailedAuthorizations.TryGetValue(clientIP, out byte attempts) && attempts >= MaxFailedAuthorizationAttempts)
        {
            return (HttpStatusCode.Forbidden, false);
        }

        string ipcPassword = Global.GlobalConfig.AccessToken;

        if (string.IsNullOrEmpty(ipcPassword))
        {
            if (IPAddress.IsLoopback(clientIP))
            {
                return (HttpStatusCode.OK, true);
            }

            if (ForwardedHeadersOptions.KnownNetworks.Count == 0)
            {
                return (HttpStatusCode.Forbidden, true);
            }

            if (clientIP.IsIPv4MappedToIPv6)
            {
                IPAddress mappedClientIP = clientIP.MapToIPv4();

                if (ForwardedHeadersOptions.KnownNetworks.Any(network => network.Contains(mappedClientIP)))
                {
                    return (HttpStatusCode.OK, true);
                }
            }

            return (ForwardedHeadersOptions.KnownNetworks.Any(network => network.Contains(clientIP)) ? HttpStatusCode.OK : HttpStatusCode.Forbidden, true);
        }

        if (!context.Request.Headers.TryGetValue(HeadersField, out StringValues passwords) && !context.Request.Query.TryGetValue("password", out passwords))
        {
            return (HttpStatusCode.Unauthorized, true);
        }

        string inputPassword = passwords.FirstOrDefault(static password => !string.IsNullOrEmpty(password));

        if (string.IsNullOrEmpty(inputPassword))
        {
            return (HttpStatusCode.Unauthorized, true);
        }

        //ArchiCryptoHelper.EHashingMethod ipcPasswordFormat = ASF.GlobalConfig != null ? ASF.GlobalConfig.IPCPasswordFormat : GlobalConfig.DefaultIPCPasswordFormat;

        //string inputHash = ArchiCryptoHelper.Hash(ipcPasswordFormat, inputPassword);

        bool authorized = ipcPassword == inputPassword;

        while (true)
        {
            if (AuthorizationTasks.TryGetValue(clientIP, out Task task))
            {
                await task.ConfigureAwait(false);

                continue;
            }

            TaskCompletionSource taskCompletionSource = new();

            if (!AuthorizationTasks.TryAdd(clientIP, taskCompletionSource.Task))
            {
                continue;
            }

            try
            {
                bool hasFailedAuthorizations = FailedAuthorizations.TryGetValue(clientIP, out attempts);

                if (hasFailedAuthorizations && attempts >= MaxFailedAuthorizationAttempts)
                {
                    return (HttpStatusCode.Forbidden, false);
                }

                if (!authorized)
                {
                    FailedAuthorizations[clientIP] = hasFailedAuthorizations ? ++attempts : (byte)1;
                }
            }
            finally
            {
                AuthorizationTasks.TryRemove(clientIP, out _);

                taskCompletionSource.SetResult();
            }

            return (authorized ? HttpStatusCode.OK : HttpStatusCode.Unauthorized, true);
        }
    }
}