//Written by Max Kofford for StrataVR
namespace Strata.Events
{
    using UnityEngine;

    /// <summary>
    /// An event for when a object has come awake.
    /// </summary>
    public class EventObjectAwake : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventObjectAwake";

        /// <summary>
        /// The data to be passed along with the event.
        /// </summary>
        private readonly Data data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventObjectAwake" /> class.
        /// </summary>
        /// <param name="d">The data to be passed along.</param>
        public EventObjectAwake(Data d)
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
        /// Represents the data for a EventObjectAwake event.
        /// </summary>
        public class Data
        {


            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="name">The name assosiated with the object thats coming awake.</param>
            /// <param name="inawaker">The object thats coming awake.</param>
            public Data(string name, GameObject inawaker)
            {
                awaker = inawaker;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>               
            public Data()
            {
                awaker = null;
                Name = string.Empty;
            }

            /// <summary>
            /// Gets or sets the object thats just come awake. - if its null then its a reciever thats waking up and requesting awakes
            /// </summary>
            public GameObject awaker
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the name assosiated with the waking object.
            /// </summary>
            public string Name
            {
                get;
                set;
            }
        }

    }

}