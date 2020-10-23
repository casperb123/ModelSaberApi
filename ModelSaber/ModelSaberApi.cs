using ModelSaber.Entities;
using ModelSaber.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace ModelSaber
{
    public class ModelSaberApi
    {
        private readonly string modelSaber;
        private readonly string modelSaberApi;
        private readonly string downloadPath;
        private readonly string[] excludedCharacters;

        public string SabersPath { get; set; }
        public string AvatarsPath { get; set; }
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
            SabersPath = Path.Combine(beatSaberPath, "CustomSabers");
            AvatarsPath = Path.Combine(beatSaberPath, "CustomAvatars");
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

        public async Task<OnlineModels> GetOnlineSabers(Sort sort, bool descending, List<Filter> filters, int page = 0)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string projectName = Assembly.GetEntryAssembly().GetName().Name;
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, projectName);
                    int startIndex = page * 10;
                    int endIndex = startIndex + 10;
                    string sortDirection = descending ? "desc" : "asc";
                    string filtersText = string.Join(",", filters.Select(x => $"{x.Type.ToString().ToLower()}:{x.Text}"));
                    string json = null;

                    if (filters is null || filters.Count == 0)
                        json = await webClient.DownloadStringTaskAsync($"{modelSaberApi}?type=saber&start={startIndex}&end={endIndex}&sort={sort.ToString().ToLower()}&sortDirection={sortDirection}");
                    else
                        json = await webClient.DownloadStringTaskAsync($"{modelSaberApi}?type=saber&start={startIndex}&end={endIndex}&sort={sort.ToString().ToLower()}&sortDirection={sortDirection}&filter={filtersText}");

                    Dictionary<string, JToken> jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);
                    OnlineModels onlineModels = new OnlineModels
                    {
                        StartIndex = startIndex,
                        EndIndex = endIndex
                    };
                    string[] sabersDownloaded = Directory.GetFiles(SabersPath, "*.saber");

                    foreach (var jsonToken in jsonDictionary)
                    {
                        OnlineModel onlineModel = JsonConvert.DeserializeObject<OnlineModel>(jsonToken.Value.ToString());
                        string saberPath = sabersDownloaded.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == onlineModel.Name);

                        if (string.IsNullOrEmpty(saberPath))
                        {
                            if (Downloading.Any(x => x.Id == onlineModel.Id))
                                onlineModel.IsDownloading = true;
                        }
                        else
                        {
                            onlineModel.IsDownloaded = true;
                            onlineModel.SaberPath = saberPath;
                        }

                        onlineModels.Models.Add(onlineModel);
                    }

                    return RefreshOnlinePages(onlineModels);
                }
            }
            catch (WebException e)
            {
                throw new WebException("Can't connect to ModelSaber", e.InnerException);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public OnlineModels RefreshOnlinePages(OnlineModels onlineModels)
        {
            OnlineModels newOnlineModels = new OnlineModels(onlineModels);

            if (newOnlineModels.StartIndex == 0)
                newOnlineModels.PrevPage = null;
            else
                newOnlineModels.PrevPage = (newOnlineModels.StartIndex / 10) - 1;

            if (newOnlineModels.Models.Count == 10)
                newOnlineModels.NextPage = newOnlineModels.EndIndex / 10;
            else
                newOnlineModels.NextPage = null;
            //int lastPage = 0;

            //foreach (OnlineModel onlineModel in newOnlineModels.Models)
            //{
            //    int index = newOnlineModels.Models.IndexOf(onlineModel);
            //    if (index > 0 && index % 10 == 0)
            //        lastPage++;

            //    onlineModel.Page = lastPage;
            //}

            //newOnlineModels.LastPage = lastPage;
            //if (lastPage == 0)
            //{
            //    newOnlineModels.NextPage = null;
            //    newOnlineModels.PrevPage = null;
            //}
            //else
            //{
            //    if (newOnlineModels.NextPage is null && newOnlineModels.PrevPage is null)
            //    {
            //        if (lastPage >= 1)
            //            newOnlineModels.NextPage = 1;
            //    }
            //    else
            //    {
            //        if (newOnlineModels.NextPage is null)
            //        {
            //            if (newOnlineModels.PrevPage < lastPage)
            //            {
            //                if (newOnlineModels.PrevPage + 2 <= lastPage)
            //                    newOnlineModels.NextPage = newOnlineModels.PrevPage + 2;
            //                else
            //                    newOnlineModels.PrevPage = lastPage - 1;
            //            }
            //            else
            //                newOnlineModels.PrevPage = lastPage - 1;
            //        }
            //        else
            //        {
            //            if (newOnlineModels.NextPage > lastPage)
            //            {
            //                newOnlineModels.NextPage = null;
            //                if (lastPage - 1 >= 0)
            //                    newOnlineModels.PrevPage = lastPage - 1;
            //            }
            //        }
            //    }
            //}

            return newOnlineModels;
        }

        public void ChangeOnlinePage(OnlineModels onlineModels, int page)
        {
            

            //if (page >= 0 && page <= onlineModels.LastPage)
            //{
            //    if (page == 0)
            //        onlineModels.PrevPage = null;
            //    else
            //        onlineModels.PrevPage = page - 1;

            //    if (page == onlineModels.LastPage)
            //        onlineModels.NextPage = null;
            //    else
            //        onlineModels.NextPage = page + 1;
            //}
            //else if (page <= 0)
            //{
            //    page = 0;
            //    onlineModels.PrevPage = null;
            //    if (page + 1 <= onlineModels.LastPage)
            //        onlineModels.NextPage = page + 1;
            //    else
            //        onlineModels.NextPage = null;
            //}
            //else if (page >= onlineModels.LastPage)
            //{
            //    page = onlineModels.LastPage;
            //    onlineModels.NextPage = null;
            //    if (page - 1 >= 0)
            //        onlineModels.PrevPage = page - 1;
            //    else
            //        onlineModels.PrevPage = null;
            //}
        }
    }
}
