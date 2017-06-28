namespace Strata
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
  
    /// <summary>
    /// A data object present in every active workspace used to persist the state of the workspace
    /// data gets refreshed before every "save" action based on the condition of actors active in the scene
    /// under Workspace Hierarchy Root The root does not need to be persisted, but (almost) all the nodes under it do
    /// </summary>
    [Serializable]
    public class ISVR_MetaData 
    {
        [SerializeField]
        string[] _header; 

        public ISVR_MetaData()
        {
            //TODO source this string from server?
            _header = new string[]
            { "Strata inStudio VR metafile.",
            "Copyright 2017, Strata Mixed Reality LTD.",
            "All rights reserved." };

            _actorRecordCollection = new List<ActorRecord>();
        }

        /// <summary>
        /// an empty or null _filename indicates a workspace that has never been saved
        /// and so must receive a user-authored name in order to save it
        /// unnamed files will never auto-save or respond to a save shortcut
        /// </summary>
        [SerializeField]
        string _fileName = string.Empty;

        public string FileName { get { return _fileName; } }
        public void SetFileName(string name)
        {
            Debug.LogWarning("Just named a workspace metafile to '" + name +"'");
            _fileName = name;
        }
       

        [SerializeField]
        public WorkspaceProperties _properties;

        [SerializeField]
        List<ActorRecord> _actorRecordCollection;

        [SerializeField]
        ISVR_DependencyManifest _dependencyManifest;

        public ISVR_DependencyManifest Dependencies
        {
            get { return _dependencyManifest; }
        }


        public List<ActorRecord> ActorRecordCollection {get {return _actorRecordCollection;}}

        //this is a recursion-based serialization scheme
        //apparently this has problems dealing with nested collections
        //so I need to instead make a flat ActorRecord collection that
        // encompasses the entire workspace actor tree.

        public bool Populate(IWorkspace workspace)
        {

            //workspace type name
            _properties = new WorkspaceProperties()
            {
                _workspaceTypeName = workspace.GetWorkspaceTypeName(),
                _authorLockName = workspace.GetAuthorLockName() 
            };

            if (string.IsNullOrEmpty(_properties._workspaceTypeName))
                return false;

            //all them actors
            var actorCollection = workspace.GetActorCollection();
            _actorRecordCollection = GetActorRecordCollectionFromActorCollection(actorCollection);
            if (_actorRecordCollection == null || _actorRecordCollection.Count == 0)
                return false;

            PopulateDependencyManifest();

            return true;
        }

        void PopulateDependencyManifest()
        {
            _dependencyManifest = new ISVR_DependencyManifest(ActorRecordCollection);
        }

        List<ActorRecord> GetActorRecordCollectionFromActorCollection(List<IActor> actors)
        {
            var records = new List<ActorRecord>();

            foreach (var actor in actors)
            {
                records.Add(new ActorRecord(actor));
            }

            return records;
        }


        /// <summary>
        /// checks to see whether the metadata has other than default values
        /// </summary>
        /// <returns>Whether the metadata is worthy to save</returns>
        public bool Validate()
        {
            //a metadata without a root actor record must not have been populated
            //from its workspace.
            if (_actorRecordCollection == null || _actorRecordCollection.Count == 0)
                return false;

            //a metadata must have a name given by the user
            if (string.IsNullOrEmpty(FileName))
                return false;

            //lets make sure all the actors under the root have an unbroken heritage to the root
            foreach (var record in _actorRecordCollection)
            {
                var trans = record.GetTransform();
                if (trans != null)
                {
                    var actor = trans.GetComponent<Actor>();

                    //note: this actor could be an ActorStage, a different class

                    if (actor != null && !actor.ValidateHeritage())
                    {
                        Debug.LogWarning(actor.name + " failed heritage validation!");
                        return false;
                    }
                }
            }
 
            return true;
        }

        #region Nested Types

        /// <summary>
        /// In theory, we should be able to save the entire working hierarchy
        /// present in the scene, simply by constructing one ActorRecord for
        /// the hierarchy root transform (guaranteed to have an IActor implementing 
        /// MonoBehaviour on it) and the recursion that happens in PopulateChildren
        /// should traverse the entire tree, assuming that there is an unbroken
        /// heritage from the root to every actor!!!!
        /// </summary>
        [Serializable]
        public class ActorRecord
        {
            [NonSerialized]
            public IActor _actor;

            public ActorRecord(IActor actor)
            {
                _actor = actor;
                _id = _actor.GetID();
                var trans = _actor.GetTransform();
                _transform = new TransformHolder(trans);
                _parent = _actor.GetActorParent();
                _components = GetActorComponents(_actor);
                _prefabKey = _actor.GetPrefabKey(); 
                _actorType = actor.GetActorType().ToString();
            }

            public ActorRecord(ActorRecord other)
            {
                _actor = other._actor;
                _id = other._id;
                _transform = other._transform;
                _parent = other._parent;
                 _components = other._components;
                _prefabKey = other._prefabKey;
                _actorType = other._actorType;
            }

            [SerializeField]
            public ActorID _id = ActorID.InvalidID;

            [SerializeField]
            public string _actorType = string.Empty;

            [SerializeField]
            public string _prefabKey = "unknown";

            [SerializeField]
            public ActorID _parent = ActorID.InvalidID;
 
            [SerializeField]
            public TransformHolder _transform = new TransformHolder();

            [SerializeField]
            public List<ActorComponentRecord> _components = new List<ActorComponentRecord>();

            public Transform GetTransform()
            {
                return _transform.GetTransform();
            }

            IActor GetActorFromTransform(Transform trans)
            {
                foreach(var actor in trans.GetComponents<MonoBehaviour>())
                {
                    if (actor is IActor)
                        return (IActor)actor;
                }

                return null;
            }

            List<ActorComponentRecord> GetActorComponents(IActor actor)
            {
                var components = actor.GetActorComponents();
                var componentRecords = new List<ActorComponentRecord>();

                foreach (var component in components)
                {
                    if (component is IActor)
                        continue;

                    var record = new ActorComponentRecord(component);
                    componentRecords.Add(record);
                }

                return componentRecords;
            }

        }


        [Serializable]
        public class ActorComponentRecord
        {
            [SerializeField]
            public string _type;
        
            [SerializeField]
            public ActorID _id;

            [SerializeField]
            public ActorComponent.Data _data;

            public ActorComponentRecord(ActorComponent component)
            {
                _type = component.GetType().Name;
               // _name = component.GetName();
                _id = component.GetID();
               // _parent = component.GetParentID();
               // _parentName = component.GetParentName();

                _data = component.GetData();

            }
        }
         
 

        [Serializable]
        public class TransformHolder
        {
            public TransformHolder() { }
            public TransformHolder(Transform trans)
                : this(trans.position, trans.rotation, trans.localScale)
            {
                _transform = trans;
            }

            private TransformHolder(Vector3 pos, Quaternion rot, Vector3 scale)
            {
                if (pos != Vector3.zero) Position = new float[] { pos.x,pos.y,pos.z};
                if (rot != Quaternion.identity) Rotation = new float[] { rot.x, rot.y, rot.z, rot.w };
                if (scale != Vector3.one) Scale = new float[] { scale.x, scale.y, scale.z };
            }

            [SerializeField]
            public float[] Position = null;

            [SerializeField]
            public float[] Rotation = null;

            [SerializeField]
            public float[] Scale = null;

            [NonSerialized]
            private Transform _transform;

            public Transform GetTransform()
            {
                return _transform;
            }
        }

        [Serializable]
        public class WorkspaceProperties
        {
            [SerializeField]
            public string _workspaceTypeName = "unknown";

            [SerializeField]
            public string _authorLockName = string.Empty;
  
            [SerializeField]
            public Color AmbientColor;
        }

        #endregion

    }
}