using ModelSaber.Entities;
using ModelSaber.Events;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModelSaber
{
    public class ModelSaberApi
    {
        private readonly string modelSaber;
        private readonly string modelSaberApi;
        private readonly string downloadPath;
        private readonly string[] excludedCharacters;

        public string CustomSabersPath { get; set; }
        public string CustomAvatarsPath { get; set; }
        public List<OnlineModel> Downloading { get; set; }

        public event EventHandler<DownloadStartedEventArgs> DownloadStarted;
        public event EventHandler<DownloadProgressedEventArgs> DownloadProgressed;
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;
        public event EventHandler<DownloadFailedEventArgs> DownloadFailed;
        public event EventHandler<OnlineModelDeletedEventArgs> OnlineModelDeleted;
        public event EventHandler<LocalModelDeletedEventArgs> LocalModelDeleted;

        public ModelSaberApi(string beatSaberPath)
        {
            modelSaber = "https://modelsaber.com";
            modelSaberApi = $"{modelSaber}/api/v2/get.php";
            CustomSabersPath = Path.Combine(beatSaberPath, "CustomSabers");
            CustomAvatarsPath = Path.Combine(beatSaberPath, "CustomAvatars");
            Downloading = new List<OnlineModel>();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            downloadPath = Path.Combine(appData, "ModelSaberApi");

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            excludedCharacters = new string[]
            {
                "<",
                ">",
                ":",
                "/",
                @"\",
                "|",
                "?",
                "*"
            };
        }


    }
}
