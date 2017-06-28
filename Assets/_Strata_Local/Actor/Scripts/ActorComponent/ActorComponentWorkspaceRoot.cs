namespace Strata
{
    using UnityEngine;

    /// <summary>
    /// The ActorComponent which must accompany the Actor at the root of every actor prefab.
    /// ActorComponentWorkspaceRoot knows about actor type and other nitty gritty facts about
    /// the actor and its parts which the simple Actor class doesn't and shouldn't know.
    /// </summary>
    [RequireComponent(typeof(Actor))]  
    public class ActorComponentWorkspaceRoot : ActorComponent
    {
        public Actor _siblingActor;
        public ActorType _actorType;
         
        public override Data GetData() { return null; }
        public ActorType GetActorType() { return _actorType; }

        public override ActorID GetActorAncestor()
        {
            if (_siblingActor = null)
                _siblingActor = GetComponent<Actor>();

            if (_siblingActor == null)
            {
                Debug.LogWarning("What the heck? no Actor component found by ActorComponentWorkspaceRoot");
                return ActorID.InvalidID;
            }

            return _siblingActor.GetID();
        }
 
    }
}