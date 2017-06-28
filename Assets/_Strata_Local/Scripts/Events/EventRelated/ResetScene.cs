//Written by Max Kofford for StrataVR
namespace Strata
{
    using Strata.Events;
    using UnityEngine;

    /// <summary>
    /// Just invokes the reset scene event.
    /// </summary>
    public class ResetScene : MonoBehaviour
    {
        /// <summary>
        /// Inspector level method for calling the reset scene event.
        /// </summary>
        public void ResetTheScene()
        {
            EventManager.TriggerEvent(new EventResetScene());
        }
    }
}