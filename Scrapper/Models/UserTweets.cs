using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;
using TweetSharp;

namespace Scrapper.Models
{
    [Serializable]
    [XmlRoot("User_T")]
    public class UserTweets
    {
        public int UserTweetsId { get; set; }

        public string text { get; set; }

        public int retweets { get; set; }

        //[ForeignKey(nameof(User))]
        //public int UserId { get; set; }
        //[JsonProperty]
        //[XmlElement("User")]
        //public User user;
        //[JsonProperty]
        //[XmlAttribute("Tweets")]
        //public List<string> tweets = new List<string>();
    }

}
