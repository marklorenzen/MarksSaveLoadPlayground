namespace Strata
{
    using System;
    using UnityEngine;

    public class ActorComponentBuildablePart : ActorComponent
    {
        [Serializable]
        public new class Data
        {
            [SerializeField]
            float blahblahblah;
        }

        protected ActorComponent.Data _data;
        public override ActorComponent.Data GetData()
        {
            return _data;
        }

        protected override void Awake()
        {
            base.Awake();

            if (_data == null)
            {
                _data = new ActorComponent.Data();
                _data.BuildablePartData = new Data[] { new Data() };
            }

        }

        /// <summary>
        /// TODO this is bogus. should return the parent actor of the buildable
        /// </summary>
        /// <returns></returns>
        public override ActorID GetActorAncestor()
        {
            return ActorID.InvalidID;
        }

    }
}
