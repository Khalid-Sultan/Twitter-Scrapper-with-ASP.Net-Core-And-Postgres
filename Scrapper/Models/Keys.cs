using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper.Models
{
    public class Keys
    {
        public Keys()
        {

        }
        private const string _ConsumerAPIKey = "1pr7DuaZRuJj6fP5vkFHzeOvI";
        private const string _ConsumerSecretAPIKey = "s3rVZWid5jmGoCahF8Gk5uWr94CY1bQcDpjg1IjwAwtSY3NFQB";
        private const string _AccessTokenKey = "1206081156317417472-kxOJYbSiTB7bA7VrcGRpKg2AbBSrp0";
        private const string _AccessTokenSecretKey = "bEqyvnijfu6Bmvzszppb1teJ6I9oZMIJXGEx7m04uM0TW";

        public string GetConsumerAPIKey() => _ConsumerAPIKey;
        public string GetConsumerSecretAPIKey() => _ConsumerSecretAPIKey;
        public string GetAccessTokenKey() => _AccessTokenKey;
        public string GetAccessTokenSecretKey() => _AccessTokenSecretKey;

    }
}
