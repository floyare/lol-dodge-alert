using System;
using System.Management;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace lol_dodge_alert
{
    /// <summary>
    /// Riot Auth system
    /// </summary>
    public class ValueTuple
    {
        private static readonly string string_0 = "\"--remoting-auth-token=(?'token'.*?)\" | \"--app-port=(?'port'|.*?)\"";
        private static readonly RegexOptions regexOptions_0 = RegexOptions.Multiline;
        public ValueTuple<string, string> GetInfo()
        {
            string text = "";
            string text2 = "";
            foreach (ManagementBaseObject managementBaseObject in new ManagementClass("Win32_Process").GetInstances())
            {
                ManagementObject managementObject = (ManagementObject)managementBaseObject;
                if (managementObject["Name"].Equals("LeagueClientUx.exe"))
                {
                    foreach (object obj in Regex.Matches(managementObject["CommandLine"].ToString(), ValueTuple.string_0, ValueTuple.regexOptions_0))
                    {
                        Match match = (Match)obj;
                        if (!string.IsNullOrEmpty(match.Groups["port"].ToString()))
                        {
                            text2 = match.Groups["port"].ToString();
                        }
                        else if (!string.IsNullOrEmpty(match.Groups["token"].ToString()))
                        {
                            text = match.Groups["token"].ToString();
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2)){}
            return new ValueTuple<string, string>(text, text2);
        }

        /// <summary>
        /// Auth token placeholder
        /// </summary>
        public class Security
        {
            internal bool Main(object object_0, X509Certificate x509Certificate_0, X509Chain x509Chain_0, SslPolicyErrors sslPolicyErrors_0)
            {
                return true;
            }

            public static byte[] token = { 0x62, 0x79, 0x20, 0x66, 0x6c, 0x6f, 0x79, 0x61, 0x72, 0x65, 0x20, 0x7c, 0x20, 0x30, 0x2e, 0x31};

            public static readonly ValueTuple.Security Sec = new ValueTuple.Security();

            private static RemoteCertificateValidationCallback callback;
        }
    }
}
