//Written by Max Kofford for StrataVR
namespace Strata.Events
{

    /// <summary>
    /// Event to notify if a menu has opened or closed.
    /// </summary>
    public class EventMenuStatusChange : IEvent
    {
        /// <summary>
        /// The name for the event.
        /// </summary>
        public const string EventName = "EventMenuStatusChange";

       
       

        /// <summary>
        /// The backend data for one trigger for this event.
        /// </summary>
        private readonly Data data;

        /// <summary>
        ///  Initializes a new instance of the <see cref="EventMenuStatusChange" /> class.	
        /// </summary>
        /// <param name="indata">The data passed along with the event trigger.</param>
        public EventMenuStatusChange(Data indata)
        {
            this.data = indata;
        }

        /// <summary>
        /// Returns the name for this event type.
        /// </summary>
        /// <returns>The string value of this event name.</returns>
        string IEvent.GetName()
        {
            return EventName;
        }


        /// <summary>
        /// Returns the data for a event trigger of this event.
        /// </summary>
        /// <returns>The data for this event trigger.</returns>
        object IEvent.GetData()
        {
            return data;
        }


        /// <summary>
        /// Represents the data passed along by the EventMenuStatusChange event.
        /// </summary>
        public class Data
        {
             
            

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.	
            /// </summary>
            /// <param name="s">The new status change for the trigging menu.</param>
            /// <param name="menuName">The triggering menu type.</param>
            public Data(StatusChange s ,MenuType menuName)
            {
                Status = s;
                Menu = menuName;
            }

           

            /// <summary>
            /// Represents whether its a open or a close menu event.
            /// </summary>
            public enum StatusChange
            {
                /// <summary>
                /// Represents a menu being opened.
                /// </summary>
                Opening,

                /// <summary>
                /// Represents a menu being closed.
                /// </summary>
                Closing
            }

            /// <summary>
            /// Which type of menu is being opened or close.
            /// </summary>
            public enum MenuType
            {
                /// <summary>
                /// Represents the main menu.
                /// </summary>
                main,

                /// <summary>
                /// Represents the object options menu.
                /// </summary>
                options,

                /// <summary>
                /// Represents the HDRI menu.
                /// </summary>
                hdri
            }



            /// <summary>
            /// Gets or sets the status or opening/closing event type.
            /// </summary>
            public StatusChange Status
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets which menu is being opened or closed.
            /// </summary>
            public MenuType Menu
            {
                get;
                set;
            }

        }

    }
}
