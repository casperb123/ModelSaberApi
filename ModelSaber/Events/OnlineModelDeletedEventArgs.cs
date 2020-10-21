using ModelSaber.Entities;
using System;

namespace ModelSaber.Events
{
    public class OnlineModelDeletedEventArgs : EventArgs
    {
        public OnlineModel Model { get; private set; }

        public OnlineModelDeletedEventArgs(OnlineModel model)
        {
            Model = model;
        }
    }
}
