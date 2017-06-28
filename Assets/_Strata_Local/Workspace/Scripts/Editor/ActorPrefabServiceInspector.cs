namespace Strata
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ActorPrefabService))]
    public class ActorPrefabServiceInspector : Editor
    {
        const float GRID_SIZE = 100;

        ActorPrefabService service;
        void OnEnable()
        {
            service = (ActorPrefabService)target;
        }


        bool showDefault;
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            if (!showDefault && GUILayout.Button("▶ Default Inspector", GUI.skin.label))
                showDefault = true;
            if (showDefault && GUILayout.Button("▼ Default Inspector", GUI.skin.label))
                showDefault = false;

            if (showDefault)
                DrawDefaultInspector();
            GUILayout.EndVertical();

            GUILayout.Space(20);

            service.Refresh();
            foreach (var pair in service.Collection)
            {
                GUILayout.BeginHorizontal();
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUILayout.Label(pair.Key);
                SetColorForActor(pair.Value._prefab);
                if (GUILayout.Button(pair.Value._prefab.GetName()))
                    Selection.activeObject = pair.Value._prefab.GetTransform().gameObject;
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            DrawPreviewForActor(ref service.DefaultActorPrefab);
            DrawPreviewForActor(ref service.RootActorPrefab);
            GUILayout.EndHorizontal();
        }

        void DrawPreviewForActor(ref Actor actor)
        {
            if (actor == null)
                return;

            GUILayout.BeginVertical(GUILayout.Width(110));

            if (actor.GetPreview() == null)
            {
                var tex = AssetPreview.GetAssetPreview(actor.gameObject);
                actor.SetPreview(ref tex);
            }
            else
            {
                if (GUILayout.Button(actor.GetPreview(), GUI.skin.box))
                    Selection.activeObject = actor;
            }
            if (GUILayout.Button(actor.name, GUI.skin.textField))
                Selection.activeObject = actor;

            GUILayout.EndVertical();
        }


        void SetColorForActor(IActor actor)
        {
            switch( actor.GetActorType())
            {
                case ActorType.Anchor:
                    GUI.color = Color.green;
                    break;
                case ActorType.Stage:
                    GUI.color = Color.magenta;
                    break;
                case ActorType.Shape:
                    GUI.color = Color.cyan;
                    break;
                case ActorType.Buildable:
                    GUI.color = Color.red;
                    break;
                default:
                    GUI.color = Color.white;
                    break;
            }

            GUI.color = Color.Lerp(GUI.color, Color.white, 0.7f);

        }
    }
}