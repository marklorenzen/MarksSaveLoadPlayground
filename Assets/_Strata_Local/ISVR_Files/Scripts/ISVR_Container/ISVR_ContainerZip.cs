namespace Strata
{
    using UnityEngine;
    using System.IO;
    using Ionic.Zip;

    public static class ISVR_ContainerZip  
    {
        public static string baseDirectryPath = "C://";

        public static bool ZipFolder(string stagingRootPath, string containerDestinationPath )
        {
            Debug.Log("  Zipping files from:" + stagingRootPath + " to destination container: " + containerDestinationPath);

            if (!Directory.Exists(stagingRootPath))
            {
                Debug.LogError("not found: " + stagingRootPath);
                return false;
            }

            ZipFile zip = new ZipFile();
            
            ZipRecursive(zip, stagingRootPath);

            if (zip.Count > 0)
                zip.Save(containerDestinationPath);

            OpenFolder(Path.GetDirectoryName(containerDestinationPath));

            return true;
        }

        

        private static void ZipRecursive(ZipFile zip, string path)
        {  
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                string directoryPath = Path.GetDirectoryName(file);

                //truncate the root folder (the 'dropFolder') from beginning of directory name
                int position = directoryPath.IndexOf(Path.DirectorySeparatorChar);
                if (position > 0)
                    directoryPath = directoryPath.Substring(position);
                else
                    directoryPath = string.Empty;


                zip.AddFile(file, directoryPath);
            }

            var folders = Directory.GetDirectories(path);

            foreach (var folder in folders)
            {  
                ZipRecursive(zip, folder);
                continue;
            }
           
        }

        public static bool Unzip( string containerPath, string stagingFolderPath)
        {
            if (! File.Exists(containerPath))
            {
                Debug.LogError(containerPath + "is not found!");

                OpenFolder(Path.GetDirectoryName(containerPath));
                return false;
            }

            ZipUtil.Unzip(containerPath, stagingFolderPath);

            OpenFolder(Path.GetDirectoryName(containerPath));

            return true;
        }

        public static void OpenFolder( string directoryPath )
        {
#if UNITY_EDITOR
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            try
            {
                System.Diagnostics.Process.Start(directoryPath);
            }
            catch
            {
                Debug.LogWarning("oops, could not open directoryPath in windows explorer for you, sorry.");
            }
#endif
        }


       


    }
}