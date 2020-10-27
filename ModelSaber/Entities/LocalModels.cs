using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ModelSaber.Entities
{
    public class LocalModels : INotifyPropertyChanged
    {
        private ObservableCollection<LocalModel> models;
        private int? prevPage;
        private int? nextPage;
        private int lastPage;

        public ObservableCollection<LocalModel> Models
        {
            get { return models; }
            set
            {
                models = value;
                OnPropertyChanged(nameof(Models));
            }
        }

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

        public int CurrentPage
        {
            get
            {
                if (!PrevPage.HasValue && !NextPage.HasValue)
                    return 0;

                return NextPage.HasValue ? (NextPage.Value - 1) : (PrevPage.Value + 1);
            }
        }

        public int CurrentPageReal
        {
            get { return CurrentPage + 1; }
        }

        public int LastPage
        {
            get { return lastPage; }
            set
            {
                lastPage = value;
                OnPropertyChanged(nameof(LastPage));
            }
        }

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

        public LocalModels()
        {
            Models = new ObservableCollection<LocalModel>();
        }

        public LocalModels(LocalModels localModels)
        {
            Models = new ObservableCollection<LocalModel>(localModels.Models);
        }
    }
}
