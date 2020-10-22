using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelSaber.Entities
{
    public class OnlineModelBase
    {
        public List<OnlineModel> Models { get; set; }

        public OnlineModelBase()
        {
            Models = new List<OnlineModel>();
        }

        public OnlineModelBase(OnlineModelBase onlineModelBase)
        {
            Models = onlineModelBase.Models.ToList();
        }
    }
}
