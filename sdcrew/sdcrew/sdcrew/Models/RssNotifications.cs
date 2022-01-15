using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace sdcrew.Models
{

    [XmlRoot(ElementName = "guid")]
    public class Guid
    {

        [XmlAttribute(AttributeName = "isPermaLink")]
        public bool IsPermaLink { get; set; }

        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "link")]
        public string Link { get; set; }

        [XmlElement(ElementName = "category")]
        public string Category { get; set; }

        [XmlElement(ElementName = "pubDate")]
        public DateTime PubDate { get; set; }

        [XmlElement(ElementName = "guid")]
        public Guid Guid { get; set; }

        [XmlElement(ElementName = "mapurl")]
        public string Mapurl { get; set; }
    }

    [XmlRoot(ElementName = "channel")]
    public class Channel
    {

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "link")]
        public string Link { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "item")]
        public List<Item> Item { get; set; }
    }

    [XmlRoot(ElementName = "rss")]
    public class Rss
    {

        [XmlElement(ElementName = "channel")]
        public Channel Channel { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public double Version { get; set; }

        [XmlAttribute(AttributeName = "SD")]
        public string SD { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    public class Root
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }


    public class RssNotification
    {
        public string Title { get; set; }

        public string HtmlDescription { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public string Category { get; set; }

        public DateTime PubDate { get; set; }

        public Guid Guid { get; set; }

        public string Mapurl { get; set; }

        public Uri ImgUri { get; set; }

    }
}
