//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    /// <summary>
    /// An event for adding a notification to the notification menu.
    /// </summary>
    public class EventNotification : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventNotification";

        /// <summary>
        /// The data to be passed along with the event.
        /// </summary>
        private readonly string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventNotification" /> class.
        /// </summary>
        /// <param name="notificationMessage">The message for this notification.</param>
        public EventNotification(string notificationMessage)
        {
           this.message = notificationMessage;
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
            return message;
        }
    }
}
