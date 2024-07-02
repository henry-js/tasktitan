using Microsoft.Kiota.Abstractions.Authentication;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

internal class DeviceCodeTokenProvider : IAccessTokenProvider
{
    private readonly string token = string.Empty;

    public DeviceCodeTokenProvider(string token)
    {
        this.token = token;
    }

    public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator();

    public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(token);
    }
}
