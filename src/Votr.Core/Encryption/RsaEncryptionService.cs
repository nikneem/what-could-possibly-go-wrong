using System.Drawing.Text;
using System.Security.Cryptography;

namespace Votr.Core.Encryption;

public class RsaEncryptionService
{

    public RsaEncryptionService()
    {
        var rsa = new RSACryptoServiceProvider(2048);
        
    }
}