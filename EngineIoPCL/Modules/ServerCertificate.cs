﻿namespace Quobject.EngineIoClientDotNet.Modules
{
    public class ServerCertificate
    {
        static ServerCertificate()
        {
            Ignore = false;
        }

        public static bool Ignore { get; set; }

        public static void IgnoreServerCertificateValidation()
        {
            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            //MONOFIX

            Ignore = true;
        }
    }
}