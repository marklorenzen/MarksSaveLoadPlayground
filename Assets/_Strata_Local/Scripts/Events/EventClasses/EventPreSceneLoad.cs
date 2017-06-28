//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    /// <summary>
    /// An event for stuff to do right before a scene is being loaded.
    /// </summary>
    public class EventPreSceneLoad : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventPreSceneLoad";

        /// <summary>
        /// The data to be passed along with the event (The name of the scene that is about to be loaded).
        /// </summary>
        private readonly string data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSceneLoad" /> class.
        /// </summary>
        /// <param name="d">The name of the scene that is about to be loaded.</param>
        public EventPreSceneLoad(string d)
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
    }

}