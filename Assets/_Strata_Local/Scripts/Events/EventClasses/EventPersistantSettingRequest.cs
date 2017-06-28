//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    /// <summary>
    /// An event for when a user setting has woken up and needs to be initialized.
    /// </summary>
    public class EventPersistantSettingRequest : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventSettingAwake";

        /// <summary>
        /// The data to be passed along with the event (and set by the event handler to be passed back as the awake setting for the event).
        /// </summary>
        private readonly Data data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPersistantSettingRequest" /> class.
        /// </summary>
        /// <param name="d">The data to be passed along and then set and then passed back for initializing the setting.</param>
        public EventPersistantSettingRequest(Data d)
        {
            data = d;
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        /// <returns>The name of the event.</returns>
        string IEvent.GetName()
        {
            return EventName;
        }

        /// <summary>
        /// Gets the data for this event call.
        /// </summary>
        /// <returns>The data for the event.</returns>
        object IEvent.GetData()
        {
            return data;
        }

        /// <summary>
        /// Represents the data for a EventSettingAwake event.
        /// </summary>
        public class Data
        {


            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>    
            /// <param name="value">The new value of the setting to be set by a event handler.</param>
            public Data(float? value)
            {   
                Value = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>               
            public Data()
            {
                
                Value = 0;
                Name = string.Empty;
            }

            /// <summary>
            /// Gets or sets the value of the setting or null if the setting doesnt have user set value yet.
            /// </summary>
            public float? Value
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the name of the setting.
            /// </summary>
            public string Name
            {
                get;
                set;
            }
        }

    }

}