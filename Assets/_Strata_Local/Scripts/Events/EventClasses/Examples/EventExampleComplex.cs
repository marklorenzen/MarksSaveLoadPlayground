using System.Collections.Generic;

/// <summary>
/// Event to make the DesignVR app switch into or out of SE mode
/// </summary>
public class EventExampleComplex : IEvent
{
    public const string EventName = "EventExampleComplex";

    private readonly Data _data;

    public EventExampleComplex(Data data)
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
        public string _str;
        public int _val;
        public Dictionary<int, string> _dict = new Dictionary<int, string>();

        public Data(string str, int val, Dictionary<int,string> dict)
        {
            _str = str;
            _val = val;
            _dict = dict;
        }
    }

}
