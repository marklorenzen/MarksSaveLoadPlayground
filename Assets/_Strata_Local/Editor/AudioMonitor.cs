namespace Strata
{
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;

    public class AudioMonitor : EditorWindow
    {
                                                                //"▶", //\u25BA In Unicode
        GUIContent animatedTitleContent = new GUIContent("▶AudioMonitor");
        AudioSource[] AllAudioSources = new AudioSource[0];
        float overallWidth;
        Vector2 scroll = Vector2.zero;
        Texture2D tex;
        bool _dirty;

        [MenuItem("Strata/Monitors/AudioMonitor")]
        static void Init()
        {
            AudioMonitor ct = (AudioMonitor)GetWindow(typeof(AudioMonitor));
            ct.titleContent = new GUIContent("\u25BAAudioMonitor"); ;
        }

        void Update()
        {
            AnimatedEditorTitleContent.Animate(ref animatedTitleContent, "AudioMonitor");
            titleContent = animatedTitleContent;
            Repaint();
         }

        void RefreshAudioSources()
        {
            AllAudioSources =  FindObjectsOfType<AudioSource>();
        }

        private void OnGUI()
        {
            if (tex == null)
            {
                tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, new Color(0.25f, 0.4f, 0.25f));//dark AudioMonitorGreen
                tex.Apply();
            }

            GUI.DrawTexture(new Rect(0,0,position.width,position.height), tex, ScaleMode.StretchToFill);

            if (GUILayout.Button("SCAN", GUILayout.Height(30)) || _dirty)
            {
                RefreshAudioSources();
            }

            if (AllAudioSources != null || AllAudioSources.Length > 0)
            {
                DoToolGUI();
            }
            
        }

        void DoToolGUI()
        {
 
            overallWidth = position.width;
            GUILayout.BeginHorizontal();
            GUILayout.Label("AudioSource",  GUILayout.Width(overallWidth / 2));
            GUILayout.Label("Clip",  GUILayout.Width(overallWidth / 2));
            GUILayout.EndHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            ShowAudioSources();

            EditorGUILayout.EndScrollView();

        }

 
        Color prefabBlue = new Color(.5f, .8f, 1f);

        void ShowAudioSources()
        { 
            foreach (var src in AllAudioSources)
            {
                if (src == null)
                {
                    RefreshAudioSources();
                    return;
                }

                Color buttonColor = Color.white;

                if (PrefabUtility.GetPrefabType(src.gameObject) == PrefabType.PrefabInstance)
                    buttonColor = prefabBlue;

                //darken if not an active gameobject
                if (!src.gameObject.activeInHierarchy)
                    buttonColor = Color.Lerp(buttonColor,Color.grey, 0.5f);

                bool isSelected = (Selection.activeGameObject == src.gameObject && Selection.activeGameObject != null)
                    ||
                    (Selection.activeObject == src.clip && Selection.activeObject != null);

                if (isSelected)
                {
                    GUI.color = Color.green;
                     GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));
                }
                else
                    GUILayout.BeginHorizontal();

                GUI.color = buttonColor;
                string sourceName = src.name;
                if (src.isPlaying)
                    sourceName = AnimatedEditorTitleContent.GetSpinnyTriangle() + sourceName; 

                if (GUILayout.Button(sourceName, GUILayout.Width(overallWidth / 2))) 
                {
                    Selection.activeGameObject = src.gameObject;
                }

                if (src.clip != null)
                {
                    string clipLabel = src.clip.name;


                    if (isSelected)
                    {
                        if (src.clip == currentPlayingClip || src.isPlaying) 
                        {
                            GUI.color = Color.Lerp(Color.yellow, Color.red, 0.5f);
                            clipLabel = "Stop " + clipLabel;
                        }
                        else
                            clipLabel = "Play " + clipLabel;
                    }

                    if (GUILayout.Button(clipLabel))
                    {

                        if (isSelected)
                        {
                            if (Application.isPlaying)
                            {
                                if (!src.isPlaying)
                                    src.Play();
                                else
                                {
                                    src.Stop();
                                    _dirty = true;
                                     
                                }
                            }
                            else //edit mode
                            {
                                PlayOrStopClip(src.clip);
                            }

                        }
                        Selection.activeObject = src.clip;
                    }
                }
                else
                    GUILayout.Label("");

                GUILayout.EndHorizontal();
            }

            GUI.color = Color.white;
        }

        static AudioClip currentPlayingClip;

        static void PlayOrStopClip(AudioClip clip)
        {
            if (currentPlayingClip == clip)
                StopAllClips();
            else
                PlayClip(clip);
        }

        public static void PlayClip(AudioClip clip)
        {
            currentPlayingClip = clip;

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "PlayClip", BindingFlags.Static | BindingFlags.Public,
                null, new System.Type[] { typeof(AudioClip) }, null );

            method.Invoke( null,  new object[] { clip } );
        }

        public static void StopAllClips()
        {
            currentPlayingClip = null;

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "StopAllClips", BindingFlags.Static | BindingFlags.Public,
                null, new System.Type[] { }, null );

            method.Invoke( null, new object[] { } );
        }

    }

     
}