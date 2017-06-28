
/// <summary>
/// No data, just a notice that something happened
/// </summary>
public class EventVerySimple : IEvent
{
    public const string EventName = "EventVerySimple";
    string IEvent.GetName() { return EventName; }
    object IEvent.GetData() { return null; }
}

