namespace Strata
{
    using System;
    using UnityEngine;

    public class ISVR_FileRequestorClient : MonoBehaviour, IEventListener
    {
        public string _path = string.Empty;

        public Color _color;

        public string _qualifyingSuffix = "";

        private void Awake()
        {
            _path = Application.dataPath;
            Subscribe(SubscribeMode.Subscribe);
        }

        private void OnDestroy()
        {
            Subscribe(SubscribeMode.Unsubscribe);
        }

        public string currentFilePath = string.Empty;

        public void Save()
        {
            if (!string.IsNullOrEmpty(currentFilePath))
                EventManager.TriggerEvent(new EventWorkspaceSave(currentFilePath)); 
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(currentFilePath))
                EventManager.TriggerEvent(new EventWorkspaceLoad(currentFilePath));
        }

        public void New()
        {
            EventManager.TriggerEvent(new EventWorkspaceNew(WorkspaceManager.WorkspaceType.PresentationEdit));
        }

        public ListenerResult HandleEvent(IEvent evt)
        {
            switch (evt.GetName())
            {
                case EventOnWorkspaceSwitched.EventName:
                    //this code is iffy... it presumes the file requestor client component
                    //is on the same gameobject as the workspace component being activated
                    //which is true the way I set up the prefab, but not enforceable
                    //IOW, there is no strict binding between the file requestor and its
                    //respective workspace instance...maybe need to tightly couple these?
                    var workspace = (IWorkspace)evt.GetData();
                    if (workspace != null && workspace.GetWorkspaceTransform().gameObject == gameObject)
                        _qualifyingSuffix = workspace.GetSpecificSuffix();

                    return ListenerResult.Handled;
            }

            return ListenerResult.Ignored;
        }

        public void Subscribe(SubscribeMode mode)
        {
            EventManager.ManageSubscriber(mode, this, EventOnWorkspaceSwitched.EventName);
        }
    }
}