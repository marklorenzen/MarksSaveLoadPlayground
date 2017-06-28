//Written by Max Kofford for StrataVR
namespace Strata
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

     
    /// <summary>
    /// Helper class that will only pass on a call once the first time it is called.  All other calls are ignored.
    /// </summary>
    public class OnlyPassOnOnce : MonoBehaviour
    {
        /// <summary>
        /// The things to call when a event is being passed on.
        /// </summary>
        [SerializeField]
        private UnityEvent thingsToCall;

        /// <summary>
        /// Whether or not we have passed on a call yet.
        /// </summary>
        private bool hasNotPassedYet = true;

        /// <summary>
        /// Attempting to pass on a call.  Will only pass on calls once.
        /// </summary>
        public void CallThings()
        {
            if (hasNotPassedYet == true)
            {
                thingsToCall.Invoke();
                hasNotPassedYet = false;
            }
        }

    }
}
