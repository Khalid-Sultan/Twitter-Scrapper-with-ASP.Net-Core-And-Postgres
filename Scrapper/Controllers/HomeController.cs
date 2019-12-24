using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using IronWebScraper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scrapper.Models;
using TweetSharp;

namespace Scrapper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        Keys Key = new Keys();
        private TwitterService service;

        private string ConsumerKey = "";
        private string ConsumerSecret = "";
        private string AccessToken = "";
        private string AccessTokenSecret = "";
        private ScrapperContext scrapperContext;

        public HomeController(ILogger<HomeController> logger, ScrapperContext context)
        {
            _logger = logger;
            scrapperContext = context;
        }
        [Route("tweets/{username}")]
        public IActionResult Tweets(string username)
        {
            ConsumerKey = Key.GetConsumerAPIKey();
            ConsumerSecret = Key.GetConsumerSecretAPIKey();
            AccessToken = Key.GetAccessTokenKey();
            AccessTokenSecret = Key.GetAccessTokenSecretKey();
            service = new TwitterService(ConsumerKey, ConsumerSecret);
            service.AuthenticateWith(AccessToken, AccessTokenSecret);
            IEnumerable<TwitterStatus> tweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
            {
                ScreenName = username,
                Count = 100,
                ExcludeReplies = true
            });
            List<UserTweets> tweet = new List<UserTweets>();
            foreach (TwitterStatus t in tweets)
            {
                tweet.Add(new UserTweets
                {                    
                    text =  t.Text,
                    retweets = t.RetweetCount
                });
            }
            return View(tweet);
        }

        public IActionResult Index()
        {
            ConsumerKey = Key.GetConsumerAPIKey();
            ConsumerSecret = Key.GetConsumerSecretAPIKey();
            AccessToken = Key.GetAccessTokenKey();
            AccessTokenSecret = Key.GetAccessTokenSecretKey();
            service = new TwitterService(ConsumerKey, ConsumerSecret);
            service.AuthenticateWith(AccessToken, AccessTokenSecret);
            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
            if(scrapperContext.backlogs.Count()==0 && scrapperContext.users.Count() == 0)
            {
                keyValuePairs = new Utils().country_top_users;
                foreach (List<string> vs in keyValuePairs.Values)
                {
                    vs.ForEach(x => scrapperContext.users.Add(ScrapData(x)));
                }
                scrapperContext.SaveChanges();
                List<User> c = scrapperContext.users.ToList();
                return View(c);

            }
            if (scrapperContext.backlogs.Count() != 0)
            {
                List<Backlog> values = scrapperContext.backlogs.ToList();
                List<User> users = scrapperContext.users.ToList();

                List<string> usernames = new List<string>();
                foreach(Backlog backlog in values)
                {
                    usernames.Add(backlog.ScreenName);
                    scrapperContext.backlogs.Remove(backlog);
                    scrapperContext.SaveChanges();
                }
                keyValuePairs.Add("Random", usernames);

                List<string> usernames2 = new List<string>();
                foreach(User u in users)
                {
                    usernames2.Add(u.ScreenName);
                    scrapperContext.users.Remove(u);
                    scrapperContext.SaveChanges();
                }
                keyValuePairs.Add("Old", usernames2);
            }
            if (scrapperContext.users.Count() == 0)
            {
                foreach (List<string> vs in keyValuePairs.Values)
                {
                    vs.ForEach(x => scrapperContext.users.Add(ScrapData(x)));
                    scrapperContext.SaveChanges();
                }
            }
            scrapperContext.users.ToList().ForEach(x =>
            {
                if (x.Name == null)
                {
                    scrapperContext.users.Remove(x);
                    scrapperContext.SaveChanges();
                }
            });
            List<User> collection = scrapperContext.users.ToList();
            return View(collection);
        }
        public User ScrapData(string screenName)
        {
            User user_obj = new User
            {
                ScreenName = screenName
            };
            try
            {
                TwitterUser user = service.GetUserProfileFor(new GetUserProfileForOptions
                {
                    ScreenName = screenName
                });
                user_obj.Name = user.Name;
                user_obj.Location = user.Location;
                user_obj.Description = user.Description;
                user_obj.Verified = user.IsVerified == null ? false : user.IsVerified == true;
                user_obj.FollowersCount = user.FollowersCount;
                user_obj.FriendsCount = user.FriendsCount;
                user_obj.StatusesCount = user.StatusesCount;
                user_obj.ProfileImageUrlHttps = user.ProfileImageUrlHttps;
                //IEnumerable<TwitterStatus> tweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
                //{
                //    ScreenName = screenName,
                //    Count = 500,
                //    ExcludeReplies = true
                //});

                //foreach (TwitterStatus i in tweets)
                //{
                //    if (i == null || i.User == null || i.Text == null || i.Text.Length < 5) continue;
                //    user_obj.TotalNumberOfLikes += i.User.FavouritesCount;
                //    user_obj.TotalNumberOfRetweets += i.RetweetCount;
                //    user_obj.FollowersCount = i.User.FollowersCount;
                //}
            }
            catch { }
            return user_obj;
        }

        public IActionResult AddUser()
        {
            var model = new User();
            return PartialView("_AddUser", model);
        }

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            List<User> users = scrapperContext.users.ToList();
            users = users.Where(x => x.ScreenName == user.ScreenName).ToList();
            if (users.Count == 0)
            {
                scrapperContext.backlogs.Add(new Backlog { ScreenName = user.ScreenName });
                scrapperContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public void DeleteUser(string screenName)
        {
            try
            {
                scrapperContext.users.Remove(scrapperContext.users.FirstOrDefault(x => x.ScreenName == screenName));
                scrapperContext.SaveChanges();
            }
            catch
            {
            }
        }
        public User GetUser(string screenName)
        {
            return scrapperContext.users.FirstOrDefault(x => x.ScreenName == screenName);
        }

        public IActionResult Details(string screenName)
        {
            return View();
        }

        public void RefreshDatabase()
        {

        }



        //XML FILE MODIFICATION

        //public IActionResult Index()
        //{
        //    ConsumerKey = Key.GetConsumerAPIKey();
        //    ConsumerSecret = Key.GetConsumerSecretAPIKey();
        //    AccessToken = Key.GetAccessTokenKey();
        //    AccessTokenSecret = Key.GetAccessTokenSecretKey();
        //    service = new TwitterService(ConsumerKey, ConsumerSecret);
        //    service.AuthenticateWith(AccessToken, AccessTokenSecret);

        //    List<Models.User_Tweets> collection = new List<Models.User_Tweets>();
        //    XmlDocument doc = new XmlDocument();
        //    try
        //    {
        //        doc.Load("data.xml");
        //        XmlNodeList nodeList = doc.DocumentElement.ChildNodes;
        //        foreach (XmlNode node in nodeList)
        //        {
        //            XmlNode usernode = node.SelectSingleNode("User");
        //            Models.User_Tweets user_Tweets = new Models.User_Tweets();
        //            user_Tweets.user = new Models.User();
        //            user_Tweets.user.FollowersCount = Convert.ToInt32(usernode.SelectSingleNode("Followers").InnerText);
        //            user_Tweets.user.TotalNumberOfRetweets = Convert.ToInt32(usernode.SelectSingleNode("Retweets").InnerText);
        //            user_Tweets.user.TotalNumberOfLikes = Convert.ToInt32(usernode.SelectSingleNode("Likes").InnerText);
        //            user_Tweets.user.ScreenName = usernode.SelectSingleNode("ScreenName").InnerText;
        //            XmlNode tweets_node = node.SelectSingleNode("Tweets");
        //            foreach (XmlNode tweet in tweets_node)
        //            {
        //                user_Tweets.tweets.Add(tweet.InnerText);
        //            }
        //            collection.Add(user_Tweets);
        //        }
        //    }
        //    catch (FileNotFoundException ex)
        //    {
        //        ScrapeWebsite();
        //        return RefreshPage();
        //    }
        //    return View(collection);
        //}

        //internal async void ScrapeWebsite()
        //{
        //    List<Models.User_Tweets> user_Tweets = new List<Models.User_Tweets>();
        //    Dictionary<string,List<string>> country_top_users = new Dictionary<string,List<string>>();
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load("accounts.xml");
        //    XmlNodeList nodeList = doc.DocumentElement.ChildNodes;
        //    foreach(XmlNode node in nodeList)
        //    {
        //        XmlNode countryNode = node.SelectSingleNode("Name");
        //        XmlNode accountsNode = node.SelectSingleNode("Accounts");
        //        List<string> accounts = new List<string>();
        //        foreach(XmlNode account in accountsNode)
        //        {
        //            accounts.Add(account.InnerText);
        //        }
        //        country_top_users.Add(countryNode.InnerText, accounts);
        //    }
        //    foreach (string country in country_top_users.Keys)
        //    {
        //        foreach (string user in country_top_users[country])
        //        {
        //            try
        //            {
        //                Models.User_Tweets tweets = new Models.User_Tweets();
        //                Models.User user_obj = new Models.User();
        //                user_obj.ScreenName = user;
        //                IEnumerable<TwitterStatus> currentTweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
        //                {
        //                    ScreenName = user,
        //                    Count = 500,
        //                    ExcludeReplies = true
        //                });

        //                foreach (TwitterStatus i in currentTweets)
        //                {
        //                    if (i == null || i.User == null || i.Text == null || i.Text.Length < 5) continue;
        //                    user_obj.TotalNumberOfLikes += i.User.FavouritesCount;
        //                    user_obj.TotalNumberOfRetweets += i.RetweetCount;
        //                    user_obj.FollowersCount = i.User.FollowersCount;
        //                    tweets.tweets.Add(i.Text);
        //                }
        //                tweets.user = user_obj;
        //                user_Tweets.Add(tweets);

        //            }
        //            catch { }
        //        }
        //    }

        //    List<XElement> eleList = new List<XElement>();

        //    foreach (Models.User_Tweets i in user_Tweets)
        //    {
        //        eleList.Add(
        //            new XElement("User_T",
        //                new XElement("Tweets",
        //                    i.tweets.Select(j => new XElement("tweet", j))
        //                ),
        //                new XElement("User",
        //                    new XElement("ScreenName", i.user.ScreenName),
        //                    new XElement("Retweets", i.user.TotalNumberOfRetweets),
        //                    new XElement("Likes", i.user.TotalNumberOfLikes),
        //                    new XElement("Followers", i.user.FollowersCount)
        //                )
        //            )
        //        );
        //    }
        //    XDocument xml = new XDocument(
        //        new XDeclaration("1.0", "UTF-8", null),
        //        new XElement("Root", eleList));
        //    StringBuilder sb = new StringBuilder();

        //    foreach (var node in xml.Nodes())
        //    {
        //        sb.Append(node.ToString());
        //    }

        //    using StreamWriter file = new StreamWriter(@"data.xml");
        //    await file.WriteLineAsync(sb.ToString());
        //}

        //[HttpPost]
        //public IActionResult AddUser(string country, string screenName)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load("accounts.xml");
        //    XmlNodeList nodeList = doc.DocumentElement.ChildNodes;
        //    foreach (XmlNode node in nodeList)
        //    {
        //        if (node.SelectSingleNode("Name").InnerText.ToLower() == country.ToLower())
        //        {
        //            XmlElement i = doc.CreateElement("Account");
        //            i.InnerText = screenName;
        //            XmlNode accounts = node.SelectSingleNode("Accounts");
        //            foreach(XmlNode x in accounts){
        //                if (x.InnerText == screenName) return RefreshPage();
        //            }
        //            accounts.AppendChild(i);
        //            doc.Save("accounts.xml");
        //            return RefreshPage();
        //        }
        //    }
        //    XmlElement temp = doc.CreateElement("Country");
        //    temp.InnerXml = $"<Name>{country}</Name><Account>{screenName}</Account>";
        //    doc.DocumentElement.AppendChild(temp);
        //    doc.Save("accounts.xml");
        //    return RefreshPage();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
