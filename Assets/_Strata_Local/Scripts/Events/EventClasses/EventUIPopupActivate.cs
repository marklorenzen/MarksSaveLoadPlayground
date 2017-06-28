using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strata
{
    public class EventUIPopupActivate : IEvent
    {
        public const string EventName = "EventUIPopupActivate";
        private readonly Data _data;

        public EventUIPopupActivate(Data data)
        {
            _data = data;
        }

        string IEvent.GetName()
        {
            return EventName;
        }

        object IEvent.GetData()
        {
            return _data;
        }

        public class Data
        {
            public Data(bool active, string message = null, string buttonLabel = null, System.Action callback = null)
            {
                Active = active;
                Message = message;
                ButtonLabel = buttonLabel;
                Callback = callback;
            }

            public bool Active;
            public string Message;
            public string ButtonLabel;
            public System.Action Callback;
        }


    }
}