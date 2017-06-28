namespace Strata
{
    using UnityEngine;

    public interface IActorComponent
    {
        ActorID GetID();
        string GetName();
        ActorID GetActorAncestor();
        bool IsActor();
        Transform GetTransform();

        void PreSave();
        void OnSave();
        void OnLoad(ISVR_MetaData.ActorRecord record);
        void PostLoad();

    }
}