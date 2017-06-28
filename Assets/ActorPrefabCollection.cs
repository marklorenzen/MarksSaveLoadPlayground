namespace Strata
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ActorPrefabCollection : MonoBehaviour
    {
        public List<MonoBehaviour> Actors = new List<MonoBehaviour>();

        public delegate void SynchronizeDelegate(MonoBehaviour prefab);

        public ActorType _actorType;
         
        public Color _color = Color.cyan;

        public void Synchronize(SynchronizeDelegate syncFn)
        {
            RefreshActors();
            foreach (var actor in Actors)
                syncFn(actor);
        }

        public void RefreshActors()
        {
            for (int i = Actors.Count-1; i >= 0; --i)
            {
                var iactor = Actors[i] as IActor;
                if (iactor == null)
                {
                    RemoveActor(Actors[i]);
                    continue;
                }

                if (iactor.GetActorType() != _actorType)
                    RemoveActor(Actors[i]);
            }
        }

        public void RemoveActor(MonoBehaviour actor)
        {
            if (Actors.Contains(actor))
                Actors.Remove(actor);
        }

        public void AddActor( IActor actor)
        {
            //reject the wrong types of actors
            if (actor.GetActorType() != _actorType)
                return;

            var iactor = actor as MonoBehaviour;

            if (iactor == null)
                return;

            if (Actors.Contains(iactor))
                return;

            Actors.Add(iactor);
         }

    }
}