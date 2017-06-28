/// <summary>
/// 
/// </summary>
public class EventSEModeSwitch : IEvent
{
    public const string EventName = "EventSEModeSwitch";

    private readonly bool _data;

    public EventSEModeSwitch(bool se)
    {
        _data = se;
    }

    string IEvent.GetName()
    {
        return EventName;
    }

    object IEvent.GetData()
    {
        return _data;
    }

}

