using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelSaber.Events
{
    public class DownloadStartedEventArgs : EventArgs
    {
        public OnlineModel Model { get; private set; }

        public DownloadStartedEventArgs(OnlineModel model)
        {
            Model = model;
        }
    }
}
