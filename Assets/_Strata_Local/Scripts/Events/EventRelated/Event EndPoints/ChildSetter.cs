//Written by Max Kofford for StrataVR
namespace Strata
{

    using UnityEngine;

    /// <summary>
    /// Used as a reciever end point foor EventRecieverObjectAwake to set up a hiearchy between objects spawned with bootstrapper.
    /// </summary>
    public class ChildSetter : MonoBehaviour
    {
        /// <summary>
        /// The target object that will become a parent (wife is pregnant).
        /// </summary>
        [SerializeField]
        private GameObject targetParent;

        /// <summary>
        /// Whether or not the new child should be activated after being set as a child.
        /// </summary>
        [SerializeField]
        private bool shouldActivateChild;


        /// <summary>
        /// Adding the input object as a child of the targetParent.
        /// </summary>
        /// <param name="input">The new child object.</param>
        public void SetAsChild(GameObject input)
        {
            input.transform.SetParent(targetParent.transform);
            if (shouldActivateChild)
            {
                input.SetActive(true);
            }
        }
    }
}