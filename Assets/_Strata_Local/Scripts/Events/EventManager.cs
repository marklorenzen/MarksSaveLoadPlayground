using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public static class EventManager 
{
    private static Hashtable _listenerTable = new Hashtable();
    private static Dictionary<IEventListener, GameObject> _listenerGameObjects = new Dictionary<IEventListener, GameObject>();

    public static Hashtable ListenerTable
    {
        get { return _listenerTable; }
    }
    /// <summary>
    /// Adds a listener to the EventManager that will receive any events of the supplied event name
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static bool AddListener(IEventListener listener, string eventName)
    {
        if (listener == null || eventName == null)
        {
            Debug.Log("Event Manager: AddListener failed due to no listener or event name specified.");
            return false;
        }

        if (!_listenerTable.ContainsKey(eventName))
            _listenerTable.Add(eventName, new ArrayList());

        ArrayList listenerList = _listenerTable[eventName] as ArrayList;
        if (listenerList == null)
            return false;

        if (listenerList.Contains(listener))
        {
            Debug.Log("Event Manager: Listener: " + listener.GetType().ToString() + " is already in list for event: " + eventName);
            return false; //listener already in list
        }

        listenerList.Add(listener);

        var behaviour = listener as MonoBehaviour;
        if (behaviour != null)
        {
            var gob = behaviour.gameObject;
            if (gob != null)
            {
                _listenerGameObjects[listener] = gob;
            }
        }
        return true;
    }



    /// <summary>
    /// Either adds or removes a listener from the subscribed event
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="listener"></param>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static bool ManageSubscriber(SubscribeMode mode, IEventListener listener, string eventName)
    {
        if (mode == SubscribeMode.Subscribe)
        {
            AddListener(listener, eventName);
            return true;
        }
	
        return DetachListener(listener, eventName);

    }


    /// <summary>
    /// Removes a listener from the subscribed event
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static bool DetachListener(IEventListener listener, string eventName)
    {

        if (! _listenerTable.ContainsKey(eventName))
            return false;

        ArrayList listenerList = _listenerTable[eventName] as ArrayList;
        if (listenerList == null)
            return false;

        if (!listenerList.Contains(listener))
            return false;

        listenerList.Remove(listener);

        _listenerGameObjects.Remove(listener);

        return true;
    }

    /// <summary>
    /// Triggers an event instantly. 
    /// All listeners will execute their handleEvent inside this block, even if it is a widely subscribed event
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public static bool TriggerEvent(IEvent evt)
    {
#if UNITY_EDITOR
         //var stackTrace = new StackTrace();
        //if (stackTrace.FrameCount > 1 && stackTrace.GetFrame(1).GetMethod().Name.Contains("Update"))
        //{

        //}
        //else
        //{
        //    Logging.Log("TriggerEvent: " + evt.GetName(), Logging.MessageFilter.Events, "yellow");
        //}
#endif
        string eventName = evt.GetName();
        if (!_listenerTable.ContainsKey(eventName))
        {
            Debug.LogWarning("Event Manager: Event \"" + eventName + "\" triggered has no listeners!");
            return false; //No listeners for event so ignore it
        }

        ArrayList listenerList = _listenerTable[eventName] as ArrayList;
        if (listenerList == null)
            return true;

        //  Changed this to a for loop because detaching a listener during the enumeration causes errors.
        for (var i = listenerList.Count - 1; i >= 0; --i)
        {
            i = Mathf.Clamp(i, 0, listenerList.Count - 1);  //  Ran into a case once where i became out of range.  Band-aid fix.  I tried moving anything that modifies _listenerTable to outside this loop with no good results.
            var listener = (IEventListener)listenerList[i];
            if (listener == null)
            {
                ((ArrayList)_listenerTable[eventName]).RemoveAt(i);
                continue;
            }

            if (_listenerGameObjects.ContainsKey(listener) && _listenerGameObjects[listener] == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("A listener for " + eventName + " is attached to a null game object and is being removed by the EventManger.");
#endif
                ((ArrayList)_listenerTable[eventName]).RemoveAt(i);
                _listenerGameObjects.Remove(listener);
                continue;
            }

            if (listener.HandleEvent(evt) == ListenerResult.Ignored)
                Debug.LogWarning("The listener, " + listener + " appears to be ignoring the event, " + evt.GetName() + " to which it is subscribed. This is not optimal.");

        }

        return true;
    }

 
    /// <summary>
    /// returns whether the event has a registered listener
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public static bool HasListener(IEvent evt)
    {
        return _listenerTable.ContainsKey(evt.GetName());
    }

    /// <summary>
    /// Completely restarts the manager
    /// forgets all subscribers of all event types
    /// </summary>
    public static void Reset()
    {
        _listenerTable = new Hashtable();
        _listenerGameObjects = new Dictionary<IEventListener, GameObject>();
    }
}