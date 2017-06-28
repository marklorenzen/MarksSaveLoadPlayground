namespace Strata
{
    using UnityEngine;

    public class WorkspaceShowMode: WorkspaceBase
    {
        //TODO: this kind of workspace should never save, and it should always and only ever
        //load isvr files of the "presentation edit" type (with the PE suffix)
        public override string GetSpecificSuffix() { return WorkspacePresentationEdit.PRESENTATION_EDIT_SUFFIX; }//yah, the other guy's suffix, so we can load that file type
        public override Color GetColor() { return Color.magenta; }
        public override bool IsRead_OnlyWorkspace() { return true; }
        public override bool AddActor(IActor actor) { return false; } //this should never allow the addition of actors... we are in presenting mode, yo
    }
}
