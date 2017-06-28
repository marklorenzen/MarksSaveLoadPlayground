namespace Strata
{
    using UnityEngine;

    public class WorkspacePresentationEdit: WorkspaceBase 
    {
        public const string PRESENTATION_EDIT_SUFFIX = "_PE";
        public ActorStage Stage;

        public override string GetSpecificSuffix() { return PRESENTATION_EDIT_SUFFIX; }
        public override Color GetColor() { return Color.yellow; }

        protected override void Awake()
        {
            base.Awake();

            if (Stage != null)
                _actorCollection.Add(Stage);
        }

        public override bool IsRead_OnlyWorkspace()
        {
            //todo add all the presentation-edit specific reasons to be read-only
            return base.IsRead_OnlyWorkspace();
        }

        public override bool AddActor(IActor actor)
        {
            if ( base.AddActor(actor))
            {
                if (actor.GetActorType() == ActorType.Stage)
                {
                    Debug.Log("ooh, look: a stage");
                    Stage = actor as ActorStage;
                }

                return true;
            }

            return false;
        }

        void AddStage(ActorStage stage)
        {
            if (Stage == stage)
               return; //leave well enough alone

            if (Stage != null)
                DestroyStage();

            Stage = stage;

            AddActor(stage);
           
        }

        void DestroyStage()
        {
            base.RemoveAndDestroyActor(Stage);
            Stage = null;
        }

        public override bool SaveWorkspace(string containerPath)
        {
            if (Stage != null && !_actorCollection.Contains(Stage))
                _actorCollection.Add(Stage);

            return base.SaveWorkspace(containerPath);
        }

        public override bool ClearWorkspace()
        {
            DestroyStage();
            return base.ClearWorkspace();

        }

        protected override bool IsAllowedActorType(ActorType actorType)
        {
            switch (actorType)
            {
                case ActorType.Shape:
                case ActorType.Stage:
                case ActorType.Root:
                    return true;
            }

            return false;
        }

    }
}
