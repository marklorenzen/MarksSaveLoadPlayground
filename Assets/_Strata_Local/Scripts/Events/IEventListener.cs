/// <summary>
/// determines whether a listener can stop other listeners from handling events they have handled
/// </summary>
public enum ListenerResult { Ignored, Handled };
public enum SubscribeMode { Subscribe, Unsubscribe };

/// <summary>
/// interface for all classes that listen to events
/// </summary>
public interface IEventListener
{
    ListenerResult HandleEvent(IEvent evt);
    void Subscribe(SubscribeMode mode);
}
