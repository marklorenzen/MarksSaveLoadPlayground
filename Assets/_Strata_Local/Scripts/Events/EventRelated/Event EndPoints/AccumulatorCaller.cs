//Written by Max Kofford for StrataVR
namespace Strata
{

    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// A inspector helper script that calls events when it has accumulated enough to reach a target value.
    /// </summary>
    public class AccumulatorCaller : MonoBehaviour
    {
        /// <summary>
        /// The accumulator that increments.
        /// </summary>
        [SerializeField]
        private int accumulator = 0;

        /// <summary>
        /// The target value to change the object state upon reaching.
        /// </summary>
        [SerializeField]
        private int targetValue = 0;

        /// <summary>
        /// The things to call when the incrementor reaches the targetValue.
        /// </summary>
        [SerializeField]
        private UnityEvent thingsToCall;

        /// <summary>
        /// Increments the incrementor by 1 and calls the thingsToCall if it reaches targetValue
        /// </summary>
        public void IncrementAccumulator()
        {
            accumulator++;
            if (accumulator == targetValue)
            {
                thingsToCall.Invoke();
            }
        }

        /// <summary>
        /// Decrements the incrementor by 1 and calls the thingsToCall if it reaches targetValue
        /// </summary>
        public void DecrementAccumulator()
        {
            accumulator--;
            if (accumulator == targetValue)
            {
                thingsToCall.Invoke();
            }
        }
    }
}