namespace Strata
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Actor))]
    public class ActorInspector : Editor
    {
        IActor actor;
        void OnEnable()
        {
            actor = (IActor)target;
        }
 
        public override void OnInspectorGUI()
        {
            bool isProjectPrefab = IsAPrefabMonoBehaviour(target);
            GUI.backgroundColor = isProjectPrefab ? new Color(.5f, .4f, .3f) : new Color(.3f, .4f, .5f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(isProjectPrefab ? "A Project Actor Prefab" : "A Scene Actor Instance");
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(GUI.skin.textArea);

            DrawDefaultInspector();
            DoTheInspectorThing();

            GUILayout.EndVertical();
        }

        protected virtual void DoTheInspectorThing()
        {

            bool isProjectPrefab = IsAPrefabMonoBehaviour(target);

            //PrefabRootString shoudl draw only if 
            if (isProjectPrefab)
            {
                string rootString = actor.GetPrefabRootString();
                string result = EditorGUILayout.TextField("Prefab Root String", rootString);
                if (result != rootString)
                {
                    actor.SetPrefabRootString(result);
                    EditorUtility.SetDirty(target);
                }
            }
            else
            {
                var prfb = serializedObject.FindProperty("Prefab");
                EditorGUILayout.ObjectField(prfb);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.cyan;
                GUILayout.Label(actor.GetPrefabKey());
                GUILayout.EndHorizontal();
                GUI.color = Color.white;
            }

            //this  section of UI only draws for instances of actors in the scene hierarchy
            if (!isProjectPrefab)
            {
                DoIDBlock();
            }
            else
            {  //this second part is drawn, only for Prefabs that reside in the project hierarchy

                GUILayout.Space(20);

                //GUI.color = new Color(1, .5f, 0);
                bool hasAKey = !string.IsNullOrEmpty(actor.GetPrefabKey());
                Color orange = new Color(1, .5f, 0);
                Color beige = Color.Lerp(orange, Color.white, 0.7f);
                GUI.backgroundColor = hasAKey ? beige : orange;
                GUILayout.BeginHorizontal(GUI.skin.box);
                if (GUILayout.Button("GENERATE UNIQUE KEY", GUILayout.Width(180), GUILayout.Height(hasAKey ? 20 : 100)))
                {
                    if (string.IsNullOrEmpty(actor.GetPrefabRootString()))
                    {
                        if (!EditorUtility.DisplayDialog("PrefabRootString is too short.", "...must be at least 12 characters long.", "Sorry", "||| Insert GameObject name |||"))
                        {
                            actor.SetPrefabRootString(actor.GetName());
                            EditorUtility.SetDirty((UnityEngine.Object)actor);
                        }
                    }

                    if (hasAKey)
                    {
                        if (EditorUtility.DisplayDialog("PrefabKey has already been assigned.",
                            actor.GetPrefabKey()
                            + System.Environment.NewLine
                            + System.Environment.NewLine
                            + "...if you override, you risk orphaning the asset with the previous key.  Override ONLY if this prefab has never been commited to the live database.",
                            "                   Cancel                   ",
                            "Override"))
                            return;
                    }

                    //whip up a unique key based on datetime, user name, and a string provided by the author
                    var dateTime = DateTime.Now.ToString().Replace("/", "");
                    dateTime = dateTime.Replace(":", "");
                    dateTime = dateTime.Replace(" ", "");
                    var user = Environment.UserName.Replace(" ", "").Substring(0, 5);

                    actor.SetPrefabKey(actor.GetPrefabRootString() + "_" + dateTime + user);
                    EditorUtility.SetDirty((UnityEngine.Object)actor);


                }
                GUILayout.TextField(actor.GetPrefabKey());
                GUILayout.EndHorizontal();

                GUI.color = Color.white;
            }


        }

        protected void DoIDBlock()
        {
            GUILayout.Space(10);

            GUI.backgroundColor = Color.grey;
            GUILayout.BeginHorizontal(GUI.skin.button);

            GUI.color = ColorFromActorType(actor.GetActorType());
            GUILayout.Label("ActorType: " + actor.GetActorType().ToString(), EditorStyles.whiteLargeLabel);
            GUI.color = Color.white;

            var id = actor.GetID();
            GUI.backgroundColor = id.IsValid ? id.Color : Color.black;

            GUILayout.BeginHorizontal(GUI.skin.button);

            if (id.IsValid)
            {
                GUI.color = id.Color;
                EditorGUILayout.LabelField("ID", actor.GetID().Value.ToString(), EditorStyles.whiteLargeLabel);
            }
            else
            {
                GUI.color = Color.magenta;
                EditorGUILayout.LabelField("ID", "INVALID", EditorStyles.whiteLargeLabel);
            }
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        Color ColorFromActorType(ActorType type)
        {
            switch (type)
            {
                case ActorType.Anchor:
                    return Color.blue;
                case ActorType.Buildable:
                    return Color.red;
                case ActorType.Root:
                    return Color.yellow;
                case ActorType.Shape:
                    return Color.green;
                case ActorType.Stage:
                    return Color.magenta;

                default:
                    return Color.grey;

            }
        }


        protected bool IsAPrefabMonoBehaviour(UnityEngine.Object obj)
        {
            var mono = (MonoBehaviour)obj;

            if (mono == null)
                return false;


            return PrefabUtility.GetPrefabParent(mono.gameObject) == null && PrefabUtility.GetPrefabObject(mono.gameObject) != null;
        }

    }

    [CustomEditor(typeof(ActorStage))]
    public class ActorStageInspector : ActorInspector
    {
        public override void OnInspectorGUI()
        {
            var actorStage = (ActorStage)target;


            bool isProjectPrefab = IsAPrefabMonoBehaviour(target);
            GUI.backgroundColor = isProjectPrefab ? new Color(.5f, .4f, .3f) : new Color(.3f, .4f, .5f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(isProjectPrefab ? "A Project Actor Prefab" : "A Scene Actor Instance");
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(GUI.skin.textArea);

            GUI.color = Color.Lerp(Color.yellow, Color.grey, .8f);
            GUILayout.Label("Actor Stage is a strange hybrid.", EditorStyles.whiteLargeLabel);
            GUILayout.Label("It inherits WorkspaceBase so it can load stage workspaces.");
            GUILayout.Label("It implements IActor so it can reside in presentation workspaces");
            GUILayout.Label("It implements IActorComponent so it can save/load the stage container file");
            GUILayout.Space(10);

            GUI.color = new Color(.8f, .8f, 1f);

            showDefault = EditorGUILayout.Toggle("Show default inspector", showDefault);
            GUI.color = Color.white;
            if (showDefault)
            {
                GUILayout.BeginVertical(GUI.skin.button);
                DrawDefaultInspector();//or not
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }

            actorStage.AuthorLockName = EditorGUILayout.TextField("Author Lock Name", actorStage.AuthorLockName);
            if (!isProjectPrefab)
            {
                actorStage.ActorRoot = (Actor)EditorGUILayout.ObjectField("ActorRoot", actorStage.ActorRoot, typeof(Actor), true);
            }

            DoTheInspectorThing();

            GUILayout.EndVertical();

            if (actorStage.Prefab != null)
                actorStage.SetPrefabKey(actorStage.Prefab.GetPrefabKey());
        }
        bool showDefault;
    }
}