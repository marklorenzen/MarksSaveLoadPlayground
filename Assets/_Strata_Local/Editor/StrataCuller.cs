namespace Strata
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class StrataCuller : EditorWindow, System.Collections.Generic.IComparer<System.Type>
    {
#if USE_UNTYPED_CONTAINERS
        // I'd prefer that we adopt generic Dictionary over Hashtable in the future for type safety and optimization opportunities.
        // I believe that's in the modern guidelines too. Feel free to object!
        private class ValueArray : System.Collections.ArrayList { }
        private class KeyValueStorage : System.Collections.Hashtable
        {
            public KeyValueStorage(StrataCuller self) : base()
            {
            }
        }
#else
        // Using "SortedDictionary" gives us sorted keys for free while building the UI.
        // I found I really wanted sorted entries in the UI while using this tool.
        private class ValueArray : System.Collections.Generic.List<GameObject> { }
        private class KeyValueStorage : System.Collections.Generic.SortedDictionary<System.Type, ValueArray>
        {
            public KeyValueStorage(StrataCuller self) : base(self)
            {
            }
        }
#endif

        KeyValueStorage _monoBehaviorsInActiveScenes;
        KeyValueStorage _monoBehaviorsInProject;
        Dictionary<string, UnityEngine.Object>  _assetsInProject;

        string[] _monoTypesReport = new string[0];
        Vector2 _masterScrollPos;
        Vector2 _sceneMonoScrollPosition;
        Vector2 _projectMonoScrollPosition;
        Vector2 _projectAssetPosition;
        bool _showSceneUI;
        bool _showProjectUI;
        bool _showAssembliesUI;
        bool _showAssets;
        bool _sortPartialNames;
        bool _sortDescending;
        float _overallWidth;
        readonly Color _rhubarb = new Color(.9f, .8f, 1);
        readonly Color _paleCyan = Color.Lerp(Color.cyan, Color.white, .5f);
        readonly Color _paleGreen = Color.Lerp(Color.white, Color.green, .5f);

        string _extensionsFilter = "tga, exr, png, wav, hdr, psd,cubemap,fbx,tif";

        const string exx = "✘";
        const string chk = "✔";
                                                       //"▶", //\u25BA In Unicode
        GUIContent animatedTitleContent = new GUIContent("▶StrataCuller");
        Texture2D wallpaper;

        public StrataCuller()
        {
            // Support IComparable callbacks to ourself.
            _monoBehaviorsInActiveScenes = new KeyValueStorage(this);
            _monoBehaviorsInProject = new KeyValueStorage(this);
            _assetsInProject = new Dictionary<string, UnityEngine.Object>();

            //static so this collection survives an editor-rebuild operation
            //saving mucho tiempo
            if (_usedAssetList == null)
                _usedAssetList = new HashSet<string>();
        }

        [MenuItem("Strata/Monitors/StrataCuller")]
        public static void Launch()
        {
            StrataCuller window = (StrataCuller)GetWindow(typeof(StrataCuller));
            window.titleContent = new GUIContent("StrataCuller");
            window.Show();
        }

        void Update()
        {
            var before = animatedTitleContent.text;
            AnimatedEditorTitleContent.Animate(ref animatedTitleContent, "StrataCuller");

            if (before != animatedTitleContent.text)//optimization, this editor is starting to bog with long reports
            {
                titleContent = animatedTitleContent;
                Repaint();
            }

            if (_assetScanRoutine != null)
                _assetScanRoutine.MoveNext();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is MonoScript)
                filter = Selection.activeObject.name;
        }

        public void OnGUI()
        {
            
            if (wallpaper == null)
            {
                wallpaper = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                wallpaper.SetPixel(0, 0, new Color(0.25f, 0.25f, 0.4f));//dark StrataCullerBlue
                wallpaper.Apply();
            }
            

            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), wallpaper, ScaleMode.StretchToFill);

            _overallWidth = position.width - 1;

            GUI.color = Color.white;
            //_masterScrollPos = GUILayout.BeginScrollView(_masterScrollPos);

            

            //search filter row
            GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 20));
            {
                GUILayout.Label("Options:");
                GUILayout.Space(1);
                if (_sortPartialNames != GUILayout.Toggle(_sortPartialNames, "Exclude Namespace when Sorting"))
                {
                    _sortPartialNames = !_sortPartialNames;
                    RefreshMonoAssembliesList();
                }
                GUILayout.Space(1);
                if (_sortDescending != GUILayout.Toggle(_sortDescending, "Sort Descending"))
                {
                    _sortDescending = !_sortDescending;
                    RefreshMonoAssembliesList();
                }
            }
            GUILayout.EndHorizontal();

            

            GUILayout.BeginHorizontal(GUILayout.Width(_overallWidth *.6f));
            {
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUILayout.Label("Filter:");
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                filter = GUILayout.TextField(filter);
            }
            GUILayout.EndHorizontal();

            


            GUI.color = Color.white;
            _masterScrollPos = GUILayout.BeginScrollView(_masterScrollPos);
            {

                GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 10));
                {
                    _showSceneUI = EditorGUILayout.Foldout(_showSceneUI, "Components in Active Scenes");
                    if (_showSceneUI)
                    {
                        DoSceneMonoUI();
                    }
                }
                GUILayout.EndVertical();

                GUI.color = _rhubarb;
                GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 10));
                {
                    _showProjectUI = EditorGUILayout.Foldout(_showProjectUI, "MonoBehaviours in Project");
                    if (_showProjectUI)
                    {
                        DoProjectMonoUI();
                    }
                }
                GUILayout.EndVertical();


                GUI.color = _paleCyan;
                GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 10));
                {
                    _showAssembliesUI = EditorGUILayout.Foldout(_showAssembliesUI, "MonoBehaviours in Assemblies");
                    if (_showAssembliesUI)
                    {
                        DoAssembliesUI();
                    }
                }
                GUILayout.EndVertical();


                GUI.color = _paleGreen;
                GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 10));
                {
                    _showAssets = EditorGUILayout.Foldout(_showAssets, "Assets in Project");
                    if (_showAssets)
                    {
                        DoAssetsUI();
                    }
                }
                GUILayout.EndVertical();



            }
            GUILayout.EndScrollView();
        }


        IEnumerator _assetScanRoutine;
        

        void BeginAssetScanRoutine()
        {
            _assetScanRoutine = GetAssetsInProject();
        }

        void BeginUsedAssetsFromLogRoutine()
        {
            _assetScanRoutine = RefreshUsedAssetList();
        }

        static HashSet<string> _usedAssetList; 

        IEnumerator GetAssetsInProject()
        {

            if (_usedAssetList.Count == 0)
            {
                EditorUtility.ClearProgressBar(); 
                EditorUtility.DisplayDialog("No used assets found in Editor.log.", "Do a build first. Until you make a local Windows Standalone build, this tool can't compare project assets with those that actually get built.  Note: It is recommended that you quit the editor and delete Editor.log file before making a new build. These log files get pretty huge.", "Gotcha, Chief.");
                yield break;
            } 

            _assetsInProject.Clear();

            var extensions = _extensionsFilter.Split(new char[] { ',', ';' });
            foreach ( var ext in extensions)
            {
                string extension = ext.Trim();

                string path = "Assets";
                foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
                {
                    path = AssetDatabase.GetAssetPath(obj);
                    if (File.Exists(path))
                    {
                        path = Path.GetDirectoryName(path);
                        break;
                    }
                }

                var paths = AssetDatabase.GetAllAssetPaths().Where(s => s.EndsWith("."+ extension, StringComparison.OrdinalIgnoreCase) 
                && s.StartsWith(path));
                float progress = 0;
                float count = 0;
                float total = paths.Count();
                foreach (string s in paths) 
                {
                    progress = count / total;
                    count++;

                    EditorUtility.DisplayProgressBar("Scanning Assets in " + path, "With extension: " + ext, progress);

                    var asset =  AssetDatabase.LoadAssetAtPath(s, typeof(UnityEngine.Object));
                    if (asset != null)
                    {
                        string label = s;
                        if (!_usedAssetList.Contains(s))
                            label = exx + label;

                        _assetsInProject.Add(label, asset);
                    }

                    if (count%20<1 || count < 20)
                        yield return null;
                }

            }

            EditorUtility.ClearProgressBar();
            
            _assetScanRoutine = null;

            yield break;
        }

        private bool IsUnityBehavior(MonoBehaviour mb)
        {
            return IsUnityBehavior(mb.GetType());
        }

        private bool IsUnityBehavior(System.Type T)
        {
            if (T.IsSubclassOf(typeof(UIBehaviour)))
                return true;

            //TODO add more cases; types that we would exclude from out report

            return false;
        }

        private void RefreshMonoAssembliesList()
        {
            filter = "";
            RefreshMonoInScenesList();
            RefreshMonoInProjectList();
            _monoTypesReport = BuildMonoAssembliesList();
        }

        private void RefreshMonoInScenesList()
        {
            filter = "";
            _monoBehaviorsInActiveScenes.Clear();

            var mbs = Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour)) as MonoBehaviour[];

            if (mbs == null)
                return;

            foreach (MonoBehaviour mb in mbs)
            {
                if (string.IsNullOrEmpty(mb.gameObject.scene.name))
                    continue;

                if (IsUnityBehavior(mb))
                    continue;

                if (!_monoBehaviorsInActiveScenes.ContainsKey(mb.GetType()))
                {
                    _monoBehaviorsInActiveScenes[mb.GetType()] = new ValueArray();
                }

                ((ValueArray)_monoBehaviorsInActiveScenes[mb.GetType()]).Add(mb.gameObject);
            }
        }

        private string[] BuildMonoAssembliesList()
        {
            filter = "";
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var A in AS)
            {
                System.Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        if (T.IsSubclassOf(typeof(UIBehaviour)))
                            continue;

                        result.Add(T);
                    }
                }
            }

            result.Sort(this.Compare);
            //result.Sort((typA, typB) => typA.Name.CompareTo(typB.Name));

            var output = new System.Collections.Generic.List<string>();
            float unusedCount = 0;
            foreach (var type in result)
            {
                string reportLine = type.Name + "     (" + type.FullName + ")";
                if (_monoBehaviorsInProject.ContainsKey(type))
                {
                    reportLine = chk + reportLine;
                    reportLine += "     Used in: [";
                    foreach (GameObject gameObject in (ValueArray)_monoBehaviorsInProject[type])
                        reportLine += " " + gameObject.name;
                    reportLine += " ]";
                }
                else
                {  
                    unusedCount++;
                    reportLine = exx + reportLine;
                }


                output.Add(reportLine);
            }

            output.Insert(0, "      UNUSED MONOBEHAVIOUR CLASSES = " + (100f * unusedCount / result.Count).ToString("0.0") + "%");

            return output.ToArray();
        }

        private void RefreshMonoInProjectList()
        {
            _monoBehaviorsInProject.Clear();

            var mbs = Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour)) as MonoBehaviour[];

            if (mbs == null)
                return;

            foreach (MonoBehaviour mb in mbs)
            {
                if (!string.IsNullOrEmpty(mb.gameObject.scene.name))
                    continue;

                if (IsUnityBehavior(mb))
                    continue;


                if (!_monoBehaviorsInProject.ContainsKey(mb.GetType()))
                {
                    _monoBehaviorsInProject[mb.GetType()] = new ValueArray();
                }

                ((ValueArray)_monoBehaviorsInProject[mb.GetType()]).Add(mb.gameObject);
            }

            _monoTypesReport = BuildMonoAssembliesList();
        }

        string filter = "";


        private void DoAssetsUI()
        {
            GUILayout.Space(10);

            //scan button row
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 20));

            //the extensions filter, comma separated
            GUILayout.BeginHorizontal();
            {
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUILayout.Label("Extensions to scan:");
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.color = Color.white;
                _extensionsFilter = GUILayout.TextField(_extensionsFilter, GUILayout.MinWidth(200));
                GUI.color = _paleGreen;
            }
            GUILayout.EndHorizontal();

            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("(1) Scan Assets Used In Most Recent Build"))
                {
                    BeginUsedAssetsFromLogRoutine();
                }

                bool showScanButton = (_assetScanRoutine == null && _usedAssetList.Count > 0);
                GUI.color = showScanButton ? _paleGreen : Color.grey;

                if (GUILayout.Button("(2) Scan all Assets in Selected Folder") && showScanButton)
                {
                    BeginAssetScanRoutine();
                }
            }
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();

            if (_assetsInProject.Count() == 0)
            {
                GUILayout.Label("No Results To Show");
                return;
            }
            
            if (_killList.Count > 0)
            {
                GUI.color = Color.cyan;
                GUILayout.BeginHorizontal();
                GUILayout.Label("");
                if (GUILayout.Button("DELETE WITH PREJUDICE", GUILayout.Height(50), GUILayout.Width(200)))
                {
                    int result = EditorUtility.DisplayDialogComplex("Deleting Assets from Project", "All the assets marked in blue will be permanently deleted. Are you sure?", "Yup: kill em.", "Wait, what? No!", "Aw heck, no!");

                    if (result == 0)
                    {

                        Debug.Log(result);


                        foreach (var path in _killList)
                        {
                            string originalPath = path.TrimStart('✘');

                            bool deleted = AssetDatabase.DeleteAsset(originalPath);

                            if (deleted)
                            {
                                _assetsInProject.Remove(path);
                            }
                            else
                            {
                                EditorApplication.Beep();
                                Debug.LogWarning("Could not delete asset: " + path);
                            }
                        }

                        _killList.Clear();
                    }
                }
                GUILayout.EndHorizontal();
                GUI.color = _paleGreen;
            }

            GUILayout.BeginHorizontal();
            GUI.color = Color.red;
            GUILayout.Label("");
            bool selectAll = GUILayout.Button("all", GUILayout.Width(90));
            GUI.color = _paleGreen;
            GUILayout.EndHorizontal();



            _unusedScrollPosition = GUILayout.BeginScrollView(_unusedScrollPosition);

            foreach (var reportPair in _assetsInProject)
            {
                bool unused = reportPair.Key.StartsWith(exx);

                GUI.color = unused ? Color.red : _paleGreen;

                GUILayout.BeginHorizontal();

                string label = reportPair.Value == null ? "null" : reportPair.Value.name;



                if (GUILayout.Button(label, GUILayout.Width(_overallWidth * .3f)))
                    Selection.activeObject = reportPair.Value;

                if (Selection.activeObject == reportPair.Value)
                    GUI.color = Color.yellow;

                    GUILayout.Label(reportPair.Key);


                if (_killList.Contains(reportPair.Key))
                    GUI.color = Color.cyan;

                if (unused && (selectAll || GUILayout.Button("del", GUILayout.Width(50))))
                {
                    if (_killList.Contains(reportPair.Key))
                        _killList.Remove(reportPair.Key);
                    else
                        _killList.Add(reportPair.Key);

                }

                GUILayout.EndHorizontal();
             }

            GUILayout.EndScrollView();
            

        }
        Vector2 _unusedScrollPosition = Vector2.zero;

        HashSet<string> _killList = new HashSet<string>();


        private void DoAssembliesUI()
        {
            GUILayout.Space(10);

            //scan button row
            GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 20));
            GUILayout.Label(" ");
            GUILayout.FlexibleSpace();
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button("Scan all MonoBehaviours in Assemblies"))
            {
                RefreshMonoAssembliesList();
            }
            GUILayout.EndHorizontal();

            foreach (var reportLine in _monoTypesReport)
            {
                if (!string.IsNullOrEmpty(reportLine))
                {
                    if (!(reportLine.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0))
                        continue;

                    if (reportLine.StartsWith(exx))
                    {
                        GUI.color = Color.white;
                        GUILayout.Label(reportLine);
                    }
                    else
                    {
                        GUI.color = _paleCyan;
                        GUILayout.TextArea(reportLine);
                    }

                }
            }

        }

        private void DoProjectMonoUI()
        {
            GUI.color = _rhubarb;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 20));
            GUILayout.Label(" ");
            GUILayout.FlexibleSpace();

            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button("Scan all MonoBehaviours in Project"))
            {
                RefreshMonoInProjectList();
            }
            GUILayout.EndHorizontal();

            _projectMonoScrollPosition = GUILayout.BeginScrollView(_projectMonoScrollPosition, false, true, GUILayout.Width(_overallWidth - 10));//,GUILayout.MinHeight(200)

            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

            foreach (System.Type type in _monoBehaviorsInProject.Keys)
            {
                GUILayout.Space(4);

                string name = type.Name;

                if (!(name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;
                
                GUILayout.Label(name + ":");

                foreach (GameObject gameObject in (ValueArray)_monoBehaviorsInProject[type])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    if (GUILayout.Button(gameObject.name, GUILayout.MinWidth(200)))
                    {
                        Selection.SetActiveObjectWithContext(gameObject, gameObject);
                    }
                    GUILayout.EndHorizontal();

                }
            }

            GUILayout.EndScrollView();

            GUI.color = Color.white;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

        }

        private void DoSceneMonoUI()
        {
            GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"), GUILayout.Width(_overallWidth - 20));
            GUILayout.Label(" ");
            GUILayout.FlexibleSpace();

            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            if (GUILayout.Button("Scan all Components in active Scene(s)"))
            {
                RefreshMonoInScenesList();
            }
            GUILayout.EndHorizontal();

            _sceneMonoScrollPosition = GUILayout.BeginScrollView(_sceneMonoScrollPosition, false, true, GUILayout.Width(_overallWidth - 10));//,GUILayout.MinHeight(200)

            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

            Color lightBlue = new Color(.5f, .7f, 1);
            GUI.color = lightBlue;
            foreach (System.Type type in _monoBehaviorsInActiveScenes.Keys)
            {
                bool hasLabeledType = false;

                //first the active gameObjects
                foreach (GameObject gameObject in (ValueArray)_monoBehaviorsInActiveScenes[type])
                {
                    if (!gameObject.activeInHierarchy)
                        continue;

                    string name = type.Name;
                    if (!string.IsNullOrEmpty(filter) && !name.Contains(filter))

                    if ( ! (name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0))
                        continue;



                    if (!hasLabeledType)
                    {
                        GUILayout.Label(this.DisplayName(type) + ":");
                        hasLabeledType = true;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    if (GUILayout.Button(gameObject.name))
                    {
                        Selection.SetActiveObjectWithContext(gameObject, gameObject);
                    }
                    GUILayout.EndHorizontal();
                }

            }
            Color lightGrey = new Color(.7f, .7f, .7f);
            GUI.color = lightGrey;
            foreach (System.Type type in _monoBehaviorsInActiveScenes.Keys)
            {
                bool hasLabeledType = false;

                //then all the inactive ones...
                foreach (GameObject gameObject in (ValueArray)_monoBehaviorsInActiveScenes[type])
                {
                    if (gameObject.activeInHierarchy)
                        continue;

                    if (!hasLabeledType)
                    {
                        GUILayout.Label(this.DisplayName(type) + ":");
                        hasLabeledType = true;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    if (GUILayout.Button(gameObject.name))
                    {
                        Selection.SetActiveObjectWithContext(gameObject, gameObject);
                    }
                    GUILayout.EndHorizontal();
                }

            }

            GUILayout.EndScrollView();

            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

        }

        private string SortName(System.Type type)
        {
            // This implementation selects which type name is used for sorting.
            return (_sortPartialNames? type.Name : type.FullName);
        }
 
        private string DisplayName(System.Type type) // We could pass a format string or varargs additions
        {
            // This implementation selects which type name is used for display.
            string result = (_sortPartialNames ? type.Name : type.FullName);
            if (_sortPartialNames && !string.IsNullOrEmpty(type.Namespace))
            {
                result += " (in " + type.Namespace + ")";
            }
            return result;
        }
 
        public int Compare(System.Type x, System.Type y)
        {
            if (x == null || y == null)
                throw new System.ArgumentException("At least one argument is null");
            int result = this.SortName(x).CompareTo(this.SortName(y));
            return (_sortDescending? -result : result);
        }




        

        public IEnumerator RefreshUsedAssetList( )
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string logfile = localAppData + "\\Unity\\Editor\\Editor.log";
            string strataCachePath = localAppData + "\\Unity\\Editor\\StrataCache";


            FileStream logFile = new FileStream(logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (logFile == null)
            {
                Debug.Log("Could not find Editor Log... that is really, really weird.");
                _assetScanRoutine = null;
                yield break;
            }

            
            DateTime lastModified = File.GetLastWriteTime(strataCachePath);
            //if cached "today"
            if (lastModified > DateTime.Now.AddHours(-12))
            {
                ParseUsedAssetsFromCache(strataCachePath);
                _assetScanRoutine = null;
                yield break;
            }

            float progress = 0;
            
            progress = Mathf.Lerp(progress, 1f, 0.001f);
            EditorUtility.DisplayProgressBar("Scanning Assets in most recent build", "Examining Build Log...", progress);

            yield return null;
 
            
            _usedAssetList.Clear();
            StreamReader reader = new StreamReader(logFile);
            string line = "";

            int counter = 0;
            while (!reader.EndOfStream && !(line = reader.ReadLine()).StartsWith("Complete size"))
            {
                if (++counter % 300 < 1)
                {
                    progress = Mathf.Lerp(progress, 1f, 0.001f);
                    EditorUtility.DisplayCancelableProgressBar("Scanning Assets in Project", "Examining Build Log... Seeking Build Report" + counter, progress);

                    yield return new WaitForEndOfFrame();
                }
            }
            while (!reader.EndOfStream && !(line = reader.ReadLine()).StartsWith("Used Assets"))
            {
                progress = Mathf.Lerp(progress, 1f, 0.001f);
                EditorUtility.DisplayCancelableProgressBar("Scanning Assets in Project", "Examining Build Log... Seeking Used Assets Section" + (++counter), progress);

                yield return new WaitForEndOfFrame();
            }
            while (!reader.EndOfStream && !string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                line = line.Substring(line.IndexOf("% ") + 2);

                progress = Mathf.Lerp(progress, 1f, 0.001f);
                EditorUtility.DisplayCancelableProgressBar("Scanning Assets in Project", "Examining Build Log... Recording Used Assets " + (++counter), progress);

                if (line.StartsWith("Assets"))
                {
                    _usedAssetList.Add(line);

                    //Debug.Log("adding line " + line);
                }
                else
                    continue;

                yield return new WaitForEndOfFrame();
            }

            if (_usedAssetList.Count == 0)
            {
                EditorUtility.DisplayDialog("No used assets found in Editor.log.", "Do a build first. Until you make a local Windows Standalone build, this tool can't compare project assets with those that actually get built.  Note: It is recommended that you quit the editor and delete Editor.log file before making a new build. These log files get pretty huge.", "Gotcha, Chief.");
            }
            else
            {
                SaveUsedAssetsToCacheFile(strataCachePath);
            }

            logFile.Close();

            EditorUtility.ClearProgressBar();

            _assetScanRoutine = null;

            yield return null;
        }

        void ParseUsedAssetsFromCache( string path )
        {
            if (!File.Exists(path))
                return;

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (file != null)
            {
                _usedAssetList.Clear();
                StreamReader reader = new StreamReader(file);
                while (reader != null)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    _usedAssetList.Add(line);
                }

                file.Close();

            }

           
        }

        void SaveUsedAssetsToCacheFile( string path )
        {
            if (!File.Exists(path))
                File.Create(path);

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

            if (file != null)
            {
                StreamWriter writer = new StreamWriter(file);
                foreach (var line in _usedAssetList)
                    writer.WriteLine(line);

                file.Close();
            }

        }



    }
}
 