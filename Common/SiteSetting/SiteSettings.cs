using System;
using System.Collections.Generic;
using System.Text;

namespace Common.SiteSetting
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }

    public class JwtSettings
    {
        public string Key { get; set; }
        public string EncryptionKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforMinutes { get; set; } = 0;
        public int ExpiertionMinutes { get; set; } = 10;

    }

}
