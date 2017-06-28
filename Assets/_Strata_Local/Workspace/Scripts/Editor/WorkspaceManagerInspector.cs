namespace Strata
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(WorkspaceManager))]
    public class WorkspaceManagerInspector : Editor
    {
        WorkspaceManager manager;
        void OnEnable()
        {
            manager = (WorkspaceManager)target;
        }


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);


            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("New Presentation Workspace", GUILayout.Height(50)))
                    EventManager.TriggerEvent(new EventWorkspaceNew(WorkspaceManager.WorkspaceType.PresentationEdit));
                if (GUILayout.Button("New Stage Workspace", GUILayout.Height(50)))
                    EventManager.TriggerEvent(new EventWorkspaceNew(WorkspaceManager.WorkspaceType.StageEdit));

                GUILayout.Space(10);

                var workspaces = manager.FriendWorkspaces;

                int count = 0;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                foreach (var workspace in workspaces)
                {
                    if (count >= 10)
                    {
                        count = 0;
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical();
                    }

                    GUI.backgroundColor = workspace.GetColor();

                    if (GUILayout.Button(workspace.GetWorkspaceTypeName(), GUILayout.Height(50)))
                        EventManager.TriggerEvent(new EventWorkspaceSwitch(workspace));

                    count++;
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = Color.magenta;
                GUILayout.Button("The application is not running", GUILayout.Height(50));
                GUI.backgroundColor = Color.white;
            }

        }
    }
}