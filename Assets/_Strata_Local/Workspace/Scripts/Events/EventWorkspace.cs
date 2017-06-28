namespace Strata
{
    public class EventWorkspaceNew : IEvent
    {
        //todo add the type of workspace, perhaps
        public EventWorkspaceNew(WorkspaceManager.WorkspaceType type)
        {
            _type = type;
        }
        WorkspaceManager.WorkspaceType _type;
        public const string EventName = "EventWorkspaceNew";
        public object GetData() { return _type; }
        public string GetName() { return EventName; }
    }

    public class EventWorkspaceSave : IEvent
    {
        private string _path = "unknown";
        public EventWorkspaceSave(string path)
        {
            _path = path;
        }
        public const string EventName = "EventWorkspaceSave";
        public object GetData() { return _path; }
        public string GetName() { return EventName; }
    }

    public class EventWorkspaceClear : IEvent
    {
        public const string EventName = "EventWorkspaceClear";
        public object GetData() { return null; }
        public string GetName() { return EventName; }
    }

    public class EventWorkspaceLoad : IEvent
    {
        private string _path = "unknown";
        public EventWorkspaceLoad(string path)
        {
            _path = path;
        }
        public const string EventName = "EventWorkspaceLoad";
        public object GetData() { return _path; }
        public string GetName() { return EventName; }
    }

    public class EventWorkspaceSwitch : IEvent
    {
        public const string EventName = "EventWorkspaceSwitch";
        public IWorkspace _newWorkspace;

        public EventWorkspaceSwitch(IWorkspace newWorkspace)
        {
            _newWorkspace = newWorkspace;
        }

        public object GetData() { return _newWorkspace; }
        public string GetName() { return EventName; }
    }

    public class EventOnWorkspaceSwitched : IEvent
    {
        public const string EventName = "EventOnWorkspaceSwitched";
        public IWorkspace _newWorkspace;

        public EventOnWorkspaceSwitched(IWorkspace newWorkspace)
        {
            _newWorkspace = newWorkspace;
        }

        public object GetData() { return _newWorkspace; }
        public string GetName() { return EventName; }
    }

 
}
