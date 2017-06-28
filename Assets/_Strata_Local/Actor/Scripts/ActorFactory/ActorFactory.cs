namespace Strata
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;

    public class ActorFactory : MonoSingleton<ActorFactory>, IEventListener
    {
        //public Actor ActorPrefab;

        public ActorID GetIDHackHackHack(IActorComponent actor)
        {
            return GetFactoryID(actor);
        }

        ActorID GetFactoryID(IActorComponent actor)
        {
            //first check to see if the usedIDs dictionary contains this actor already
            //and if so, just return the id already assigned
            if (_usedActorIDs.ContainsValue(actor))
            {
                return new ActorID(_usedActorIDs.First(x => x.Value == actor).Key, actor.GetName());
            }

            //spin-up a unique ulong based on the specifics of this gameobject
            return HashIDForActor(actor);
        }


        ActorID HashIDForActor(IActorComponent component)
        {
            //we assume that only MonoBehaviours implement IActor
            return HashIDForMonoBehaviour(component as MonoBehaviour);
        }

        //ActorID HashIDForActor(IActorComponent actor)
        //{
        //    //we assume that only MonoBehaviours implement IActorComponent
        //    return HashIDForMonoBehaviour(actor as MonoBehaviour);
        //}



        ActorID HashIDForMonoBehaviour(MonoBehaviour mono)
        {  
            if (mono != null)
            {
                ulong value = 0UL;

                //we seed the hash algorithm with the name and the instanceID and the unique mac address
                string seed = mono.name + mono.GetInstanceID().ToString() + GetMacAddressString();

                //spin the name
                int order = 0;
                foreach (var c in seed)
                {
                    order += (c - 31);//ascii kinda starts at 32
                    order %= 64;//precision of ulong
                    value ^= 1UL << order;
                }

                //ensure that it is unique
                while (_usedActorIDs.ContainsKey(value))
                {
                    Debug.LogWarning("Gadzooks! We have a hash-crash for: " + mono.name + "   which collides with: " + _usedActorIDs[value].GetName());
                    ++value;
                }


                //add it to the usedIDs dictionary
                _usedActorIDs.Add(value, mono as IActorComponent);

                return new ActorID(value, mono.name);

            }

            return ActorID.InvalidID;
        }


        Dictionary<ulong, IActorComponent> _usedActorIDs = new Dictionary<ulong, IActorComponent>();


        void ForceActorFromLoad(IActorComponent actor)
        {
            if (_usedActorIDs.ContainsKey(actor.GetID().Value))
            {
                 //this is perfectly valid, we may have just reloaded an actor we already knew the ID for, now we know this instance of same actor
                _usedActorIDs[actor.GetID().Value] = actor;
            }
            else
                _usedActorIDs.Add(actor.GetID().Value, actor);
        }

        IActorComponent FindActorComponent(ActorID id)
        {
            IActorComponent actor = null;
            _usedActorIDs.TryGetValue(id.Value, out actor);
            return actor;
        }

        public ListenerResult HandleEvent(IEvent evt)
        {
            var eventName = evt.GetName();

            switch(eventName)
            {
                case EventActorCreate.EventName:
                    var eacData = (EventActorCreate.Data)evt.GetData();
                    eacData._newActor = CreateActor(eacData._prefab);
                    return ListenerResult.Handled;

                case EventActorDestroy.EventName:
                    var victim = (IActor)evt.GetData();
                    DestroyActor(victim);
                    return ListenerResult.Handled;

                case EventActorComponentFindFromID.EventName:
                    var data = (EventActorComponentFindFromID.Data)evt.GetData();
                    data.SetActor(FindActorComponent(data.idKey));
                    ((EventActorComponentFindFromID)evt).SetData(data);
                    return ListenerResult.Handled;

                case EventActorComponentComputeID.EventName:
                    var eaccidData = (EventActorComponentComputeID.Data)evt.GetData();
                    eaccidData.ID = HashIDForActor(eaccidData.ActorComponent);
                    return ListenerResult.Handled;

                case EventActorLoaded.EventName:
                    var newlyLoadedActor = (IActorComponent)evt.GetData();
                    ForceActorFromLoad(newlyLoadedActor);
                    return ListenerResult.Handled;

            }
            return ListenerResult.Ignored;
        }

        /// <summary>
        /// todo this should take some parameters to fix-up the new actor before assigning reference in event
        /// todo this should place the new Actor's transform into the appropriate spot under the workspace root
        /// </summary>
        /// <returns></returns>
        IActor CreateActor(Actor prefab)
        {
            Transform actorParent = GetParentForNewActor();

            var newActor = Instantiate(prefab, actorParent) as Actor;

            FixUpNewActor(ref newActor);

            return newActor;
        }

        void DestroyActor(IActor victim)
        {
            if (victim != null)
            {
                //todo, should this remove the ID from the usedIDs dictionary?
                //or should the ActorComponent class do that from within its OnDestroy?
                //should the ID be added from Actor Awake()?
                Destroy(victim.GetTransform().gameObject);
            }
            else
                Debug.LogWarning("don't trigger to destroy a null IActor, please.");
        }

        void FixUpNewActor( ref Actor newActor)
        {
            //todo this should be full of fix-up code to make the new actor all  
            //nice and pretty and a suitable member of polite actor society
        }

        /// <summary>
        /// WorkspaceManager keeps a static reference to the currentWorkspace
        /// whose transform is always a suitable (fallback) parent for any new actor
        /// </summary>
        /// <returns></returns>
        Transform GetParentForNewActor()
        {
            //todo this sux, should not couple workspace manager to actor factory like this
            return WorkspaceManager.CurrentWorkspace.GetWorkspaceTransform(); 
        }

        public void Subscribe(SubscribeMode mode)
        {
            EventManager.ManageSubscriber(mode, this, EventActorCreate.EventName);
            EventManager.ManageSubscriber(mode, this, EventActorComponentFindFromID.EventName);
            EventManager.ManageSubscriber(mode, this, EventActorComponentComputeID.EventName);
            EventManager.ManageSubscriber(mode, this, EventActorLoaded.EventName);
            
        }

        public string GetMacAddressString()
        {
            string macAddress = "";

            //each network adapter found
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                PhysicalAddress address = adapter.GetPhysicalAddress();
                byte[] bytes = address.GetAddressBytes();
                
                for (int i = 0; i < bytes.Length; i++)
                    macAddress += bytes[i].ToString("X2");
            }

            return macAddress;
        }

        private new void Awake()
        {
            base.Awake();

            Subscribe(SubscribeMode.Subscribe);
        }

        private void OnEnable()
        {
            Subscribe(SubscribeMode.Subscribe);
        }

        private void OnDisable()
        {
            Subscribe(SubscribeMode.Unsubscribe);
        }

        private void OnDestroy()
        {
            Subscribe(SubscribeMode.Unsubscribe);
        }

    }

    

}