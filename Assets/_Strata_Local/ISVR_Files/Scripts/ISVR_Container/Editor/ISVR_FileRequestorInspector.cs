namespace Strata
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    [CustomEditor(typeof(ISVR_FileRequestorClient))]
    public class ISVR_FileRequestorInspector : Editor
    {
        Vector2 scrollPos;
        const int RECENT_ALIAS_COUNT = 4;
        ISVR_FileRequestorClient client;
        Texture isvrIcon;
        Texture genericFileIcon;
        bool isvrOnly;
        string yetUnnamedNewFolder = string.Empty;

        void OnEnable()
        {
            genericFileIcon = EditorGUIUtility.ObjectContent(null, typeof(TextAsset)).image;
            isvrIcon = EditorGUIUtility.ObjectContent(null, typeof(TerrainCollider)).image;
            client = target as ISVR_FileRequestorClient;
            yetUnnamedNewFolder = string.Empty;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (client == null || !client.gameObject.activeSelf)
                return;

            GUILayout.Space(25);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("New folder", GUILayout.Width(80)))
            {
                string root = client._path;

                if (string.IsNullOrEmpty(yetUnnamedNewFolder))
                {
                    //make a new folder and let its name be pending
                    int n = 1;
                    string newFolderName = "NewFolder";
                    while (Directory.Exists(Path.Combine(root, newFolderName)) && n < 100)
                    {
                        newFolderName = "NewFolder" + (++n).ToString("00");
                    }

                    yetUnnamedNewFolder = Path.Combine(root, newFolderName );
                    Directory.CreateDirectory(yetUnnamedNewFolder);
                }
            }

            GUILayout.FlexibleSpace();

            GUILayout.Label(client._qualifyingSuffix, GUI.skin.textField, GUILayout.Width(40));

            if (isvrOnly)
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Show All file types", GUILayout.Width(150)))
                {
                    isvrOnly = false;
                }
            }
            else
            {
                if (GUILayout.Button("Show ISVR files only", GUILayout.Width(150)))
                {
                    isvrOnly = true;
                    if (!client.currentFilePath.EndsWith("isvr", true, System.Globalization.CultureInfo.CurrentCulture))
                        client.currentFilePath = string.Empty;
                }
            }

            GUI.backgroundColor = Color.white;


            GUILayout.EndHorizontal();
            GUILayout.Space(5); 

            GUILayout.BeginHorizontal();

            //parent folder button
            if ( GUILayout.Button("▲", GUILayout.Width(25)))
            {
                var info = Directory.GetParent(client._path);
                if (info != null)
                {
                    client._path = info.FullName;
                    EditorUtility.SetDirty(this);
                }
            }

            //breadcrumbs
            GUILayout.BeginHorizontal(GUI.skin.textField);
            var crumbs = client._path.Split(new char[] { Path.DirectorySeparatorChar, '/' });
            for (int i = 0; i < crumbs.Length; i++)
            {
                if ( GUILayout.Button(crumbs[i] + " > ", GUI.skin.textField))
                {
                    //build a new path
                    var newPath = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        newPath += crumbs[j] + Path.DirectorySeparatorChar;
                    }
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        client._path = newPath.TrimEnd(Path.DirectorySeparatorChar);
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

            GUILayout.BeginHorizontal();

            //shortcuts bar
            var lightGrey = Color.Lerp(Color.grey, Color.white, .5f);
            GUI.backgroundColor = lightGrey;
            int sidebarWidth = 200;
            GUILayout.BeginVertical(GUI.skin.button, GUILayout.Width(sidebarWidth));

            var driveLetters = new string[]
                {"C:/","D:/","E:/","F:/","G:/","H:/","I:/","J:/","K:/"};

            string newBase = string.Empty;

            foreach (var letter in driveLetters)
            {
                if (Directory.Exists(letter))
                {
                    if (GUILayout.Button(letter, GUILayout.Height(30)))
                    {
                        newBase = letter;
                        break;
                    }
                }
            }

            GUILayout.Space(10);

            GUI.backgroundColor = Color.Lerp(lightGrey, Color.yellow, .1f);
            if (GUILayout.Button("AppData", GUILayout.Height(30) ))
                newBase = Application.persistentDataPath;

            if (GUILayout.Button("StreamingAssets", GUILayout.Height(30) ))
                newBase = Application.streamingAssetsPath;

            if (GUILayout.Button("DataPath", GUILayout.Height(30) ))
                newBase = Application.dataPath;

            if (GUILayout.Button("Desktop", GUILayout.Height(30) ))
                newBase = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            if (GUILayout.Button("Documents", GUILayout.Height(30) ))
                newBase = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            var recentPaths = GetRecentPathsList();
            if (recentPaths.Count > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("Recent");

                GUI.backgroundColor = Color.Lerp(lightGrey, Color.green, .1f);


                foreach (var recent in recentPaths)
                {
                    string foldername = recent.Split(Path.DirectorySeparatorChar).Last();

                    string recentLabel = recent;

                    if (recent.Length > 43)
                        recentLabel = recent.Substring(0, 10) + "..." + recent.Substring(recent.Length - 30, 30);

                    //if (GUILayout.Button(".../" + foldername, GUILayout.Height(70), GUILayout.Width(sidebarWidth)))
                    if (GUILayout.Button(recentLabel, GUILayout.Height(30) ))
                        newBase = recent;

                }
            }

            GUI.backgroundColor = Color.white;

            if (!string.IsNullOrEmpty(newBase))
            {
                if (Directory.Exists(newBase))
                {
                    client._path = newBase;
                    AddPathToRecents(newBase);

                }
            }

            GUI.color = Color.white;
            GUILayout.EndVertical();

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUI.skin.textArea, GUILayout.MaxHeight(450) );
            foreach (var folder in GetFolders())
            {
                string fullPath = Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar);
                string foldername = fullPath.Split(Path.DirectorySeparatorChar).Last();

                if (folder == yetUnnamedNewFolder)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("▇▆ ", GUILayout.Width(15));

                    var newName = GUILayout.TextField(yetUnnamedNewFolder);
                    if (newName != yetUnnamedNewFolder)
                    {
                        //editing name
                        Directory.Move(yetUnnamedNewFolder, newName);
                        yetUnnamedNewFolder = newName;
                    }
                    GUILayout.EndHorizontal();

                    if (GetNewNameDone())
                    {
                        yetUnnamedNewFolder = string.Empty;
                        EditorUtility.SetDirty(this);
                    }
                }
                else
                {
                    bool pressed = false;

                    GUILayout.BeginHorizontal();
                    GUI.color = new Color(1, .9f, .5f);

                    if (GUILayout.Button("▇▆ ", GUI.skin.label, GUILayout.Width(20)))
                        pressed = true;

                    GUI.color = Color.white;

                    if (GUILayout.Button(foldername, GUI.skin.label, GUILayout.Height(20)))
                        pressed = true;

                    if (pressed)
                    {
                        var evt = Event.current;

                        if (evt.button == 1)//right "context" click
                        {
                            System.Diagnostics.Process.Start(client._path);
                        }
                        else
                        {
                            client._path = folder;

                            AddPathToRecents(folder);

                            yetUnnamedNewFolder = string.Empty;
                            EditorUtility.SetDirty(this);
                        }
                    }
                    GUILayout.EndHorizontal();
                }  
                
            }
            foreach (var file in GetFiles())
            {
                string fullPath = Path.GetFullPath(file).TrimEnd(Path.DirectorySeparatorChar);
                string filename = fullPath.Split(Path.DirectorySeparatorChar).Last();

                bool isvrFile = filename.EndsWith(".isvr", true, System.Globalization.CultureInfo.CurrentCulture);
                bool suffixMatch = string.IsNullOrEmpty(client._qualifyingSuffix) || Path.GetFileNameWithoutExtension(filename).EndsWith(client._qualifyingSuffix);

                if (isvrOnly && !isvrFile)
                    continue;

                if (!suffixMatch)
                    continue;

                GUILayout.BeginHorizontal();

                var icon = isvrFile ? isvrIcon : genericFileIcon;

                GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));

                GUI.backgroundColor = Color.grey;

                bool isCurrentFilePath = (file == client.currentFilePath);
                GUIStyle style = isCurrentFilePath ? GUI.skin.textArea : GUI.skin.label;
                GUI.color = isCurrentFilePath ? Color.green : Color.white;

                if (GUILayout.Button(filename, style))
                {
                    if (isvrFile || !isvrOnly)
                    {
                        client.currentFilePath = file;
                        EditorUtility.SetDirty(this);
                    }
                }
            
                GUI.backgroundColor = Color.white;
                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }

            GUI.color = Color.white;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            //extra space at the bottom
            GUILayout.Space(30);

            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUI.backgroundColor = Color.grey;
            client.currentFilePath = GUILayout.TextField(client.currentFilePath);
            GUI.backgroundColor = Color.white;
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if ( EditorApplication.isPlaying && !string.IsNullOrEmpty(client.currentFilePath))
            {
                if (GUILayout.Button("Save", GUILayout.Width(100)))
                    client.Save();

                if (GUILayout.Button("Load", GUILayout.Width(100)))
                    client.Load();
            }

            GUILayout.EndHorizontal();
        }

        void AddPathToRecents(string path)
        {
            if (!path.Contains(":"))
                return;

            if (path.Length <= 3)
                return;

            string aliasName = path.Replace('\\', '_');
            aliasName = aliasName.Replace('/', '_');
            aliasName = aliasName.Replace(':', '_');
            aliasName = aliasName.Replace(' ', '_');

            ISVR_Alias.URLShortcutToRecent(aliasName, path);

            EditorUtility.SetDirty(this);
        }

        Dictionary<string, DateTime> _recentItems = new Dictionary<string, DateTime>();
        Queue<string> _recentPaths = new Queue<string>();

        /// <summary>
        /// todo parts of this fn could be done more sparsely, as in a
        /// slowly updating coroutine
        /// </summary>
        /// <returns></returns>
        List<string> GetRecentPathsList()
        {
            //code to examine the contents of Windows' own recents folder
            string recentFolder = ISVR_Alias.RECENTS_FOLDER;

            //update the dictionary
            //this can be done sparsely
            var lnkFiles = Directory.GetFiles(recentFolder);
            foreach (var lnkFile in lnkFiles)
            {
                if (!File.Exists(lnkFile))
                    continue;
                try
                {
                    string url = File.ReadAllText(lnkFile);

                    DateTime timeStamp = File.GetLastWriteTime(lnkFile);

                    if (!_recentItems.ContainsKey(lnkFile))
                        _recentItems.Add(lnkFile, timeStamp);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                 
            }

            //check for outdated files, and delete them 
            //both the file from the file system, and the dictionary entry
            //outdated means: older than the Nth youngest file (N=RECENT_ALIAS_COUNT)

            //default
            DateTime cutoff = DateTime.Now.AddYears(-1);//a year ago today

            if (_recentItems.Count > RECENT_ALIAS_COUNT)
            {
                var times = _recentItems.Values.ToList();
                times.Sort((a, b) => b.CompareTo(a));
                cutoff = times[RECENT_ALIAS_COUNT];

            }

            List<String> toRemove = new List<string>();
            foreach (var item in _recentItems)
            {
                //too old to keep or show?
                if (item.Value < cutoff)
                {
                    toRemove.Add(item.Key);
                }
                else if (!File.Exists(item.Key))
                {
                    toRemove.Add(item.Key);
                }
            }

            foreach( var victim in toRemove)
            {
                try
                {
                    File.Delete(victim);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                EditorUtility.SetDirty(this);
                _recentItems.Remove(victim);
            }

            var result = _recentItems.Keys.ToList();

            result.Sort((a, b) => a.CompareTo(b));

            return result;

        }

        public string[] GetFolders()
        {
            try
            {
                return Directory.GetDirectories(client._path);
            }
            catch
            {
                return new string[] { "sorry, no such folder as "+ client._path };
            }
        }

        public string[] GetFiles()
        {
            try
            {  
                return Directory.GetFiles(client._path);
            }
            catch
            {
                return new string[] { "sorry, no such folder as " + client._path };
            }
        }

        bool GetNewNameDone()
        {
            if (Event.current.type == EventType.MouseDown)
                return true;
            if (Event.current.keyCode == KeyCode.KeypadEnter ||
                Event.current.keyCode == KeyCode.Return)
                return true;

            return false;
        }

    }
}