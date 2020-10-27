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
        private int? prevPage;
        private int? nextPage;
        private int lastPage;

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
        public int LastPage
        {
            get { return lastPage; }
            set
            {
                lastPage = value;
                OnPropertyChanged(nameof(LastPage));
            }
        }

        [JsonIgnore]
        public int LastPageReal
        {
            get { return LastPage + 1; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public OnlineModels()
        {
            Models = new ObservableCollection<OnlineModel>();
        }

        public OnlineModels(OnlineModels onlineModels)
        {
            Models = new ObservableCollection<OnlineModel>(onlineModels.Models);
            LastPage = onlineModels.LastPage;
            PrevPage = onlineModels.PrevPage;
            NextPage = onlineModels.NextPage;
        }
    }
}
