namespace Strata
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface IWorkspace
    {
        bool SaveWorkspace(string path);
        bool LoadWorkspace(string path);
        bool ClearWorkspace();
        void Activate(bool active);
        string GetWorkspaceTypeName();
        string GetAuthorLockName();
        bool IsRead_OnlyWorkspace();
        List<IActor> GetActorCollection();
        Transform GetWorkspaceTransform();
        bool AddActor(IActor actor);
        Color GetColor();
        string GetSpecificSuffix();



    }
}
