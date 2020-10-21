using ModelSaber.Entities;
using System;

namespace ModelSaber.Events
{
    public class DownloadFailedEventArgs : EventArgs
    {
        public OnlineModel Model { get; private set; }
        public Exception Exception { get; private set; }

        public DownloadFailedEventArgs(OnlineModel model, Exception exception)
        {
            Model = model;
            Exception = exception;
        }
    }
}
