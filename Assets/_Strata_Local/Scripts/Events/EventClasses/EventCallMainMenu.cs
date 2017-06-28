using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCallMainMenu : IEvent {

    public const string EventName = "EventCallMainMenu";
    string IEvent.GetName() { return EventName; }
    object IEvent.GetData() { return null; }

}
