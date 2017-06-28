namespace Strata
{
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEngine;

    public class EventMonitor : EditorWindow
    {
        bool groupEnabled = true;
        static Texture2D tex;
        Texture2D wallpaper;
        Vector2 scrollPosition;
        static Hashtable ListenerTable;
        GUIStyle guiStyle = new GUIStyle();
        GUIContent animatedTitleContent = new GUIContent("▶EventMonitor");
        //"▶", //\u25BA In Unicode

        [MenuItem("Strata/Monitors/EventMonitor")]
        public static void ShowWindow()
        {
            tex = AnimatedEditorTitleContent.MakeTex(32, 32, new Color(1.0f, 1.0f, 1.0f, 0.1f));
        }

        void Update()
        {
            AnimatedEditorTitleContent.Animate(ref animatedTitleContent, "EventMonitor");
            titleContent = animatedTitleContent;
            Repaint();
        }

        void OnGUI()
        {
            if (wallpaper == null)
            {
                wallpaper = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                wallpaper.SetPixel(0, 0, new Color(0.4f, 0.25f, 0.25f));//dark EventMonitorRed
                wallpaper.Apply();
            }

            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), wallpaper, ScaleMode.StretchToFill);

            if (!EditorApplication.isPlaying)
            {
                GUILayout.Label("Not Playing");
                return;
            }

            EditorGUIUtility.labelWidth = 70;

            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
            groupEnabled = EditorGUILayout.Foldout(groupEnabled, "Enabled");
            if (!groupEnabled)
            {
                GUILayout.EndVertical();
                return;
            }

            if (ListenerTable == null)
                ListenerTable = EventManager.ListenerTable;

            if (tex == null)
                tex = AnimatedEditorTitleContent.MakeTex(32, 32, new Color(1.0f, 1.0f, 1.0f, 0.1f));

            GUILayout.BeginHorizontal();
            GUI.color = Color.white;
            GUILayout.Label("EVENTS", EditorStyles.boldLabel, GUILayout.Width(250));
            GUI.color = Color.yellow;
            GUILayout.Label("SUBSCRIBERS", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUI.color = Color.white;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            GUI.skin.label.fontSize = 10;
            GUI.skin.button.fontSize = 10;

            bool toggle = false;

            Color lightGrey = new Color(.7f, .7f, .7f);

            foreach (DictionaryEntry pair in ListenerTable)
            {
                toggle = !toggle;
                guiStyle.normal.background = toggle ? tex : null;

                GUI.color = lightGrey;
                GUILayout.BeginHorizontal(guiStyle);

                GUI.color = Color.white;
                if (GUILayout.Button(pair.Key.ToString(), GUILayout.Width(250)))
                    Trigger(pair.Key.ToString());

                ArrayList listenerList = pair.Value as ArrayList;

                GUILayout.BeginVertical();
                GUI.color = Color.yellow;

                for (var i = listenerList.Count - 1; i >= 0; --i)
                {
                    i = Mathf.Clamp(i, 0, listenerList.Count - 1);  //  Ran into a case once where i became out of range.  Band-aid fix.  I tried moving anything that modifies _listenerTable to outside this loop with no good results.
                    var listener = (IEventListener)listenerList[i];
                    GUILayout.Label(listener.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(7);
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            EditorUtility.SetDirty(this);
        }

        private void Trigger(string evtName)
        {
            EventManager.TriggerEvent(new EventAnonymous(evtName));
        }

       

    }

    public class EventAnonymous : IEvent
    {
        const string EvtName = "EventAnonymous";
        public EventAnonymous(string name)
        {
            _name = name;
        }
        private string _name;
        public object GetData()
        {
            return _name;
        }

        public string GetName()
        {
            return EvtName;
        }
    }
}
 