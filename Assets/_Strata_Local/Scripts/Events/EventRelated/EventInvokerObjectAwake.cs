//Written by Max Kofford for StrataVR
namespace Strata
{
    using Events;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Invokes the EventObjectAwake event when this object wakes up.
    /// Also Invokes it when a handler wakes up and requests wake up events.
    /// </summary>
    public class EventInvokerObjectAwake : MonoBehaviour, IEventListener
    {
        /// <summary>
        /// The name for this object.
        /// </summary>
        [SerializeField]
        private string associatedName;

        /// <summary>
        /// The target object that is waking up.
        /// </summary>
        [SerializeField]
        private GameObject targetAwaker;


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

                    //null awaker means its a reciever requesting events.
                    //if a reciever of this event wakes up and we are ready to recieve the event (we woke up first) then we should tell the reciever that we are already awake.
                    EventObjectAwake.Data data = (EventObjectAwake.Data)evt.GetData();
                    if (data.awaker == null && data.Name.Equals(associatedName))
                    {
                        Start();
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
        /// Removing the listener for the event(s).
        /// </summary>
        private void OnDestroy()
        {
            Subscribe(SubscribeMode.Unsubscribe);
        }

        /// <summary>
        /// Sending the awake notification.
        /// </summary>
        private void Start()
        {
            EventObjectAwake.Data data = new EventObjectAwake.Data();
            data.Name = associatedName;
            data.awaker = targetAwaker;
            EventManager.TriggerEvent(new EventObjectAwake(data));

        }   

    }
}