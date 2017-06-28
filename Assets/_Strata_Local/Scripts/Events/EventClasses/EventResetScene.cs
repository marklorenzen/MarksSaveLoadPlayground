//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    /// <summary>
    /// An event for reseting the scene.
    /// </summary>
    public class EventResetScene : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventResetScene";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventResetScene" /> class.
        /// </summary>    
        public EventResetScene()
        {   
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
            return null;
        }
    }

}
