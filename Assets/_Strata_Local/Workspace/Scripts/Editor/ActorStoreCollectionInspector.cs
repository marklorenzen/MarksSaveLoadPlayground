namespace Strata
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ActorPrefabCollection))]
    public class ActorStoreCollectionInspector : Editor
    {
        const float GRID_SIZE = 100;

        ActorPrefabCollection collection;
        void OnEnable()
        {
            collection = (ActorPrefabCollection)target;
        }

        bool showDefault = false;

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

            GUILayout.Space(10);

            collection.RefreshActors();

            GUI.color = collection._color;
            GUILayout.Label("        "+   collection.name + "      Only Allows " + collection._actorType , EditorStyles.whiteLargeLabel);

            SetMutedCollectionColor();

            float xPos = 0;

            GUILayout.BeginHorizontal();//start the first row
            foreach (var actor in collection.Actors)
            {

                var iactor = actor as IActor;

                if (iactor == null)
                    continue;

                GUILayout.BeginVertical(GUILayout.Width(GRID_SIZE));


                GUILayout.BeginHorizontal();
                GUI.color = Color.grey;
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("    Remove Actor Prefab?",
                            "Are you sure you want to remove " + iactor.GetName() + "?", "Remove", "Cancel"))
                    {
                        collection.RemoveActor(actor);
                        EditorUtility.SetDirty(collection);

                    }
                }
                SetMutedCollectionColor();
                GUILayout.EndHorizontal();


                if (iactor.GetPreview() == null)
                {
                    var tex = AssetPreview.GetAssetPreview(actor.gameObject);
                    iactor.SetPreview( ref tex);
                  }
                 
                {
                    if (iactor.GetPreview() != null)
                    {
                        if (GUILayout.Button(iactor.GetPreview(), GUI.skin.box))
                            Selection.activeObject = iactor.GetTransform();
                    }
                }
                if (GUILayout.Button(actor.name, GUI.skin.textArea))
                    Selection.activeObject = actor;




                GUILayout.EndVertical();

                NewRowIfNeccessary(ref xPos);

            }

            NewRowIfNeccessary(ref xPos);

            GUILayout.BeginVertical(GUILayout.Width(GRID_SIZE));
            GUI.color = Color.grey;
            GUILayout.Label("Drag to add Actor");
            MonoBehaviour newActor =  (MonoBehaviour)EditorGUILayout.ObjectField(null, typeof(MonoBehaviour), false, GUILayout.Width(GRID_SIZE * 1.5f), GUILayout.Height(GRID_SIZE*1.33f));
            EditorGUIUtility.labelWidth = 0;

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();//terminate the last row

            GUI.color = Color.white;


            if (newActor != null && newActor is IActor)
            {
                collection.AddActor(newActor as IActor);
                EditorUtility.SetDirty(collection);
            }
        }
 
        void SetMutedCollectionColor()
        {
            GUI.color = Color.Lerp(collection._color, Color.white, .8f);
        }

        void NewRowIfNeccessary( ref float xPos)
        {
            xPos += GRID_SIZE;
            if (xPos + (GRID_SIZE * 1.5f) > Screen.width)
            {
                xPos = 0;
                GUILayout.EndHorizontal();//ding!!
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();//next row
            }
        }

    }
}
