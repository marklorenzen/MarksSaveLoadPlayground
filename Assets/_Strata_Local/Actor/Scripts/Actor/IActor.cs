namespace Strata
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum ActorType
    {
        Unknown = 0,
        Root,
        Buildable,
        Shape,
        Anchor,
        Stage 
    }

    public interface IActor
    {
        ActorID GetID();
        ActorID GetActorParent();
        Transform GetTransform();
        string GetParentName();
        string GetName();
        ActorType GetActorType();
        void SetActorType(ActorType type);
        List<ActorComponent> GetActorComponents();
        string GetPrefabRootString();
        void SetPrefabRootString(string id);
        string GetDependencyURL();
        Texture2D GetPreview();
        void SetPreview(ref Texture2D preview);
        string GetPrefabKey();
        void SetPrefabKey(string key);

        //todo: these are redundant between IActor and IActorComponent
        // I think these should only be defined in IActorComponent
        //which may require some fancy casting to get a workspace full of IActors 
        //to save and load
        void PreSave();
        void OnSave();
        void OnLoad(ISVR_MetaData.ActorRecord record);
        void PostLoad();
    }
}