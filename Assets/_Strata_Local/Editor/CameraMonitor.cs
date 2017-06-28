namespace Strata
{
    using UnityEngine;
    using UnityEditor;

    public class CameraMonitor : EditorWindow
    {
        GUIContent animatedTitleContent = new GUIContent("▶CamMonitor");
        Camera[] AllCameras = new Camera[0];
        float overallWidth;
        Vector2 scroll = Vector2.zero;
        Texture2D tex;
 
        [MenuItem("Strata/Monitors/CameraMonitor")]
        static void Init()
        {
            CameraMonitor ct = (CameraMonitor)GetWindow(typeof(CameraMonitor));
            ct.titleContent = new GUIContent("\u25BACameraMonitor"); ;
        }

        void Update()
        {
            AnimatedEditorTitleContent.Animate(ref animatedTitleContent, "CamMonitor");
            titleContent = animatedTitleContent;
            Repaint();
         }

        void RefreshCameras()
        {
            AllCameras =  FindObjectsOfType<Camera>();
        }

        private void OnGUI()
        {
            if (tex == null)
            {
                tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, new Color(0.3f, 0.15f, 0.3f));//dark CameraMonitorMaroon
                tex.Apply();
            }

            GUI.DrawTexture(new Rect(0,0,position.width,position.height), tex, ScaleMode.StretchToFill);

            if (GUILayout.Button("SCAN", GUILayout.Height(30)))
            {
                RefreshCameras();
            }

            if (AllCameras != null || AllCameras.Length > 0)
            {
                DoToolGUI();
            }
            
        }

        void DoToolGUI()
        {
            overallWidth = position.width;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera",  GUILayout.Width(overallWidth / 2));
            GUILayout.Label("Mask",  GUILayout.Width(overallWidth / 2));
            GUILayout.EndHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            ShowCameras();

            EditorGUILayout.EndScrollView();
        }
 
        Color prefabBlue = new Color(.5f, .8f, 1f);

        void ShowCameras()
        { 
            foreach (var cam in AllCameras)
            {
                if (cam == null)
                {
                    RefreshCameras();
                    return;
                }

                Color buttonColor = Color.white;

                if (PrefabUtility.GetPrefabType(cam.gameObject) == PrefabType.PrefabInstance)
                    buttonColor = prefabBlue;

                //darken if not an active game object
                if (!cam.gameObject.activeInHierarchy)
                    buttonColor = Color.Lerp(buttonColor,Color.grey, 0.5f);

                bool isSelected = (Selection.activeGameObject == cam.gameObject && Selection.activeGameObject != null);

                if (isSelected)
                {
                    GUI.color = Color.magenta;
                    GUILayout.BeginHorizontal(GUI.skin.GetStyle("Button"));
                }
                else
                    GUILayout.BeginHorizontal();

                GUI.color = buttonColor;
                string sourceName = cam.name;
                if (cam.enabled && cam.gameObject.activeInHierarchy)
                    sourceName = AnimatedEditorTitleContent.GetSpinnyTriangle() + sourceName; 

                if (GUILayout.Button(sourceName, GUILayout.Width(overallWidth / 2))) 
                {
                    Selection.activeGameObject = cam.gameObject;
                }

                string maskReport = "";
                for (int i = 0; i < 32; i++)
                {
                    if ((cam.cullingMask & (1 << i)) != 0)
                        maskReport += LayerMask.LayerToName(i) + "   ";
                }

                if (string.IsNullOrEmpty(maskReport))
                    maskReport = "Nothing";

                GUILayout.TextArea(maskReport);

                GUILayout.EndHorizontal();
            }

            GUI.color = Color.white;
        }

    }

}