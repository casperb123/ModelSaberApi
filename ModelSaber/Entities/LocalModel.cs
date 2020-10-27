using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace ModelSaber.Entities
{
    public class LocalModel : INotifyPropertyChanged
    {
        private ModelType type;
        private string name;
        private OnlineModel onlineModel;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ModelType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public string ModelPath { get; set; }

        public int Page { get; set; }

        public List<string> Errors { get; set; }

        public OnlineModel OnlineModel
        {
            get { return onlineModel; }
            set
            {
                onlineModel = value;
                OnPropertyChanged(nameof(OnlineModel));
            }
        }

        public LocalModel(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            Name = Path.GetFileNameWithoutExtension(fileName);
            if (extension == ".saber")
                Type = ModelType.Saber;
            else if (extension == ".avatar")
                Type = ModelType.Avatar;
            else if (extension == ".plat")
                Type = ModelType.Platform;
            else if (extension == ".bloq")
                Type = ModelType.Bloq;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
