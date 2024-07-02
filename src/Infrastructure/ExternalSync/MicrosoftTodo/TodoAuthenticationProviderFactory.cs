using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.Kiota.Abstractions.Authentication;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

public static class TodoAuthenticationProviderFactory
{
    private static readonly string[] scopes = ["user.read", "tasks.readwrite", "tasks.readwrite.shared", "offline_access"];
    private static readonly PublicClientApplicationOptions clientOptions = new()
    {
        AzureCloudInstance = AzureCloudInstance.AzurePublic,
        TenantId = "common",
        ClientId = "014d12e9-ba6f-45ad-ae8b-afc7d924b847"
    };
    public static async Task<IAuthenticationProvider> GetAuthenticationProvider()
    {
        var publicClient = PublicClientApplicationBuilder
            .CreateWithApplicationOptions(clientOptions)
            .WithDefaultRedirectUri()
            .Build();

        await RegisterCache(publicClient.UserTokenCache);

        var existingAccounts = await publicClient.GetAccountsAsync();
        string token = string.Empty;
        try
        {
            if (!existingAccounts.Any())
            {
                var authResult = await publicClient.AcquireTokenWithDeviceCode(scopes, async (result) =>
                {
                    Console.WriteLine(result.Message);
                    await Task.CompletedTask;
                }).ExecuteAsync();
                token = authResult.AccessToken;
            }
            else
            {
                var authResult = await publicClient.AcquireTokenSilent(scopes, existingAccounts.FirstOrDefault()).ExecuteAsync();
                token = authResult.AccessToken;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

        IAccessTokenProvider deviceCodeTokenProvider = new DeviceCodeTokenProvider(token);
        return new BaseBearerTokenAuthenticationProvider(deviceCodeTokenProvider);
    }

    public static async Task ClearCachedAccounts()
    {
        var publicClient = PublicClientApplicationBuilder
            .CreateWithApplicationOptions(clientOptions)
            .WithDefaultRedirectUri()
            .Build();
        await RegisterCache(publicClient.UserTokenCache);
        var accounts = await publicClient.GetAccountsAsync();
        foreach (var account in accounts)
        {
            await publicClient.RemoveAsync(account);
        }

        Console.WriteLine($"Cached credentials cleared for {accounts.Count()} accounts");
    }

    private static async Task RegisterCache(ITokenCache userTokenCache)
    {
        var storageProperties =
    new StorageCreationPropertiesBuilder(Settings.CacheFileName, Settings.CacheDir)
            .WithLinuxKeyring(
                Settings.LinuxKeyRingSchema,
                Settings.LinuxKeyRingCollection,
                Settings.LinuxKeyRingLabel,
                Settings.LinuxKeyRingAttr1,
                Settings.LinuxKeyRingAttr2)
            .WithMacKeyChain(
                Settings.KeyChainServiceName,
                Settings.KeyChainAccountName)
            .Build();
        var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
        cacheHelper.RegisterCache(userTokenCache);

    }
}
