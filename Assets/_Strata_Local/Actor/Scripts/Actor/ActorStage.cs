namespace Strata
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    //ActorStage,   inherits WorkspaceBase and implements
    //IActor, so it can pseudo-load a stage's stuff as if it were
    //a workspace, but it can be placed as an actor into a
    //presentation workspace... and code can be added (or omitted)
    //to enforce that the composition of the stage actor is
    //immutable (like to actors can be added or removed, except
    //by the pseudo-load operation, described above)

    public class ActorStage : WorkspaceBase, IActor, IActorComponent
    {
        [SerializeField]
        private ActorID ID = ActorID.InvalidID;
        public ActorComponent.Data _data;

        private Texture2D _preview;
        public Texture2D GetPreview() { return _preview; }
        public void SetPreview(ref Texture2D preview) { _preview = preview; }

        public override string GetSpecificSuffix() { return WorkspaceStageEdit.STAGE_EDIT_SUFFIX; } //yah, the suffix for the stage edit workspace, so we can load that 'type' of file
        public override bool IsRead_OnlyWorkspace() { return true; }
        public bool IsActor() { return true; }
        public string GetDependencyURL() { return ""; }// no url for a stage actor, it loads as a workspace not a prefab
        public string GetName() { return name; }
        public string GetParentName() { return ""; }
        public ActorType GetActorType() { return ActorType.Stage; }
        public void SetActorType(ActorType type) { } //nothing
        public ActorID GetActorParent() { return GetID(); }
        public Transform GetTransform() { return transform; }//TODO cache reference to workspace transform?
        public ActorID GetID() { return ID; }
        public ActorID GetActorAncestor() { return GetID(); }
        public override Color GetColor() { return Color.red; }
        public void OnSave() { }
        public void PostLoad() { }
        public void PreSave() { }

        [Serializable]
        public class Data
        {
            public string _url;
        }


        public ActorStage Prefab;

        [SerializeField]
        string _prefabRootString;
        public string GetPrefabRootString()
        {
            return _prefabRootString;
        }
        public void SetPrefabRootString(string root)
        {
            _prefabRootString = root;
        }

        [SerializeField]
        string _prefabKey = string.Empty;
        public string GetPrefabKey()
        {
            if (Prefab != null && Prefab != this)
                return Prefab.GetPrefabKey();

            return _prefabKey;
        }
        public void SetPrefabKey(string key) { _prefabKey = key; }


        protected new void Awake()
        {
            if (_data == null)
            {
                _data = new ActorComponent.Data();
                _data.StageData = new Data[] { new Data() };
            }

            //TODO, stash the data required for this 'actor' to load its 'workspace'
            _data.StageData[0]._url = "caca";
        }

        private void Start()
        {
            var idData = new EventActorComponentComputeID.Data(this);
            EventManager.TriggerEvent(new EventActorComponentComputeID(idData));
            ID = idData.ID;
        }

        public void OnLoad(ISVR_MetaData.ActorRecord record)
        {
            //this is super special
            //ActorStage saves and loads as a nested, readonly workspace

            if (record._components.Count > 0)
                _data = record._components[0]._data;//TODO account for multiple components, for goodness sake

            if (_data != null)
            {
                var stageData = _data.StageData;
                if (stageData != null && stageData.Length > 0)
                {
                    if (stageData[0] != null)
                    {
                        //should involve very little overhead, as this actor is already a "workspace"
                        //perhaps as simple as just calling my, LoadWorkspace right here
                        var isvrPath = stageData[0]._url;
                        LoadWorkspace(isvrPath);

                        //put myself in the 'middle' of the presentation workspace 
                        transform.position = Vector3.zero;
                        transform.rotation = Quaternion.identity;
                        transform.localScale = Vector3.one;

                        //TODO: maybe need to switch-on some read-only properties
                        //to ensure that neither this actor (nor its parts) can be selected, mutated, deleted
                        //other than by replacing it with another ActorStage.
                    }
                    else
                        Debug.LogWarning("null color at stageData[0] in ActorStage.OnLoad " + name);

                }
                else
                    Debug.LogWarning("null or empty stageData in ActorStage.OnLoad " + name);

            }
            else
                Debug.LogWarning("null data in ActorStage.OnLoad " + name);

        }

        public List<ActorComponent> GetActorComponents()
        {
            var componentsArr = GetComponents<ActorComponent>();
            var componentsList = new List<ActorComponent>();

            foreach (var component in componentsArr)
            {
                if (component.IsActor() || component == this)
                    continue;

                componentsList.Add(component);
            }

            return componentsList;
        }

    }
}