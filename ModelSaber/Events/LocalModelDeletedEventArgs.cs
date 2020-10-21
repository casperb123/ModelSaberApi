using ModelSaber.Entities;
using System;

namespace ModelSaber.Events
{
    public class LocalModelDeletedEventArgs : EventArgs
    {
        public LocalModel Model { get; private set; }

        public LocalModelDeletedEventArgs(LocalModel model)
        {
            Model = model;
        }
    }
}
