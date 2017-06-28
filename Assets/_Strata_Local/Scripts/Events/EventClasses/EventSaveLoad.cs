//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    /// <summary>
    /// An event for reseting the scene.
    /// </summary>
    public class EventSaveLoad : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventSaveLoad";

        /// <summary>
        /// The data for the event.
        /// </summary>
        private Data data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSaveLoad" /> class.
        /// </summary>
        /// <param name="saveFileName"></param>
        public EventSaveLoad(Data.SaveOrLoad sol, string saveFileName)
        {
            data = new Data(sol, saveFileName);
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
        /// Represents the data passed with the EventSaveLoad event.
        /// </summary>
        public class Data
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="sol">The type for the event.</param>
            /// <param name="inFileName">The file name to save into.</param>
            public Data(SaveOrLoad sol , string inFileName)
            {
                Type = sol;
                FileName = inFileName;
            }

            public enum SaveOrLoad
            {
                /// <summary>
                /// Represents a Save Event
                /// </summary>
                Save,

                /// <summary>
                /// Represents a Load Event
                /// </summary>
                Load
            }

            /// <summary>
            /// The file name to save into.
            /// </summary>
            public string FileName
            {
                get; set;
            }

            /// <summary>
            /// The type of event - either save or load.
            /// </summary>
            public SaveOrLoad Type
            {
                get; set;
            }
        }
    }

}
