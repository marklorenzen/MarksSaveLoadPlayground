//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Converts a EventNotification event into a unity event.
    /// </summary>
    public class EventHandlerNotification : MonoBehaviour, IEventListener
    {

        /// <summary>
        /// The events to invoke when a notification is recieved.
        /// </summary>
        [SerializeField]
        private UnityEventNotification onNotifcation;

        /// <summary>
        /// Handling the EventNotification event.
        /// </summary>
        /// <param name="evt">The event that is being sent here.</param>
        /// <returns>Whether or not to cascade the event.</returns>
        public ListenerResult HandleEvent(IEvent evt)
        {

            string evtName = evt.GetName();

            switch (evtName)
            {
                case EventNotification.EventName:

                    string message = (string)evt.GetData();

                    MessageRecieved(message);

                    return ListenerResult.Handled;

            }

            return ListenerResult.Ignored;
        }

        /// <summary>
        /// Adds or removes the listener for the EventNotification event.
        /// </summary>
        /// <param name="mode">Whether its adding or removing these settings.</param>
        public void Subscribe(SubscribeMode mode)
        {
            EventManager.ManageSubscriber(mode,this,EventNotification.EventName);
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
        /// Handling a recieved notification message.
        /// </summary>
        /// <param name="message">The message for the notification.</param>
        private void MessageRecieved(string message)
        {
            onNotifcation.Invoke(message);
        }
        
        /// <summary>
        /// A unity event with a string parameter that is set up to show in inspector.
        /// </summary>
        [System.Serializable]
        public class UnityEventNotification : UnityEvent<string>
        {
        }

    }
}
