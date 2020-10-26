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
        private string type;

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("type")]
        public string Type
        {
            get { return type; }
            set
            {
                type = value;

                if (value == "saber")
                    ModelType = ModelType.Saber;
                else if (value == "avatar")
                    ModelType = ModelType.Avatar;
                else if (value == "platform")
                    ModelType = ModelType.Platform;
                else if (value == "bloq")
                    ModelType = ModelType.Bloq;
            }
        }

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

        [JsonIgnore]
        public string RealThumbnail
        {
            get { return $"https://modelsaber.com/files/saber/{Id}/{Thumbnail}"; }
        }

        [JsonIgnore]
        public bool StatusBool
        {
            get { return Status == "approved" ? true : false; }
        }

        [JsonIgnore]
        public ModelType ModelType { get; set; }

        [JsonIgnore]
        public bool IsDownloading { get; set; }

        [JsonIgnore]
        public bool IsDownloaded { get; set; }

        [JsonIgnore]
        public string ModelPath { get; set; }

        [JsonIgnore]
        public int Page { get; set; }
    }
}
