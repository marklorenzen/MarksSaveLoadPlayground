//Written by Max Kofford for StrataVR
namespace Strata
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;

    /// <summary>
    /// Inspector helper for turning a UnityEvent<GameObject> into a reference to the object in a different script. 
    /// Mainly used for setting up references across different bootstrappable prefabs at runtime.
    /// </summary>
    public class TransferObjectReference : MonoBehaviour
    {

        /// <summary>
        /// The target script to pass references onto.
        /// </summary>
        [SerializeField]
        private MonoBehaviour targetScript;

        /// <summary>
        /// The name of the field or property that is the target to pass the reference to.
        /// </summary>
        [SerializeField]
        private string targetName;

        /// <summary>
        /// Whether or not to ignore the case for the targetName
        /// </summary>
        [SerializeField]
        private bool shouldIgnoreCase;

        /// <summary>
        /// Whether or not to ignore spaces in the targetName
        /// </summary>
        [SerializeField]
        private bool shouldIgnoreSpaces;

        /// <summary>
        /// Attempts to pass a reference to the input object to a field or property on the targetScript
        /// </summary>
        /// <param name="obj"></param>
        public void passReference(GameObject obj)
        {
            Type t = targetScript.GetType();

            string tmpTargetName = targetName;

            if (shouldIgnoreCase)
            {
                tmpTargetName = tmpTargetName.ToLower();
            }

           
            if (shouldIgnoreSpaces)
            {
                tmpTargetName = Regex.Replace(tmpTargetName,@"\s+","");
            }

            //Trying to set it if its a property
            foreach (var propInfo in t.GetProperties())
            {
                string propName = propInfo.Name;

                if (shouldIgnoreCase)
                {
                    propName = propName.ToLower();
                }

                if (propName.Equals(tmpTargetName))
                {               
                  
                    propInfo.SetValue(targetScript,obj,null);
                }
            }

            
            //Trying to set it if its a field
            foreach (var propInfo in t.GetFields())
            {
                string propName = propInfo.Name;

                if (shouldIgnoreCase)
                {
                    propName = propName.ToLower();
                }

                if (propName.Equals(tmpTargetName))
                {
                   
                    if (propInfo.FieldType.Equals(typeof(GameObject)))
                    {
                        propInfo.SetValue(targetScript,obj);
                    }
                    else
                    {
                       
                        //Tried to allow setting component references but something isnt quite working.
                        //Component component = obj.GetComponent(propInfo.FieldType);

                        //propInfo.SetValue(targetScript,component);
                    }
                }
            }
        }

    }
}