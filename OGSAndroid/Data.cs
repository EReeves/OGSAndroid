using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Android.Content;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Security;

namespace OGSAndroid
{
    class Data
    {
        public SecureString[] CiDsEc(Context context)
        {
            //Doesnt need to be secure, I just don't want it in plain text.

            string text;
            using(var str = context.Resources.OpenRawResource(Resource.Raw.data))
            using(var fr = new StreamReader(str))
                text = fr.ReadToEnd();
            text = Decrypt(text);
            var secureString = new SecureString[2];
            var split = text.Split(':');
            text = string.Empty;
            for(var i=0;i<2;i++)
            {
                foreach (var s in split[i])
                {
                    secureString[i].AppendChar(s);
                }
            }
           
            return secureString;
        }

        private string Decrypt(string s)
        {
            var bytes = Convert.FromBase64String(s);
            var tDes = new TripleDESCryptoServiceProvider
            {
                Key = Convert.FromBase64String("Bananas"),
                Mode = CipherMode.OFB,
                Padding = PaddingMode.ISO10126
            };
            var cT = tDes.CreateDecryptor();
            var outB = cT.TransformFinalBlock(bytes, 0, bytes.Length);
            tDes.Clear();
            return Encoding.UTF8.GetString(outB);
        }
    }
}