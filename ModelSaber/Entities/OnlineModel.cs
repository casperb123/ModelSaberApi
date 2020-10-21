using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelSaber.Entities
{
    public class OnlineModel
    {
        private string bsaber;

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("bsaber")]
        public string Bsaber
        {
            get
            {
                if (string.IsNullOrWhiteSpace(bsaber))
                    return null;
                else
                    return bsaber;
            }
            set { bsaber = value; }
        }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("discordid")]
        public string Discordid { get; set; }

        [JsonProperty("discord")]
        public object Discord { get; set; }

        [JsonProperty("variationid")]
        public object Variationid { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("download")]
        public string Download { get; set; }

        [JsonProperty("install_link")]
        public string InstallLink { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        public string BaseUrl
        {
            get
            {
                int lastSlash = Download.LastIndexOf("/");
                int endIndex = Download.Length - (Download.Length - lastSlash);

                return Download.Substring(0, endIndex);
            }
        }

        public string RealThumbnail
        {
            get { return $"{BaseUrl}/{Thumbnail}"; }
        }

        public bool StatusBool
        {
            get { return Status == "approved" ? true : false; }
        }

        public bool IsDownloading { get; set; }

        public bool IsDownloaded { get; set; }

        public string FolderPath { get; set; }
    }
}
