using ModelSaber.Entities;
using System;

namespace ModelSaber.Events
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public OnlineModel Model { get; private set; }

        public DownloadCompletedEventArgs(OnlineModel model)
        {
            Model = model;
        }
    }
}
