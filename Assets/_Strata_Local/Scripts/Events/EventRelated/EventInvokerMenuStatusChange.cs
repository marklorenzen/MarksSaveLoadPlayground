//Written by Max Kofford for StrataVR
namespace Strata
{
    using Strata.Events;
    using UnityEngine;

    /// <summary>
    /// A invoker class for sending menu status change events.
    /// </summary>
    public class EventInvokerMenuStatusChange : MonoBehaviour
    {

        /// <summary>
        /// What menu state to look for in default send events.
        /// </summary>
        [SerializeField]
        private EventMenuStatusChange.Data.StatusChange changeToLookFor;

        /// <summary>
        /// What menu to look for in default send events.
        /// </summary>
        [SerializeField]
        private EventMenuStatusChange.Data.MenuType targetMenu;


        /// <summary>
        /// Sending the default message (whatever is set up in the inspector).
        /// </summary>
        public void SendDefaultMessage()
        {
            EventManager.TriggerEvent(new EventMenuStatusChange(new EventMenuStatusChange.Data(changeToLookFor,targetMenu)));

        }

        /// <summary>
        /// Sending a specific message change.
        /// </summary>
        /// <param name="target">The menu type to invoke with.</param>
        /// <param name="status">The new status of the menu to invoke with.</param>
        public void SendMessage(EventMenuStatusChange.Data.MenuType target,EventMenuStatusChange.Data.StatusChange status)
        {
            EventManager.TriggerEvent(new EventMenuStatusChange(new EventMenuStatusChange.Data(status,target)));

        }
    }
}