//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    /// <summary>
    /// An event for when a user setting has been changed.
    /// </summary>
    public class EventPersistantSettingChange : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventSettingChange";

        /// <summary>
        /// The data to be passed along with the event.
        /// </summary>
        private readonly Data data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPersistantSettingChange" /> class.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="settingValue">The new value for the setting.</param>
        public EventPersistantSettingChange (string settingName, float settingValue)
        {
            data = new Data(settingName, settingValue);
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
        /// Represents the data for a EventSettingChange event.
        /// </summary>
        public class Data
        {


            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="name">The name of the setting.</param>
            /// <param name="value">The new value of the setting.</param>
            public Data(string name,float value)
            {
                Name = name;
                Value = value;   
            }


            /// <summary>
            /// Gets or sets the name of the setting that was changed.
            /// </summary>
            public string Name
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the new value of the setting that was changed.
            /// </summary>
            public float Value
            {
                get;
                set;
            }

           

        }

    }

}
