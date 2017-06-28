namespace Strata
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;

    public abstract class WorkspaceBase : MonoBehaviour, IWorkspace 
    {
        public Actor ActorRoot;
        public string AuthorLockName;
        protected HashSet<IActor> _actorCollection = new HashSet<IActor>();
        
        public abstract Color GetColor();
        public abstract string GetSpecificSuffix();

        protected virtual bool IsAllowedActorType(ActorType actorType) { return true; }
        public virtual string GetWorkspaceTypeName() { return GetType().Name; }

        public string GetAuthorLockName() { return AuthorLockName; }
        public Transform GetWorkspaceTransform() { return transform; }//todo cache this as _transform
        public ISVR_MetaData MetaData = new ISVR_MetaData();
        public IActor GetActorRoot() { return ActorRoot; }

        protected virtual void Awake()
        {
            //this sucks-in all the actors that are placed in the workspace's own prefab
            //probably only ever used for testing -ML
            var actors = GetComponentsInChildren<Actor>();

            foreach (var actor in actors)
                _actorCollection.Add(actor);
        }

        /// <summary>
        /// todo, this should use the collections of actors already
        /// populated and validated in the subclass(es)
        /// </summary>
        /// <returns></returns>
        public virtual List<IActor> GetActorCollection() { return _actorCollection.ToList(); }

        string GetLocalUserName()
        {
            return "Mark Lorenzen";
            //todo: instead, retrieve the local user name from the install stats, the name by which this copy of isvr was registered.
        }

        public virtual bool IsRead_OnlyWorkspace()
        {
            string localUserName = GetLocalUserName();
            if (!localUserName.Equals(MetaData._properties._authorLockName))
                return true;

            //todo: add many more conditions that constitute read-only status

            return false;
        }

        public virtual bool AddActor(IActor actor)
        {
            if (IsRead_OnlyWorkspace())
            {
                Debug.LogWarning("not allowing actor addition because workspace is read only");
                return false;
                //todo the disallowing of the addition of any actor should have been preempted farther down the call stack
                //todo there are many more and better ways to prevent a locked author from making edits to workspace, so
                //come up with a catch all way to make a workspace effectively read-only!
            }

            if (_actorCollection.Contains(actor))
                return false;

            if (!IsAllowedActorType(actor.GetActorType()))
            {
                Debug.LogWarning("Disallowing actor " + actor.GetName() + " because it is of type" + actor.GetActorType());
                return false;
            }

            //todo, this assumes there will be only one root actor in the metadata
            if (actor.GetActorType() == ActorType.Root)
            {
                Debug.Assert(ActorRoot == null, "It appears there are more than one ActorRoots in the loaded metadata... wierd");
                ActorRoot = actor as Actor;//sad cast
            }

            _actorCollection.Add(actor);
            actor.GetTransform().SetParent(transform);

            return true;
        }

        public virtual void RemoveAndDestroyActor(IActor actor)
        {
            if (IsRead_OnlyWorkspace())
            {
                Debug.LogWarning("not allowing actor removal because workspace is read only");
                return;
                //todo the disallowing of the addition of any actor should have been preempted farther down the call stack
                //todo there are many more and better ways to prevent a locked author from making edits to workspace, so
                //come up with a catch all way to make a workspace effectively read-only!
            }

            if (_actorCollection.Contains(actor))
                _actorCollection.Remove(actor);

            try
            {
                Destroy(actor.GetTransform().gameObject); ;
            }
            catch
            {
                Debug.LogWarning("you just tried to destroy a destroyed actor");
            }
        }


        public virtual bool SaveWorkspace( string containerPath)
        {
            if (!PopulateMetaData())
                return false;

            //derive the metafile name from the file name of the full path
            //the isvr container file will reside at the full path, and will contain
            //a metafile with the same file name
            containerPath = DecorateFilePathByWorkspaceType(containerPath);

            string metaFileName = Path.GetFileNameWithoutExtension(containerPath);

            MetaData.SetFileName(metaFileName);

            Debug.Log("saving workspace, " + name + "          to metaFile, " + metaFileName + "        into container, " + containerPath);

            if (string.IsNullOrEmpty(MetaData.FileName))//oops never named it!
            {
                Debug.LogWarning("you try to save an unnamed workspace metadata, illegal!");
                return false;
            }

            //this purges the staging root before gathering
            if (!ISVR_WorkingTree.GatherDependencyFiles(MetaData.Dependencies))
                return false;

            if (!ISVR_MetaDataFile.SaveMetaData(MetaData, MetaData.FileName))
                return false;

            if (!ISVR_ContainerZip.ZipFolder(ISVR_WorkingTree.StagingRootPath, containerPath))
                return false;

            ISVR_WorkingTree.PurgeStagingRootPath(ISVR_WorkingTree.StagingRootPath);

            ISVR_MetaDataFile.OpenFolder(Path.GetDirectoryName(containerPath));

            return true;
        }

        public virtual bool LoadWorkspace( string pathToISVRContainer)
        {
            Debug.Log("loading workspace..." + name);

            //clear out whatever schmutz is in the staging area
            ISVR_WorkingTree.PurgeStagingRootPath(ISVR_WorkingTree.StagingRootPath);

            //make a decorated pathToISVRContainer by adding the workspace's suffix just before the file extension 
            pathToISVRContainer = DecorateFilePathByWorkspaceType(pathToISVRContainer);

            //TODO this unzips the "ContainerStaging" folder right into the temporary cachePath
            // which may have been monkeyed around with by Designer Dan between the save
            //and the load... so this is potentially destructive.. hmm
            if (!ISVR_ContainerZip.Unzip(pathToISVRContainer, Application.temporaryCachePath))
                return false;

            //the json file (metaData file) is located in the staging root
            //under the same name as the container (isvr) 
            //LoadMetaData will append the .json extension
            var metaFileName = Path.GetFileNameWithoutExtension(pathToISVRContainer);

            var data = ISVR_MetaDataFile.LoadMetaData(metaFileName);
            if (data == null)
            {
                Debug.LogWarning("could not load metafile " + metaFileName + " for workspace of type, " + GetWorkspaceTypeName());
                return false;
            }

            MetaData = data;

            //make a bunch of actors
            LoadWorkspace(MetaData);//uses the actor records in metadata

            //then another step to allow every actor and actorcomponent
            //a chance to fix up
            PostLoadWorkspace(MetaData);//uses the actor records in metadata

            ISVR_WorkingTree.PurgeStagingRootPath(ISVR_WorkingTree.StagingRootPath);

            return true;
        }

        public virtual bool ClearWorkspace()
        {
            Debug.Log("clearing..." + name);

            var children = new List<GameObject>();
            foreach (Transform child in transform)
                children.Add(child.gameObject);

            children.ForEach(child => Destroy(child));

            _actorCollection.Clear();

            ActorRoot = null;

            return true;
        }

        public virtual void LoadWorkspace(ISVR_MetaData metaData)
        {
            if (metaData == null)
            {
                Debug.LogWarning("Loading Workspace with null metadata -- impossible!");
                return;
            }

            //clear out all the crap that might be in the scene hierarchy under the workspace root 
            ClearWorkspace();

            foreach(var rec in metaData.ActorRecordCollection)
            {
                //first, retrieve the prefab that constitutes this type of actor
                //prefabID is a genetic record, what prefab is common to all actors of this 'kind'
                //as opposed to ActorID, which is unique to an actor individual

                var prefab = PrefabFromActorRecord(rec);

                if (prefab == null)
                {
                    Debug.LogWarning("bad prefab in workspaceBase LoadWorkspace");
                    continue;  
                }

                //todo, it would be great to overload the ctor of EventActorCreate to take the prefabID and do all 
                //of the above work internally to the ActorFactory subscriber
                var createData = new EventActorCreate.Data(prefab);
                EventManager.TriggerEvent(new EventActorCreate(createData));
                
                var newActor = createData._newActor;
                if (newActor == null)
                {
                    Debug.LogWarning("could not get an actor back from EventCreateActor");
                    continue;
                }

                //actor takes on ID and gameobject name
                //and has a chance to instantiate other stuff it needs
                newActor.OnLoad(rec);
 
                AddActor(newActor);

            }

        }

        Actor PrefabFromActorRecord(ISVR_MetaData.ActorRecord record)
        {
            var prefabData = new EventActorPrefabFromID.Data(record._prefabKey);
            EventManager.TriggerEvent(new EventActorPrefabFromID(prefabData));

            return prefabData.Prefab;
        }

        public void PostLoadWorkspace(ISVR_MetaData metaData)
        {
            //now that all the actors exist in the scene and have their IDs and names
            //we can stitch the hierarchy back together
            foreach (var actor in _actorCollection )
            {
                //Debug.Log("about to postload the actor " + actor.GetName());
                actor.PostLoad(); 
            }
        }

        public void Activate(bool active)
        {
            //todo, the ActorFactory usedID's seems to get stale each time we switch
            //workspaces, perhaps it needs a little nudge
            //the intention is to keep any and all concurrent workspaces full
            //of validly ID'd actors

            gameObject.SetActive(active);

            //give the subclass a chance to react to the activation
            OnActivated(active);
        }

        protected virtual void OnActivated(bool active)
        {
            Camera.main.backgroundColor = GetColor()*.2f;//todo hack hack hack
        }

        protected bool PopulateMetaData()
        {
            //the actors in the workspace are all nested under the RootTransformActor
            //so all we need is that root and the serializer will do the whole
            //tree resursively, assuming there is an unbroken heritage to all
            //the actors in the hierarchy

            if (MetaData == null)
                return false;

            MetaData.Populate(this);

            return true;
        }
 

        public string DecorateFilePathByWorkspaceType(string path)
        {
            //insert the suffix right before the dot
            var filename = Path.GetFileName(path);
            path.Replace(filename, filename + GetSpecificSuffix());
            return path;
        }


#if UNITY_EDITOR
        private void OnGUI()
        {
            GUI.color = Color.Lerp(Color.white, GetColor(), .5f);
            GUI.Label(new Rect(10, 10, 200, 20), GetWorkspaceTypeName(), GUI.skin.box);
            GUI.color = Color.white;
        }
#endif
    }
    

    public static class ISVR_WorkingTree
    {
        const string CONTAINER_STAGING_FOLDER_NAME = "ContainerStaging";

        public static string StagingRootPath
        {
            get { return Path.Combine(Application.temporaryCachePath, CONTAINER_STAGING_FOLDER_NAME); }
        }

        public static bool GatherDependencyFiles(ISVR_DependencyManifest manifest)
        {
            if (manifest == null)
                return false;

            var dependencies = manifest.Dependencies;

            if (dependencies == null)
                return false;
 
            if (!Directory.Exists(StagingRootPath))
                Directory.CreateDirectory(StagingRootPath);

            //erase any and all files located beneath the stagingRootPath;
            PurgeStagingRootPath(StagingRootPath);

            foreach (var dependency in dependencies)
            {
                //find the file located at the dependency URL
                var fullPath = Path.Combine(Application.streamingAssetsPath, dependency._url);

                if (File.Exists(fullPath))
                {
                    //Debug.Log("Found dependency:" + dependency._url);

                    //stuff this file into the staging folder so we can zip it into container with the metadata
                    var destinationPath = Path.Combine(StagingRootPath, dependency._url);
                    var directory = Path.GetDirectoryName(destinationPath);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    //hmm, if a previous actor's dependency is already copied here, then should 
                    //we assume that the file already present is good to go? Yes, because
                    //the whole folder was just purged, so any file we find is extremely fresh
                    if (File.Exists(destinationPath))
                    {
                        //Debug.Log("since the dependency file " + destinationPath + " already exists, skipping.");
                        continue;
                    }

                    File.Copy(fullPath, destinationPath, true);
                }
                else
                    Debug.LogWarning("failed to find the dependency file located at " + dependency._url);

            }

            return true;
        }

        public static void PurgeStagingRootPath(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            var files = di.GetFiles();
            if (files != null && files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    if (file.Name != CONTAINER_STAGING_FOLDER_NAME)
                    {
                        file.Delete();
                    }
                }
            }

            var directories = di.GetDirectories();
            if (directories != null && directories.Length > 0)
            {
                foreach (DirectoryInfo dir in directories)
                {
                    if (dir.Name != CONTAINER_STAGING_FOLDER_NAME)
                    {
                        dir.Delete(true);
                    }
                }
            }
        }
    }

}