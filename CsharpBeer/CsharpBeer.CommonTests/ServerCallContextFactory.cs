using Grpc.Core;
using Grpc.Core.Testing;

namespace CsharpBeer.CommonTests;

public static class ServerCallContextFactory
{
    public const string AuthToken = "AuthToken";
    public static readonly Metadata DefaultHeaders = [ new Metadata.Entry("Authorization", $"Bearer {AuthToken}") ];
    public static ServerCallContext Create(
        string? method = null,
        string? host = null,
        DateTime? deadline = null,
        Metadata? requestHeaders = null,
        CancellationToken? cancellationToken = null,
        string? peer = null, 
        AuthContext? authContext = null,
        ContextPropagationToken? contextPropagationToken = null,
        Func<Metadata, Task>? writeHeadersFunc = null,
        Func<WriteOptions>? writeOptionsGetter = null,
        Action<WriteOptions>? writeOptionsSetter = null) => 
        TestServerCallContext.Create(
            method ?? "default-method",
            host ?? "default-host",
            deadline.HasValue ? deadline.Value : DateTime.MaxValue,
            requestHeaders ?? null,
            cancellationToken ?? CancellationToken.None,
            peer ?? "default-peer",
            authContext ?? null,
            contextPropagationToken ?? null,
            writeHeadersFunc ?? null,
            writeOptionsGetter ?? null,
            writeOptionsSetter ?? null);
}