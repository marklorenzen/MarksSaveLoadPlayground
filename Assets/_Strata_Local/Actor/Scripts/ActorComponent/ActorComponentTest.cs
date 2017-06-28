namespace Strata
{
    using System;

    public class ActorComponentTest : ActorComponent
    {

        public override ActorID GetActorAncestor()
        {
            //TODO this is only a test class, but if it evolves into something real
            //then make it a real parent here if one is present

            return ActorID.InvalidID;
        }

        protected override void Awake()
        {
            base.Awake();

            //do stuff
        }

        public override void OnLoad(ISVR_MetaData.ActorRecord record)
        {
            throw new NotImplementedException();
        }

        public override void OnSave()
        {
            throw new NotImplementedException();
        }

        public override void PostLoad()
        {
            throw new NotImplementedException();
        }

        public override void PreSave()
        {
            throw new NotImplementedException();
        }

    }
}
