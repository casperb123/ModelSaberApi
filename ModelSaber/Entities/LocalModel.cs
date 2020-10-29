using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace ModelSaber.Entities
{
    public class LocalModel : INotifyPropertyChanged
    {
        private ModelType modelType;
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

        public ModelType ModelType
        {
            get { return modelType; }
            set
            {
                modelType = value;
                OnPropertyChanged(nameof(ModelType));
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
                ModelType = ModelType.Saber;
            else if (extension == ".avatar")
                ModelType = ModelType.Avatar;
            else if (extension == ".plat")
                ModelType = ModelType.Platform;
            else if (extension == ".bloq")
                ModelType = ModelType.Bloq;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
