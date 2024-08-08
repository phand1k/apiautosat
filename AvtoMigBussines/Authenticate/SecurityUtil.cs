using System.Security.Cryptography;

namespace AvtoMigBussines.Authenticate
{
    public static class SecurityUtil
    {
        public static ECDsa GetPrivateKey(string filename)
        {
            try
            {
                var content = File.ReadAllText(filename);
                var privateKey = content
                    .Replace("-----BEGIN PRIVATE KEY-----", "")
                    .Replace("-----END PRIVATE KEY-----", "")
                    .Replace("\n", "")
                    .Replace("\r", "");

                var privateKeyBytes = Convert.FromBase64String(privateKey);
                var ecdsa = ECDsa.Create();
                ecdsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                return ecdsa;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while loading the private key", e);
            }
        }
    }
}
