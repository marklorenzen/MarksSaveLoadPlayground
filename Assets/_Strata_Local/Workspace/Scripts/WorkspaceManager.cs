namespace Strata
{
    using UnityEngine;
    using System.Collections.Generic;

    public class WorkspaceManager : MonoSingleton<WorkspaceManager>, IEventListener
    {
        static IWorkspace _currentWorkspace; 
        public static IWorkspace CurrentWorkspace
        {
            get { return _currentWorkspace; }
            set { _currentWorkspace = value; }
        }
 
        public ListenerResult HandleEvent(IEvent evt)
        {
            var evtName = evt.GetName();

            switch(evtName)
            {
                case EventWorkspaceSwitch.EventName:
                    //Debug.Log("WorkspaceManager.HandleEvent " + evtName);
                    var newWorkspace = (IWorkspace)evt.GetData();
                    SwitchWorkspace(newWorkspace);
                    return ListenerResult.Handled;

                case EventWorkspaceLoad.EventName:
                    //Debug.Log("WorkspaceManager.HandleEvent " + evtName);
                    var loadFilePath = (string)evt.GetData();
                    _currentWorkspace.LoadWorkspace(loadFilePath);//TODO validate
                    return ListenerResult.Handled;

                case EventWorkspaceSave.EventName:
                    //Debug.Log("WorkspaceManager.HandleEvent " + evtName);
                    var saveFilepath = (string)evt.GetData();
                    _currentWorkspace.SaveWorkspace(saveFilepath);//TODO validate
                    return ListenerResult.Handled;

                case EventWorkspaceClear.EventName:
                    //Debug.Log("WorkspaceManager.HandleEvent " + evtName);
                    _currentWorkspace.ClearWorkspace();//TODO validate
                    return ListenerResult.Handled;

                case EventWorkspaceNew.EventName:
                    //Debug.Log("WorkspaceManager.HandleEvent " + evtName);
                    var type = (WorkspaceType)evt.GetData();
                    NewWorkspace(type); 
                    return ListenerResult.Handled;
            }

            return ListenerResult.Ignored;
        }

        public void Subscribe(SubscribeMode mode)
        {
            EventManager.ManageSubscriber(mode, this, EventWorkspaceSwitch.EventName);
            EventManager.ManageSubscriber(mode, this, EventWorkspaceLoad.EventName);
            EventManager.ManageSubscriber(mode, this, EventWorkspaceSave.EventName);
            EventManager.ManageSubscriber(mode, this, EventWorkspaceNew.EventName);
            EventManager.ManageSubscriber(mode, this, EventWorkspaceClear.EventName);
            
        }

        public enum WorkspaceType
        {
            PresentationEdit,
            StageEdit
                //and many more to come...
        }

        public virtual bool NewWorkspace(WorkspaceType type)
        {
            Debug.Log("making a new, untitled workspace..." + name);

            WorkspaceBase newWorkspace = null;

            switch (type)
            {
                case WorkspaceType.PresentationEdit:
                    newWorkspace = Instantiate(WorkspacePresentationPrefab, transform) as WorkspaceBase;
                    break;
                case WorkspaceType.StageEdit:
                    newWorkspace = Instantiate(WorkspaceStagePrefab, transform) as WorkspaceBase;
                    break;
            }

            if (newWorkspace != null)
            {
                TrackNewWorkspace(newWorkspace);
                SwitchWorkspace(newWorkspace);
                return true;
            }

            return false;
        }

         
        public WorkspaceStageEdit WorkspaceStagePrefab;
        public WorkspacePresentationEdit WorkspacePresentationPrefab;

        HashSet<IWorkspace> workspaces = new HashSet<IWorkspace>();

#if UNITY_EDITOR
        /// <summary>
        /// Used only by the unity editor, WorkspaceManagerInspector
        /// </summary>
        public HashSet<IWorkspace> FriendWorkspaces 
        {
            get { return workspaces; }
        }
#endif
        private void Awake()
        {
#if UNITY_EDITOR
            var wses = GetComponentsInChildren<WorkspaceBase>();
            foreach (var ws in wses)
            {
                //ActorStage is a wierd case, not really a workspace per se
                //so not to be tracked by the Workspace Manager, just a fancy
                //free little workspace living inside an actor, self governing
                if (ws is ActorStage)
                    continue;


                ws.Activate(false);
                TrackNewWorkspace(ws);
                
            }

            SwitchWorkspace(wses[0]);
#endif

            Subscribe(SubscribeMode.Subscribe);
        }

        void TrackNewWorkspace(IWorkspace workspace)
        {
            if (workspaces.Contains(workspace))
            {
                Debug.LogWarning("WorkspaceManager is already tracking " + workspace );
            }
            else
            {
                workspaces.Add(workspace);
            }
        }


        private void OnDestroy()
        {
            Subscribe(SubscribeMode.Unsubscribe);
        }

        void SwitchWorkspace(IWorkspace newWorkspace)
        {
            if (CurrentWorkspace != null)
                CurrentWorkspace.Activate(false);

            CurrentWorkspace = newWorkspace;
            CurrentWorkspace.Activate(true);

            EventManager.TriggerEvent(new EventOnWorkspaceSwitched(CurrentWorkspace));
        }
 
    }
    
}
