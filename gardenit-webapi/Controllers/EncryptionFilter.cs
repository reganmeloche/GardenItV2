using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace gardenit_webapi.Controllers
{
    public class EncryptionFilterOptions {
        public string EncryptionKey { get; set; }
    }


    public class EncryptionFilterAttribute : ActionFilterAttribute
    {
        private readonly string _encryptionKey;

        public EncryptionFilterAttribute(IOptions<EncryptionFilterOptions> options) {
            _encryptionKey = options.Value.EncryptionKey;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            // Check header for password
            try {
                var encryptedUserId = context.HttpContext.Request.Headers["UserId"].FirstOrDefault();
                var iv = context.HttpContext.Request.Headers["iv"].FirstOrDefault();
                var userId = Decrypt(encryptedUserId, iv, _encryptionKey);

                context.HttpContext.Request.Headers["UserId"] = userId;
            } catch (Exception e) {
                throw new UnauthorizedAccessException($"Not authorized: {e.Message}");
            }
        }
        
        private static Aes CreateCipher(string keyBase64)
        {
            // Default values: Keysize 256, Padding PKC27
            Aes cipher = Aes.Create();
            cipher.Mode = CipherMode.CBC;  // Ensure the integrity of the ciphertext if using CBC
        
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = Convert.FromBase64String(keyBase64);
        
            return cipher;
        }

        public string Decrypt(string encryptedText, string IV, string encryptionKey)
        {
            var bytes = Encoding.UTF8.GetBytes(encryptionKey);
            var key = Convert.ToBase64String(bytes);
            Aes cipher = CreateCipher(key);
            cipher.IV = Convert.FromBase64String(IV);
            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}