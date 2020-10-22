using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ModelSaber.Entities
{
    public class OnlineModels : INotifyPropertyChanged
    {
        private int? prevPage;
        private int? nextPage;

        public List<OnlineModel> Models { get; set; }

        [JsonIgnore]
        public int LastPage { get; set; }

        [JsonIgnore]
        public int? PrevPage
        {
            get { return prevPage; }
            set
            {
                prevPage = value;
                OnPropertyChanged(nameof(PrevPage));
            }
        }

        [JsonIgnore]
        public int? NextPage
        {
            get { return nextPage; }
            set
            {
                nextPage = value;
                OnPropertyChanged(nameof(CurrentPageReal));
            }
        }

        [JsonIgnore]
        public int CurrentPage
        {
            get
            {
                if (!PrevPage.HasValue && !NextPage.HasValue)
                    return 0;

                return NextPage.HasValue ? (NextPage.Value - 1) : (PrevPage.Value + 1);
            }
        }

        [JsonIgnore]
        public int CurrentPageReal
        {
            get { return CurrentPage + 1; }
        }

        [JsonIgnore]
        public int LastPageReal
        {
            get { return LastPage + 1; }
        }

        public OnlineModels()
        {
            Models = new List<OnlineModel>();
        }

        public OnlineModels(OnlineModels onlineModels)
        {
            Models = new List<OnlineModel>(onlineModels.Models);
            LastPage = onlineModels.LastPage;
            PrevPage = onlineModels.PrevPage;
            NextPage = onlineModels.NextPage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
