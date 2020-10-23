using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ModelSaber.Entities
{
    public class OnlineModels : INotifyPropertyChanged
    {
        private ObservableCollection<OnlineModel> models;
        private int startIndex;
        private int endIndex;
        private int? prevPage;
        private int? nextPage;

        public ObservableCollection<OnlineModel> Models
        {
            get { return models; }
            set
            {
                models = value;
                OnPropertyChanged(nameof(Models));
            }
        }

        [JsonIgnore]
        public int? PrevPage
        {
            get { return prevPage; }
            set
            {
                prevPage = value;
                OnPropertyChanged(nameof(PrevPage));
                OnPropertyChanged(nameof(CurrentPageReal));
            }
        }

        [JsonIgnore]
        public int? NextPage
        {
            get { return nextPage; }
            set
            {
                nextPage = value;
                OnPropertyChanged(nameof(NextPage));
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
        public int StartIndex
        {
            get { return startIndex; }
            set
            {
                startIndex = value;
                OnPropertyChanged(nameof(StartIndex));
            }
        }

        [JsonIgnore]
        public int EndIndex
        {
            get { return endIndex; }
            set
            {
                endIndex = value;
                OnPropertyChanged(nameof(EndIndex));
            }
        }

        public OnlineModels()
        {
            Models = new ObservableCollection<OnlineModel>();
        }

        public OnlineModels(OnlineModels onlineModels)
        {
            Models = new ObservableCollection<OnlineModel>(onlineModels.Models);
            PrevPage = onlineModels.PrevPage;
            NextPage = onlineModels.NextPage;
            StartIndex = onlineModels.StartIndex;
            EndIndex = onlineModels.EndIndex;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
