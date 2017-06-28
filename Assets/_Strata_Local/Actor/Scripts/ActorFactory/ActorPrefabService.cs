namespace Strata
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// a service to retrieve the prefab to instance when creating an actor
    /// actor:prefab are related 1:1, and no prefab is instanced lest unto an actor
    /// prefabs are never constructed for actorComponents, except for Actors
    /// keeps a dictionary of prefab references, keyed by a unique identifier
    /// imports are keyed based on their URL
    /// prefabs (as found in bundles) are keyed based on a unique name
    /// instrumentation added to ensure uniqueness
    /// complex objects have an assortment of names, as parts of hierarchy may be instanced unto actors
    /// </summary>
    public class ActorPrefabService : MonoBehaviour, IEventListener
    {
        public ActorPrefabCollection AnchorActorPrefabs;
        public ActorPrefabCollection ShapeActorPrefabs;
        public ActorPrefabCollection BuildableActorPrefabs;
        public ActorPrefabCollection StageActorPrefabs;
        public Actor DefaultActorPrefab;
        public Actor RootActorPrefab;

        [Serializable]
        public class Record
        {
            public Record(MonoBehaviour prefab)
            {
                _prefab = prefab as IActor;
            }
            public IActor _prefab;
            //todo, maybe there are also other ways to retrieve the game object, such as URL or license key
        }

        Dictionary<string, Record> _collection = new Dictionary<string, Record>();
        public Dictionary<string, Record> Collection
        {
            get { return _collection; }
        }


        private void AddPrefabRecord(MonoBehaviour prefab)
        {
            var iactor = prefab as IActor;
            if (iactor == null)
                return;

            if (_collection.ContainsKey(iactor.GetPrefabRootString()))
                return;

            _collection.Add(iactor.GetPrefabRootString(), new Record(prefab as MonoBehaviour));
        }

        public void Refresh()
        {
            AddPrefabRecord(DefaultActorPrefab);
            AddPrefabRecord(RootActorPrefab);
            StageActorPrefabs.Synchronize(AddPrefabRecord);
            ShapeActorPrefabs.Synchronize(AddPrefabRecord);
            BuildableActorPrefabs.Synchronize(AddPrefabRecord);
            AnchorActorPrefabs.Synchronize(AddPrefabRecord);
        }

        private void Awake()
        {
            Refresh();
            Subscribe(SubscribeMode.Subscribe);
        }
        private void OnDestroy()
        {
            Subscribe(SubscribeMode.Unsubscribe);
        }

        public ListenerResult HandleEvent(IEvent evt)
        {
            switch (evt.GetName())
            {
                case EventActorPrefabFromID.EventName:
                    var data = (EventActorPrefabFromID.Data)evt.GetData();
                    Record foundRecord = null;
                    //this handler assigns the prefab value to the event's data
                    //so the code that triggered the event can examine the data
                    //after the event is triggered
                    if (_collection.TryGetValue(data.GetID(), out foundRecord))
                        data.Prefab = (Actor)foundRecord._prefab;
                    return ListenerResult.Handled;
            }

            return ListenerResult.Ignored;
        }

        public void Subscribe(SubscribeMode mode)
        {
            EventManager.ManageSubscriber(mode, this, EventActorPrefabFromID.EventName);
        }
    }

    public class EventActorPrefabFromID : IEvent
    {
        public const string EventName = "EventActorPrefabFromID";

        public class Data
        {
            public Data(string id)
            {
                _id = id;
            }

            string _id = string.Empty;
            Actor _prefab = null;

            public Actor Prefab 
            {
                get { return _prefab; }
                set { _prefab = value; }
            }

            public string GetID()
            {
                return _id;
            }
        }

        private Data _data;

        public EventActorPrefabFromID(Data data)
        { _data = data; }


        public object GetData() { return _data; }
        public string GetName() { return EventName; }
    }
}