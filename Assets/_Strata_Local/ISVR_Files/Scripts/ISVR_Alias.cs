namespace Strata
{
    using System.IO;
    using UnityEngine;

    public static class ISVR_Alias 
    {

        public static readonly string RECENTS_FOLDER = Path.Combine(Application.persistentDataPath, "Recents");

        public static void URLShortcutToRecent(string linkName, string linkUrl)
        {
            if (!Directory.Exists(RECENTS_FOLDER))
                Directory.CreateDirectory(RECENTS_FOLDER);

            string shortcutName = Path.Combine(RECENTS_FOLDER, linkName);
            
            if (!File.Exists(shortcutName))
                File.Create(shortcutName).Dispose();
            try
            {
                //FileStream file = File.Open(shortcutName, FileMode.Open, FileAccess.Write);
                File.WriteAllText(shortcutName, linkUrl);
                //StreamWriter writer = new StreamWriter(file);
                //writer.WriteLine(linkUrl);
                //writer.Flush();
                //file.Close();
            }
            catch { }
         }

    }
}
