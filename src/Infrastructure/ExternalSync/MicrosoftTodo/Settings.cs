using Microsoft.Identity.Client.Extensions.Msal;

namespace TaskTitan.Infrastructure.ExternalSync.MicrosoftTodo;

internal static class Settings
{
    // Cache settings
    public const string CacheFileName = "myapp_msal_cache.txt";
    public readonly static string CacheDir = MsalCacheHelper.UserRootDirectory;

    public const string KeyChainServiceName = "myapp_msal_service";
    public const string KeyChainAccountName = "myapp_msal_account";

    public const string LinuxKeyRingSchema = "com.contoso.devtools.tokencache";
    public const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
    public const string LinuxKeyRingLabel = "MSAL token cache for all Contoso dev tool apps.";
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new KeyValuePair<string, string>("Version", "1");
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new KeyValuePair<string, string>("ProductGroup", "MyApps");
}
