using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrns2XModUnitTest
{
    static class MD5Utils
    {
        public static string GenerateMd5Hash(byte[] input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
