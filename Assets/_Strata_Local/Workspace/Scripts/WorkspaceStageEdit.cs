namespace Strata
{
    using UnityEngine;

    public class WorkspaceStageEdit: WorkspaceBase 
    {
        public const string STAGE_EDIT_SUFFIX = "_SE";

        public override string GetSpecificSuffix() { return STAGE_EDIT_SUFFIX; }
        public override Color GetColor() { return Color.blue; }

        public override bool IsRead_OnlyWorkspace()
        {
            //todo add all the stage-edit specific reasons to be read-only
            return base.IsRead_OnlyWorkspace();
        }

        IActor _anchorAsset;
        public IActor AnchorAsset
        {
            get { return _anchorAsset; }
            private set { _anchorAsset = value; }
        }

        bool IsAnchorAsset( IActor actor)
        {
            return actor == AnchorAsset;
        }

        bool IsAnchorAsset( ActorID id)
        {
            return id == AnchorAsset.GetID();
        }

        bool AssignAnchorAsset(IActor newAnchorAsset)
        {
            //validate the IActor param before we do anything hasty and destructive
            if (newAnchorAsset == null)
                return false;

            if (IsAllowedActorType(newAnchorAsset.GetActorType()))
                return false;

            if (newAnchorAsset == AnchorAsset)
                return true; // redundant, return true

            // rid ourselves of the current anchor
            RemoveCurrentAnchorAsset();

            //accept the new anchor asset
            AnchorAsset = newAnchorAsset;

            //place that IActor transform into the hierarchy and scene space 
            var trans = newAnchorAsset.GetTransform();
            trans.SetParent(GetWorkspaceTransform());
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;

            return true;
        }

        void RemoveCurrentAnchorAsset()
        {
            //remove previous actor from the hierarchy of the workspace
            EventManager.TriggerEvent(new EventActorDestroy(AnchorAsset));
            //todo: should the OnDestroy of ActorComponent remove its ID from the factory?
            //todo: should the Awake of ActorComponent add its ID to the factory

            //nullify the current AnchorAssetID;
            AnchorAsset = null;

        }

        protected override bool IsAllowedActorType(ActorType actorType)
        {
            switch (actorType)
            {
                case ActorType.Buildable:
                case ActorType.Anchor:
                case ActorType.Root:
                    return true;
            }

            return false;
        }
     }
}
