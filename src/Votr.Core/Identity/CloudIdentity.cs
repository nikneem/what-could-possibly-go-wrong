using Azure.Core;
using Azure.Identity;

namespace Votr.Core.Identity;

public class CloudIdentity
{
    public static TokenCredential GetCloudIdentity()
    {
        return new DefaultAzureCredential();
//        var identities = new List<TokenCredential>() { new ManagedIdentityCredential() };
//#if DEBUG
//        identities.Add(new AzureCliCredential());
////        identities.Add(new VisualStudioCredential());
//#endif
//        return new ChainedTokenCredential(identities.ToArray());
    }
}