using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Android.Content;

namespace OGSAndroid
{
    internal class Data
    {
        public SecureString[] CiDsEc(Context context)
        {
            //Doesnt need to be secure, I just don't want it in plain text.

            string text;
            using (var str = context.Resources.OpenRawResource(Resource.Raw.data))
            using (var fr = new StreamReader(str))
                text = fr.ReadToEnd();
            text = Decrypt(text);
            var secureString = new SecureString[2];
            var split = text.Split(':');
            text = string.Empty;
            for (var i = 0; i < 2; i++)
            {
                foreach (var s in split[i])
                {
                    secureString[i].AppendChar(s);
                }
            }

            return secureString;
        }

        private static string Decrypt(string s)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);

            var tDes = new TripleDESCryptoServiceProvider
            {
                Key = System.Text.Encoding.UTF8.GetBytes("Banana42hJ51F4z2"),
                Mode = CipherMode.CFB,
                Padding = PaddingMode.ISO10126
            };
            var cD = tDes.CreateDecryptor();
            var outB = cD.TransformFinalBlock(bytes,0,bytes.Length);

            tDes.Clear();        
            return Encoding.UTF8.GetString(outB);
        }
    }
}