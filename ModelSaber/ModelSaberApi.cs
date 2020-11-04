using ModelSaber.Entities;
using ModelSaber.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public string PlatformsPath { get; set; }
        public string BloqsPath { get; set; }
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
            PlatformsPath = Path.Combine(beatSaberPath, "CustomPlatforms");
            BloqsPath = Path.Combine(beatSaberPath, "CustomNotes");
            Downloading = new List<OnlineModel>();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            downloadPath = Path.Combine(appData, "ModelSaberApi");

            Directory.CreateDirectory(downloadPath);
            Directory.CreateDirectory(SabersPath);
            Directory.CreateDirectory(AvatarsPath);
            Directory.CreateDirectory(PlatformsPath);
            Directory.CreateDirectory(BloqsPath);

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

        public async Task<OnlineModels> GetOnlineModels(ModelType modelType, Sort sort, bool descending, List<Filter> filters, OnlineModels cachedOnlineModels = null)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string projectName = Assembly.GetEntryAssembly().GetName().Name;
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, projectName);
                    int startIndex = cachedOnlineModels is null ? 0 : cachedOnlineModels.CurrentPage * 10;
                    int endIndex = startIndex + 10;
                    string sortDirection = descending ? "desc" : "asc";
                    string filtersText = string.Join(",", filters.Select(x => $"{x.Type.ToString().ToLower()}:{x.Text}"));
                    string json = null;
                    string apiString = $"{modelSaberApi}?type={modelType.ToString().ToLower()}&sort={sort.ToString().ToLower()}&sortDirection={sortDirection}";

                    if (cachedOnlineModels != null)
                        apiString += $"&start={startIndex}&end={endIndex}";
                    if (filters != null && filters.Count > 0)
                        apiString += $"&filter={filtersText}";

                    json = await webClient.DownloadStringTaskAsync(apiString);
                    Dictionary<string, JToken> jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);
                    OnlineModels onlineModels;
                    if (cachedOnlineModels is null || jsonDictionary.Count > cachedOnlineModels.TotalModels)
                    {
                        cachedOnlineModels = null;
                        onlineModels = new OnlineModels
                        {
                            TotalModels = jsonDictionary.Count
                        };
                    }
                    else
                        onlineModels = new OnlineModels(cachedOnlineModels, false);

                    List<JToken> jTokens = jsonDictionary.Take(10).Select(x => x.Value).ToList();
                    string extension = null;
                    string filesPath = null;
                    switch (modelType)
                    {
                        case ModelType.None:
                            break;
                        case ModelType.Saber:
                            extension = ".saber";
                            filesPath = SabersPath;
                            break;
                        case ModelType.Avatar:
                            extension = ".avatar";
                            filesPath = AvatarsPath;
                            break;
                        case ModelType.Platform:
                            extension = ".plat";
                            filesPath = PlatformsPath;
                            break;
                        case ModelType.Bloq:
                            extension = ".bloq";
                            filesPath = BloqsPath;
                            break;
                        default:
                            break;
                    }

                    string[] modelsDownloaded = Directory.GetFiles(filesPath, $"*{extension}");

                    foreach (JToken jToken in jTokens)
                    {
                        OnlineModel onlineModel = JsonConvert.DeserializeObject<OnlineModel>(jToken.ToString());
                        string filePath = modelsDownloaded.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == onlineModel.Name);

                        if (string.IsNullOrEmpty(filePath))
                        {
                            if (Downloading.Any(x => x.Id == onlineModel.Id))
                                onlineModel.IsDownloading = true;
                        }
                        else
                        {
                            onlineModel.IsDownloaded = true;
                            onlineModel.ModelPath = filePath;
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
            catch (Exception)
            {
                throw;
            }
        }

        public LocalModels GetLocalModels(ModelType modelType, LocalModels cachedLocalModels = null)
        {
            LocalModels localModels = cachedLocalModels is null ? new LocalModels() : new LocalModels(cachedLocalModels);
            string extension = null;
            string filesPath = null;
            switch (modelType)
            {
                case ModelType.None:
                    break;
                case ModelType.Saber:
                    extension = ".saber";
                    filesPath = SabersPath;
                    break;
                case ModelType.Avatar:
                    extension = ".avatar";
                    filesPath = AvatarsPath;
                    break;
                case ModelType.Platform:
                    extension = ".plat";
                    filesPath = PlatformsPath;
                    break;
                case ModelType.Bloq:
                    extension = ".bloq";
                    filesPath = BloqsPath;
                    break;
                default:
                    break;
            }
            List<string> models = Directory.GetFiles(filesPath, $"*{extension}").ToList();

            foreach (LocalModel model in localModels.Models.ToList())
            {
                string filePath = models.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == model.Name);

                if (!string.IsNullOrEmpty(filePath))
                    models.Remove(filePath);
            }

            for (int i = 0; i < models.Count; i++)
            {
                if (i > 0 && i % 10 == 0)
                    localModels.LastPage++;
            }

            foreach (string modelFile in models)
            {
                LocalModel model = new LocalModel(modelFile)
                {
                    ModelPath = modelFile
                };

                _ = Task.Run(async () =>
                {
                    try
                    {
                        model.OnlineModel = await GetModel(model.Name, model.ModelType);
                    }
                    catch (Exception e)
                    {
                        if (model.Errors is null)
                            model.Errors = new List<string>();

                        if (e.InnerException is null || string.Equals(e.Message, e.InnerException.Message))
                            model.Errors.Add(e.Message);
                        else
                            model.Errors.Add($"{e.Message} ({e.InnerException.Message})");
                    }
                });

                localModels.Models.Add(model);
            }

            return RefreshLocalPages(localModels);
        }

        public LocalModels RefreshLocalPages(LocalModels localModels)
        {
            LocalModels newLocalModels = new LocalModels(localModels);
            int lastPage = 0;

            foreach (LocalModel localModel in newLocalModels.Models)
            {
                int index = newLocalModels.Models.IndexOf(localModel);
                if (index > 0 && index % 10 == 0)
                    lastPage++;

                localModel.Page = lastPage;
            }

            newLocalModels.LastPage = lastPage;
            if (lastPage == 0)
            {
                newLocalModels.NextPage = null;
                newLocalModels.PrevPage = null;
            }
            else
            {
                if (newLocalModels.NextPage is null && newLocalModels.PrevPage is null)
                {
                    if (lastPage >= 1)
                        newLocalModels.NextPage = 1;
                }
                else
                {
                    if (newLocalModels.NextPage is null)
                    {
                        if (newLocalModels.PrevPage < lastPage)
                        {
                            if (newLocalModels.PrevPage + 2 <= lastPage)
                                newLocalModels.NextPage = newLocalModels.PrevPage + 2;
                            else
                                newLocalModels.PrevPage = lastPage - 1;
                        }
                        else
                            newLocalModels.PrevPage = lastPage - 1;
                    }
                    else
                    {
                        if (newLocalModels.NextPage > lastPage)
                        {
                            newLocalModels.NextPage = null;
                            if (lastPage - 1 >= 0)
                                newLocalModels.PrevPage = lastPage - 1;
                        }
                    }
                }
            }

            return newLocalModels;
        }

        public void ChangeLocalPage(LocalModels localModels, int page)
        {
            if (page >= 0 && page <= localModels.LastPage)
            {
                if (page == 0)
                    localModels.PrevPage = null;
                else
                    localModels.PrevPage = page - 1;

                if (page == localModels.LastPage)
                    localModels.NextPage = null;
                else
                    localModels.NextPage = page + 1;
            }
            else if (page <= 0)
            {
                page = 0;
                localModels.PrevPage = null;
                if (page + 1 <= localModels.LastPage)
                    localModels.NextPage = page + 1;
                else
                    localModels.NextPage = null;
            }
            else if (page >= localModels.LastPage)
            {
                page = localModels.LastPage;
                localModels.NextPage = null;
                if (page - 1 >= 0)
                    localModels.PrevPage = page - 1;
                else
                    localModels.PrevPage = null;
            }
        }

        public OnlineModels RefreshOnlinePages(OnlineModels onlineModels)
        {
            OnlineModels newOnlineModels = new OnlineModels(onlineModels);
            int lastPage = 0;

            for (int i = 0; i < onlineModels.TotalModels; i++)
            {
                if (i > 0 && i % 10 == 0)
                    lastPage++;
            }

            newOnlineModels.LastPage = lastPage;
            if (lastPage == 0)
            {
                newOnlineModels.NextPage = null;
                newOnlineModels.PrevPage = null;
            }
            else
            {
                if (newOnlineModels.NextPage is null && newOnlineModels.PrevPage is null)
                {
                    if (lastPage >= 1)
                        newOnlineModels.NextPage = 1;
                }
                else
                {
                    if (newOnlineModels.NextPage is null)
                    {
                        if (newOnlineModels.PrevPage < lastPage)
                        {
                            if (newOnlineModels.PrevPage + 2 <= lastPage)
                                newOnlineModels.NextPage = newOnlineModels.PrevPage + 2;
                            else
                                newOnlineModels.PrevPage = lastPage - 1;
                        }
                        else
                            newOnlineModels.PrevPage = lastPage - 1;
                    }
                    else
                    {
                        if (newOnlineModels.NextPage > lastPage)
                        {
                            newOnlineModels.NextPage = null;
                            if (lastPage - 1 >= 0)
                                newOnlineModels.PrevPage = lastPage - 1;
                        }
                    }
                }
            }

            return newOnlineModels;
        }

        public void ChangeOnlinePage(OnlineModels onlineModels, int page)
        {
            if (page >= 0 && page <= onlineModels.LastPage)
            {
                if (page == 0)
                    onlineModels.PrevPage = null;
                else
                    onlineModels.PrevPage = page - 1;

                if (page == onlineModels.LastPage)
                    onlineModels.NextPage = null;
                else
                    onlineModels.NextPage = page + 1;
            }
            else if (page <= 0)
            {
                page = 0;
                onlineModels.PrevPage = null;
                if (page + 1 <= onlineModels.LastPage)
                    onlineModels.NextPage = page + 1;
                else
                    onlineModels.NextPage = null;
            }
            else if (page >= onlineModels.LastPage)
            {
                page = onlineModels.LastPage;
                onlineModels.NextPage = null;
                if (page - 1 >= 0)
                    onlineModels.PrevPage = page - 1;
                else
                    onlineModels.PrevPage = null;
            }
        }

        private void ProgressChangedFire(OnlineModel model, DateTime startTime, DownloadProgressChangedEventArgs e)
        {
            string received = string.Format(CultureInfo.InvariantCulture, "{0:n0} kB", e.BytesReceived / 1000);
            string toReceive = string.Format(CultureInfo.InvariantCulture, "{0:n0} kB", e.TotalBytesToReceive / 1000);

            if (e.BytesReceived / 1000000 >= 1)
                received = string.Format("{0:.#0} MB", Math.Round((decimal)e.BytesReceived / 1000000, 2));
            if (e.TotalBytesToReceive / 1000000 >= 1)
                toReceive = string.Format("{0:.#0} MB", Math.Round((decimal)e.TotalBytesToReceive / 1000000, 2));

            TimeSpan timeSpent = DateTime.Now - startTime;
            int secondsRemaining = (int)(timeSpent.TotalSeconds / e.ProgressPercentage * (100 - e.ProgressPercentage));
            TimeSpan timeLeft = new TimeSpan(0, 0, secondsRemaining);
            string timeLeftString = string.Empty;
            string timeSpentString = string.Empty;

            if (timeLeft.Hours > 0)
                timeLeftString += string.Format("{0} hours", timeLeft.Hours);
            if (timeLeft.Minutes > 0)
                timeLeftString += string.IsNullOrWhiteSpace(timeLeftString) ? string.Format("{0} min", timeLeft.Minutes) : string.Format(" {0} min", timeLeft.Minutes);
            if (timeLeft.Seconds >= 0)
                timeLeftString += string.IsNullOrWhiteSpace(timeLeftString) ? string.Format("{0} sec", timeLeft.Seconds) : string.Format(" {0} sec", timeLeft.Seconds);

            if (timeSpent.Hours > 0)
                timeSpentString = string.Format("{0} hours", timeSpent.Hours);
            if (timeSpent.Minutes > 0)
                timeSpentString += string.IsNullOrWhiteSpace(timeSpentString) ? string.Format("{0} min", timeSpent.Minutes) : string.Format(" {0} min", timeSpent.Minutes);
            if (timeSpent.Seconds >= 0)
                timeSpentString += string.IsNullOrWhiteSpace(timeSpentString) ? string.Format("{0} sec", timeSpent.Seconds) : string.Format(" {0} sec", timeSpent.Seconds);

            DownloadProgressed?.Invoke(this, new DownloadProgressedEventArgs(model, e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage, timeLeftString, timeSpentString, received, toReceive));
        }

        public async Task<bool> DownloadModel(OnlineModel model)
        {
            string modelName = model.Name;

            foreach (string character in excludedCharacters)
                modelName = modelName.Replace(character, "");

            string extension = null;
            string filePath = null;
            switch (model.ModelType)
            {
                case ModelType.Saber:
                    extension = ".saber";
                    filePath = SabersPath;
                    break;
                case ModelType.Avatar:
                    extension = ".avatar";
                    filePath = AvatarsPath;
                    break;
                case ModelType.Platform:
                    extension = ".plat";
                    break;
                case ModelType.Bloq:
                    extension = ".bloq";
                    break;
                default:
                    break;
            }

            string downloadFilePath = Path.Combine(downloadPath, $"{model.Id}{extension}");
            string downloadString = model.Download;
            string modelPath = Path.Combine(filePath, $"{model.Name}{extension}");

            if (File.Exists(modelPath))
            {
                DownloadFailed?.Invoke(this, new DownloadFailedEventArgs(model, new InvalidOperationException($"The {model.ModelType.ToString().ToLower()} is already downloaded")));
                return false;
            }

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string projectName = Assembly.GetEntryAssembly().GetName().Name;

                    DateTime startTime = DateTime.Now;
                    webClient.DownloadProgressChanged += (s, e) => ProgressChangedFire(model, startTime, e);
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, projectName);
                    model.IsDownloading = true;
                    Downloading.Add(model);

                    DownloadStarted?.Invoke(this, new DownloadStartedEventArgs(model));
                    await webClient.DownloadFileTaskAsync(new Uri(downloadString), downloadFilePath);
                    await Task.Run(() =>
                    {
                        try
                        {
                            File.Move(downloadFilePath, modelPath);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    });

                    model.ModelPath = modelPath;
                    model.IsDownloading = false;
                    model.IsDownloaded = true;

                    OnlineModel downloadingModel = Downloading.FirstOrDefault(x => x.Id == model.Id);
                    if (downloadingModel != null)
                        Downloading.Remove(downloadingModel);

                    DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(model));
                }

                return true;
            }
            catch (WebException e)
            {
                if (File.Exists(downloadFilePath))
                    File.Delete(downloadFilePath);

                model.IsDownloading = false;
                DownloadFailed?.Invoke(this, new DownloadFailedEventArgs(model, new WebException("Can't connect to ModelSaber", e.InnerException)));
                return false;
            }
            catch (Exception e)
            {
                if (File.Exists(downloadFilePath))
                    File.Delete(downloadFilePath);

                model.IsDownloading = false;
                DownloadFailed?.Invoke(this, new DownloadFailedEventArgs(model, e));
                return false;
            }
        }

        public void DeleteModel(OnlineModel model)
        {
            if (model.IsDownloaded)
            {
                if (File.Exists(model.ModelPath))
                    File.Delete(model.ModelPath);

                model.IsDownloaded = false;
                OnlineModelDeleted?.Invoke(this, new OnlineModelDeletedEventArgs(model));
            }
        }

        public void DeleteModel(LocalModel model)
        {
            if (File.Exists(model.ModelPath))
                File.Delete(model.ModelPath);

            LocalModelDeleted?.Invoke(this, new LocalModelDeletedEventArgs(model));
        }

        public void DeleteModels(List<OnlineModel> models)
        {
            foreach (OnlineModel model in models)
                DeleteModel(model);
        }

        public async Task<OnlineModel> GetModel(string name, ModelType modelType)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string projectName = Assembly.GetEntryAssembly().GetName().Name;
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, projectName);
                    string api = $"{modelSaberApi}?type={modelType.ToString().ToLower()}&filter=name:{name}";
                    string json = await webClient.DownloadStringTaskAsync(api);
                    Dictionary<string, JToken> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);

                    foreach (var jsonModel in jsonDict)
                    {
                        OnlineModel model = JsonConvert.DeserializeObject<OnlineModel>(jsonModel.Value.ToString());
                        if (model.Name == name)
                            return model;
                    }

                    return null;
                }
            }
            catch (WebException e)
            {
                throw new WebException("The model couldn't be found on ModelSaber", e.InnerException);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
