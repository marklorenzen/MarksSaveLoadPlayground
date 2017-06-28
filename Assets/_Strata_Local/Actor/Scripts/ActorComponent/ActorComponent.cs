namespace Strata
{
    using System;
    using UnityEngine;

    //[DisallowMultipleComponent()]
    public abstract class ActorComponent : MonoBehaviour, IActorComponent
    {
        private Transform _transform;

        private ActorID ID = ActorID.InvalidID; 
        public ActorID GetID() { return ID; }
        public void SetID(ActorID id) { ID = id; } 
        public string GetName() { return name; }
        public virtual bool IsActor() { return false; }
        public abstract ActorID GetActorAncestor();
        public virtual Data GetData() { return null; }

        [Serializable]
        public class Data
        {
            [SerializeField]
            public ActorComponentBeACapsule.Data[] BeACapsuleData;

            [SerializeField]
            public ActorComponentBuildable.Data[] BuildableObjectData;

            [SerializeField]
            public ActorComponentBuildablePart.Data[] BuildablePartData;

            [SerializeField]
            public ActorStage.Data[] StageData;

            public Data()
            {
                BeACapsuleData = null;
                BuildableObjectData = null;
                BuildablePartData = null;
                StageData = null;
            }
        }

        public Transform GetTransform()
        {
            if (_transform == null)
                _transform = transform;

            return _transform;
        }

        public ActorID GetParentID()
        {
            return ActorID.InvalidID;//todo use a real ID
        }

        public string GetParentName()
        {
            var xform = GetTransform();

            if (GetTransform().parent != null)
            {
                return GetTransform().parent.name;
            }

            return "none";
        }

        protected virtual void Awake() 
        {
            var idData = new EventActorComponentComputeID.Data(this);
            EventManager.TriggerEvent(new EventActorComponentComputeID(idData));
            ID = idData.ID;
 
 
#if UNITY_EDITOR
            if (!ID.IsValid)
            {
                Debug.Log("Apparently we could not get an actorID for a component, was there no factory?");

                ActorFactory af = FindObjectOfType<ActorFactory>();
                if (af != null)
                    ID = af.GetIDHackHackHack(this);
                else
                    Debug.LogWarning("Oooh no! there is no ActorFactory in the scene!");
            }
#endif




        }

        public virtual void PreSave() { }
        public virtual void OnSave() { }
        public virtual void OnLoad(ISVR_MetaData.ActorRecord record) { }
        public virtual void PostLoad() { }



    }


}
