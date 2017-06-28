namespace Strata
{
    using UnityEngine;
    using UnityEditor;

    public static class AnimatedEditorTitleContent
    {
        static float spinnyIndex = 0;
        static string[] spinny = new string[]
        {
            "▲", //\u25B2 in Unicode
            "▶", //\u25BA  
            "▼", //\u25BC
            "◀"  //\u25C4
        };

        static public void Animate(ref GUIContent content, string label)
        {
            spinnyIndex = ((float)EditorApplication.timeSinceStartup * 10) % spinny.Length;
            content.text = GetSpinnyTriangle() + label;
        }

        static public string GetSpinnyTriangle()
        {
            return spinny[(int)spinnyIndex];
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
