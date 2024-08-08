using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System;
using System.Security.Claims;

namespace AvtoMigBussines.Authenticate
{
    public static class JwtTokenGenerator
    {
        public static string GenerateJwtToken(string teamId, string keyId, string privateKeyBase64)
        {
            try
            {
                var ecdsa = ECDsa.Create();
                ecdsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);

                var securityKey = new ECDsaSecurityKey(ecdsa)
                {
                    KeyId = keyId
                };

                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

                // Create JWT header without 'typ' field
                var header = new JwtHeader(credentials);
                header.Remove("typ");  // Ensure 'typ' is not included
                header["kid"] = keyId;

                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var payload = new JwtPayload
            {
                { "iss", teamId },
                { "iat", now }
            };

                var secToken = new JwtSecurityToken(header, payload);
                var handler = new JwtSecurityTokenHandler();

                return handler.WriteToken(secToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating JWT token: {ex.Message}");
                throw;
            }
        }
    }


}
