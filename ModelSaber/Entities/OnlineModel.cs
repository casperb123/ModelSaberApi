using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ModelSaber.Entities
{
    public class OnlineModel : INotifyPropertyChanged
    {
        private string bsaber;
        private string type;
        private bool isDownloaded;
        private bool isDownloading;

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
            set
            {
                bsaber = value;
                OnPropertyChanged(nameof(Bsaber));
            }
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
            get
            {
                return Thumbnail.Contains("https://modelsaber.com/files/") ? Thumbnail : $"https://modelsaber.com/files/{Type}/{Id}/{Thumbnail}";
            }
        }

        [JsonIgnore]
        public bool StatusBool
        {
            get { return Status == "approved" ? true : false; }
        }

        [JsonIgnore]
        public ModelType ModelType { get; set; }

        [JsonIgnore]
        public bool IsDownloading
        {
            get { return isDownloading; }
            set
            {
                isDownloading = value;
                OnPropertyChanged(nameof(IsDownloading));
            }
        }

        [JsonIgnore]
        public bool IsDownloaded
        {
            get { return isDownloaded; }
            set
            {
                isDownloaded = value;
                OnPropertyChanged(nameof(IsDownloaded));
            }
        }

        [JsonIgnore]
        public string ModelPath { get; set; }

        [JsonIgnore]
        public int Page { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
