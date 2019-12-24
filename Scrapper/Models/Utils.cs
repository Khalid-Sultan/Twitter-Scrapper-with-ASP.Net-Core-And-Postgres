using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper.Models
{
    class Utils
    {
        public Dictionary<string, List<string>> country_top_users;
        public Utils()
        {
            country_top_users = new Dictionary<string, List<string>>();
            country_top_users.Add("United States Of America", new List<string>() 
            {
                "BarackObama", "katyperry", "justinbieber", "realDonaldTrump"
            });
            country_top_users.Add("Russia", new List<string>()
            {
                "MedvedevRussia", "KremlinRussia",  "channelone_rus", "RT_com"
            });
            country_top_users.Add("China", new List<string>()
            {
                "XHNews", "globaltimesnews", "XinhuaChinese"
            });
            country_top_users.Add("France", new List<string>()
            {
                "davidguetta", "lemondefr"
            });
            country_top_users.Add("Ethiopia", new List<string>()
            {
                "Jawar_Mohammed", "addisstandard", "AbiyAhmedAli", "UNICEFEthiopia"
            });
        }

    }
}
