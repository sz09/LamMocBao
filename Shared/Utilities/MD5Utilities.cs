using System;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Utilities
{
    public class MD5Utilities
    {
        private static Lazy<byte[]> SALT =new Lazy<byte[]>(() => Encoding.UTF8.GetBytes("LMB_SALT_KEY")) ;

        public static string Hash(string source)
        {
            var sourceBytes = Encoding.UTF8.GetBytes(source);
            var hmacMD5 = new HMACMD5(SALT.Value);
            var saltedHash = hmacMD5.ComputeHash(sourceBytes);
            return Convert.ToBase64String(saltedHash);
        }
    }
}
