namespace Strata
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(WorkspaceBase))]
    public class WorkspaceInspector : Editor
    {
        WorkspaceBase workspace;
        void OnEnable()
        {
            workspace = (WorkspaceBase)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (EditorApplication.isPlaying && !workspace.gameObject.activeSelf)
            {
                GUI.backgroundColor = workspace.GetColor();
                if (GUILayout.Button("A C T I V A T E", GUILayout.Height(50), GUILayout.Width(140)))
                    EventManager.TriggerEvent(new EventWorkspaceSwitch(workspace));
                GUI.contentColor = Color.white;

            }
        }
    }

    [CustomEditor(typeof(WorkspacePresentationEdit))]
    public class PresentationWorkspaceInspector : WorkspaceInspector
    {
        public override void OnInspectorGUI() { base.OnInspectorGUI(); }
    }
    [CustomEditor(typeof(WorkspaceStageEdit))]
    public class StageWorkspaceInspector : WorkspaceInspector
    {
        public override void OnInspectorGUI() { base.OnInspectorGUI(); }
    }
}