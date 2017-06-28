//Written by Max Kofford for StrataVR
namespace Strata.Events
{
    using UnityEngine;

    /// <summary>
    /// An event for when the load percentage has changed for a import.
    /// </summary>
    public class EventImportLoadingStatusChange : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventImportLoadingStatusChange";

        /// <summary>
        /// The data to be passed along with the event.
        /// </summary>
        private readonly Data data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventImportLoadingStatusChange" /> class.
        /// </summary>
        /// <param name="d">The data to be passed along.</param>
        public EventImportLoadingStatusChange(Data d)
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
        /// Represents the data for a EventImportLoadingStatusChange event.
        /// </summary>
        public class Data
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="path">The path assosiated with the import to update.</param>
            /// <param name="newPercentage">The new percentage of completion for a import.</param>
            public Data(string path,float newPercentage)
            {
                Path = path;
                NewPercentage = newPercentage;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>               
            public Data()
            {
                Path = null;
                NewPercentage = 0;
            }

            /// <summary>
            /// Gets or sets percentage of completion for a import.
            /// </summary>
            public float NewPercentage
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the path for the import for this status change event.
            /// </summary>
            public string Path
            {
                get;
                set;
            }
        }

    }

}