//Written by Max Kofford for StrataVR
namespace Strata
{
    using Events;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Recieves EventObjectAwake events and converts them to UnityEvents to be processed on this game object.
    /// </summary>
    public class EventRecieverObjectAwake : MonoBehaviour, IEventListener
    {
        /// <summary>
        /// The name for the target object to listen for.
        /// </summary>
        [SerializeField]
        private string associatedName;
    
        /// <summary>
        /// Events to call when our target has woken up.
        /// </summary>
        [SerializeField]
        private UnityEventAwakeObject eventsToCall;

        /// <summary>
        /// Handling either the EventObjectAwake events.
        /// </summary>
        /// <param name="evt">The event that is being sent here.</param>
        /// <returns>Whether or not to cascade the event.</returns>
        public ListenerResult HandleEvent(IEvent evt)
        {

            string evtName = evt.GetName();

            switch (evtName)
            {
                case EventObjectAwake.EventName:

                    //invoking the unity events if its the correct target - null awaker means its the reciever waking up.
                    EventObjectAwake.Data data = (EventObjectAwake.Data)evt.GetData();
                    if (data.awaker != null && data.Name.Equals(associatedName))
                    {
                        eventsToCall.Invoke(data.awaker);
                    }

                    return ListenerResult.Handled;
            }

            return ListenerResult.Ignored;
        }

        /// <summary>
        /// Adds or removes the listener for the EventObjectAwake event.
        /// </summary>
        /// <param name="mode">Whether its adding or removing the events.</param>
        public void Subscribe(SubscribeMode mode)
        {
            EventManager.ManageSubscriber(mode,this,EventObjectAwake.EventName);

        }

        /// <summary>
        /// Adding the listener for the event(s).
        /// </summary>
        private void Awake()
        {
            Subscribe(SubscribeMode.Subscribe);
        }


        /// <summary>
        /// Sending the awake notification. - null awaker means its a reciever waking up.
        /// </summary>
        private void Start()
        {
            EventObjectAwake.Data data = new EventObjectAwake.Data();
            data.Name = associatedName;
            data.awaker = null;
            EventManager.TriggerEvent(new EventObjectAwake(data));
        }


        /// <summary>
        /// A unity event with a GameObject parameter setup to show in inspector.
        /// </summary>
        [System.Serializable]
        public class UnityEventAwakeObject : UnityEvent<GameObject>
        {
        }

    }
}