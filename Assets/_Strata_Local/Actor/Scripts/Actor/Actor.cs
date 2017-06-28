namespace Strata
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Actor : ActorComponent, IActor
    {
        private Texture2D _preview;
        public Texture2D GetPreview() { return _preview; }
        public void SetPreview(ref Texture2D preview) { _preview = preview; }

        public override bool IsActor() { return true; }
        public ActorType _actorType;
        public ActorType GetActorType() { return _actorType; }
        public void SetActorType(ActorType type) { _actorType = type; }
        public override ActorID GetActorAncestor() { return GetID(); } //actors are always their own parent actors!

        [HideInInspector]
        public GameObject Prefab;
        ISVR_MetaData.ActorRecord _record;

        [SerializeField]
        [HideInInspector]
        string _prefabRootString;

        public string GetPrefabRootString() { return _prefabRootString; }
        public void SetPrefabRootString(string root) { _prefabRootString = root; }
        public string GetDependencyURL() { return "FakeTestDependencies/SophiaVergara.png"; }//todo, return an authentic dependency
         
        [SerializeField]
        [HideInInspector]
        string _prefabKey = string.Empty;
        public string GetPrefabKey() { return _prefabKey; }
        public void SetPrefabKey(string key) { _prefabKey = key; }

        public ActorID GetActorParent()
        {
            var xform = GetTransform();

            if (xform = null)
                return ActorID.InvalidID;

            if (xform.parent == null)
                return ActorID.InvalidID;

            var actor = xform.parent.GetComponent<Actor>();
            if (actor != null)
                return actor.GetID();

            return ActorID.InvalidID;
         }

        /// <summary>
        /// returns true if this ActorBase's transform can trace upwards to the WorkspaceRoot
        /// via an unbroken heritage of gameobjects which bear an Actor component
        /// </summary>
        /// <returns></returns>
        public bool ValidateHeritage()
        {

            //I have no parent (sitting in the Scene hiererchy root) (illegal)
            if (transform.parent == null)
                return false;

            Actor parentAsActor = transform.parent.GetComponent<Actor>();

            WorkspaceBase parentAsWorkspace = transform.parent.GetComponent<WorkspaceBase>();
            if (parentAsWorkspace != null)
                return true;

            //my parent is not an Actor (Illegal)  
            if (parentAsActor == null)
                return false;

            //ask your father
            return parentAsActor.ValidateHeritage();
        }


        public override void OnLoad(ISVR_MetaData.ActorRecord record)
        {
            //Debug.Log("OnLoad " + record._id._name );

            _actorType = ActorType.Unknown;
            if (Enum.IsDefined(typeof(ActorType), record._actorType))
                _actorType = (ActorType)Enum.Parse(typeof(ActorType), record._actorType);

            _record = new ISVR_MetaData.ActorRecord(record);

            SetID(_record._id);

            EventManager.TriggerEvent(new EventActorLoaded(this));

            //my unity GameObject name
            name = _record._id._name;
            // name += "__Loaded";
 
            //let any actorcomponents on this gameobject also do a load
            var actorComponents = GetComponents<ActorComponent>();
            foreach (var actorComponent in actorComponents)
                if (actorComponent != this)
                    actorComponent.OnLoad(record);
        }

        public override void PostLoad()
        {
            //Debug.Log("PostLoad " + name);

            if (_record == null)
            {
                Debug.LogWarning("PostLoad of Actor expected a non-null _record");
                return;

            }

            //my parent in the unity scene hierarchy
            ActorID parentID = _record._parent;
            var data = new EventActorComponentFindFromID.Data(parentID);
            EventManager.TriggerEvent(new EventActorComponentFindFromID(data));
            var parentComponent = data._foundActorComponent;

            if (parentComponent != null)
            {
                try
                {
                    //Debug.Log("parenting " + name + " to " + parentComponent.GetName());
                    transform.SetParent(parentComponent.GetTransform());
                }
                catch( Exception e)
                {
                    Debug.LogException(e);
                }
            }

            //my world position
            var trans = _record._transform;
            if (trans.Position != null && trans.Position.Length == 3)
                transform.position = new Vector3(trans.Position[0], trans.Position[1], trans.Position[2]);
            else
                transform.position = Vector3.zero;

            if (trans.Rotation != null && trans.Rotation.Length == 4)
                transform.rotation = new Quaternion(trans.Rotation[0], trans.Rotation[1], trans.Rotation[2], trans.Rotation[3]);
            else
                transform.rotation = Quaternion.identity;

            //todo local scale should be assigned after transform is parented
            if (trans.Scale != null && trans.Scale.Length == 3)
                transform.localScale = new Vector3(trans.Scale[0], trans.Scale[1], trans.Scale[2]);
            else
                transform.localScale = Vector3.one;

            //let any actorcomponents on this gameobject also do a postLoad
            var actorComponents = GetComponents<ActorComponent>();
            foreach (var actorComponent in actorComponents)
                if (actorComponent != this)
                    actorComponent.PostLoad();

        }

        /// <summary>
        /// Gets all the ActorComponents within the actor's hierarchy
        /// todo, We can make a flat-ordered array of all the component
        /// children throughout the hierarchy, all the buildableaxes, 
        /// buildable parts, etc, such that the Actor.OnLoad can rebuild
        /// the full complexity of the actor, as if the actor had made it.
        /// </summary>
        /// <returns></returns>
        public List<ActorComponent> GetActorComponents()
        {
            var componentsArr = GetComponents<ActorComponent>();
            var componentsList = new List<ActorComponent>();

            foreach (var component in componentsArr)
            {
                //actors are special
                if (component.IsActor())
                    continue;

                //right about here would be a good place to use a
                //componentRecord class, that stores the ActorID of the
                //component along with something that locates it in the
                //actor's potentially complex footprint
                componentsList.Add(component);
            }

            return componentsList;
        }

    }

    public class EventActorLoaded : IEvent
    {
        public const string EventName = "EventActorLoaded";
        private IActorComponent _actor;

        public EventActorLoaded(IActorComponent actor)
        { _actor = actor; }

        public object GetData() { return _actor; }
        public string GetName() { return EventName; }
    }

}
