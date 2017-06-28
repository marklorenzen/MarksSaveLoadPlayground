//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Converts a EventSettingChange event into a unity event.
    /// </summary>
    public class SettingsEventReciever : MonoBehaviour, IEventListener
    {


        /// <summary>
        /// The target setting to invoke the unity events when changed.
        /// </summary>
        [SerializeField]
        private string targetSetting;


        /// <summary>
        /// The events to invoke if the saveload driver has a setting already set up.
        /// </summary>
        [SerializeField]
        private UnityEventSettingChange onSettingChange;



        /// <summary>
        /// Handling either the EventSettingChange events.
        /// </summary>
        /// <param name="evt">The event that is being sent here.</param>
        /// <returns>Whether or not to cascade the event.</returns>
        public ListenerResult HandleEvent(IEvent evt)
        {

            string evtName = evt.GetName();

            switch (evtName)
            {
                case EventPersistantSettingChange.EventName:

                    EventPersistantSettingChange.Data data = (EventPersistantSettingChange.Data)evt.GetData();

                    SettingChange(data.Name,data.Value);

                    return ListenerResult.Handled;

            }

            return ListenerResult.Ignored;
        }

        /// <summary>
        /// Adds or removes the listener for the EventSettingChange event.
        /// </summary>
        /// <param name="mode">Whether its adding or removing these settings.</param>
        public void Subscribe(SubscribeMode mode)
        {
            EventManager.ManageSubscriber(mode,this,EventPersistantSettingChange.EventName);
        }

        /// <summary>
        /// Adding the listener for the EventSettingChange event.
        /// </summary>
        private void Awake()
        {
            Subscribe(SubscribeMode.Subscribe);
        }

        /// <summary>
        /// The stuff to do when we recieve a event.
        /// </summary>
        /// <param name="name">The name of the setting that was changed.</param>
        /// <param name="value">The name of the value for the modified setting.</param>
        private void SettingChange(string name,float value)
        {
            if (name.Equals(targetSetting))
            {
                onSettingChange.Invoke(value);
            }
        }

        /// <summary>
        /// A unity event with a float parameter that is set up to show in inspector.
        /// </summary>
        [System.Serializable]
        public class UnityEventSettingChange : UnityEvent<float>
        {
        }

    }
}