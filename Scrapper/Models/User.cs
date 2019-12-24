using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using TweetSharp;

namespace Scrapper.Models
{
    [Serializable]
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }
        [Required]
        public string ScreenName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool Verified { get; set; }
        public int FollowersCount { get; set; } = 0;
        public int FriendsCount { get; set; } = 0;
        public int StatusesCount { get; set; } = 0;
        public string ProfileImageUrlHttps { get; set; }

        //[Required]
        //[JsonProperty]
        //[XmlAttribute("ScreenName")]
        //public string ScreenName { get; set; }

        //[JsonProperty]
        //[XmlAttribute("Retweets")]
        //public int TotalNumberOfRetweets { get; set; } = 0;

        //[JsonProperty]
        //[XmlAttribute("Likes")]
        //public int TotalNumberOfLikes { get; set; } = 0;

        //[JsonProperty]
        //[XmlAttribute("Followers")]
        //public int FollowersCount { get; set; } = 0;
    }
}
