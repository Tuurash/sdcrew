using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using ChoETL;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using sdcrew.Models;
using sdcrew.Services;

namespace sdcrew.Repositories
{
    public class MiscRepository
    {
        private readonly IRequestsService _requestService;

        MiscEndPoints misc;

        public MiscRepository()
        {
            misc = new MiscEndPoints();
        }

        //RssNotifications
        public async Task<List<RssNotification>> FetchAllRss()
        {
            XDocument doc = new XDocument();
            List<RssNotification> notifications = new List<RssNotification>();
            try
            {
                string url = misc.RSS_NOTIFICATIONS;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Accept = "application/xml"; // <== THIS FIXED IT


                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        doc = XDocument.Load(stream);
                    }
                }

                string xmlstring = doc.ToString();

                var JsonString = Xml2JSON(xmlstring);

                JsonString = JsonString.Remove(0, 1);
                JsonString = JsonString.Remove(JsonString.Length - 1, 1);

                var Allrss = new Root();
                Allrss = JsonConvert.DeserializeObject<Root>(JsonString);


                await Task.Delay(0);
                foreach (var item in Allrss.Items)
                {
                    var notification = new RssNotification
                    {
                        Title = item.Title,
                        Category = item.Category,
                        HtmlDescription = item.Description,
                        Description = GetItemBetween("class=\"OutageDescription\">", "</pre>", item.Description),
                        ImgUri = new Uri("https://maps.satcomdirect.com/GetMap?" + GetItemBetween("https://maps.satcomdirect.com/GetMap?", "FGCOLOR=0xDEE1DD", item.Description) + "FGCOLOR=0xDEE1DD"),
                    };

                    notifications.Add(notification);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return notifications;
        }

        public string GetItemBetween(string startKyword, string endKyword, string txt)
        {
            StringBuilder sb = new StringBuilder(txt);
            int pos1 = txt.IndexOf(startKyword) + startKyword.Length;
            int len = (txt.Length) - pos1;

            string reminder = txt.Substring(pos1, len);

            int pos2 = reminder.IndexOf(endKyword); // - endKyword.Length + 1

            return reminder.Substring(0, pos2);
        }

        public static string Xml2JSON(string xml)
        {
            StringBuilder json = new StringBuilder();
            using (var r = ChoXmlReader.LoadText(xml))
            {
                using (var w = new ChoJSONWriter(json)
                      )
                    w.Write(r);
            }

            Console.WriteLine(json.ToString());
            return json.ToString();
        }
    }
}
