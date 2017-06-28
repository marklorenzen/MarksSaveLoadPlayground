namespace Strata
{
    /// <summary>
    /// one of the rare events that use the event data as a delivery container back 
    /// to the triggering object
    /// </summary>
    public class EventActorComponentComputeID : IEvent
    {
        public const string EventName = "EventActorComponentComputeID";

        private Data _data;

        public EventActorComponentComputeID(Data data)
        {
            _data = data;
        }

        public class Data
        {
            public Data(IActorComponent component)
            {
                _component = component;
            }

            private IActorComponent _component;
            private ActorID _id = ActorID.InvalidID;

            public ActorID ID
            {
                get { return _id; }
                set { _id = value; }
            }

            public IActorComponent ActorComponent { get { return _component; } }
        }

        public object GetData() { return _data; }
        public string GetName() { return EventName; }
    }

    /// <summary>
    /// This is one of those rare events which also serve as a delivery container
    /// it the subscriber sends a new IActor object back to the triggering object
    /// which needs to retain its reference to the event in order to extract it.
    /// This decouples the ActorFactory, while leaving IActor as a coupled concept
    /// EventActorCreate does not need a constructor because only the subscriber
    /// assigns the value
    /// </summary>
    public class EventActorCreate : IEvent
    {
        public const string EventName = "EventActorCreate";
        public class Data
        {
            public Data(Actor prefab)
            {
                _prefab = prefab;
            }
            public Actor _prefab;
            public IActor _newActor;
        }
        private Data _data;

        public EventActorCreate(Data data)
        {
            _data = data;
        }
        public object GetData() { return _data; }
        public string GetName() { return EventName; }
    }

    public class EventActorDestroy : IEvent
    {
        public const string EventName = "EventActorDestroy";
        
        private IActor _victim;

        public EventActorDestroy(IActor victim)
        {
            _victim = victim;
        }
        public object GetData() { return _victim; }
        public string GetName() { return EventName; }
    }

    /// <summary>
    /// This is one of the unusual events which serve as a delivery container
    /// the subscriber assigns the ID value to the event, so the object that
    /// constructs/triggers the event can examine the result.
    /// </summary>
    public class EventActorComponentFindFromID : IEvent
    {
        public const string EventName = "EventActorGetFromID";

        public EventActorComponentFindFromID(Data data)
        {
            SetData(data);
        }

        public void SetData(Data data)
        {
            _data = data;
        }

        public class Data
        {
            public Data (ActorID key)
            {
                idKey = key;
            }
            public ActorID idKey;
            public IActorComponent _foundActorComponent;
            public void SetActor(IActorComponent component)
            {
                _foundActorComponent = component;
            }
        }

        private Data _data;
        public object GetData() { return _data; }
        public string GetName() { return EventName; }
    }
}

