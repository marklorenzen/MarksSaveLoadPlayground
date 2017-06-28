namespace Strata
{
    using System.IO;
    using UnityEngine;

    public static class ISVR_MetaDataFile
    {
        public static ISVR_MetaData LoadMetaData(string metaDataFileName)
        {
            string path = MakeMetaDataFilePath(metaDataFileName);

            if (File.Exists(path))
            {
                string dataText = File.ReadAllText(path);
                var metaData = ISVR_MetaData_Serializer.Deserialize(dataText);

                return metaData;
            }

            Debug.LogError("Cannot load metadata at " + path);
            return null;

        }

        public static bool SaveMetaData(ISVR_MetaData metaData, string metaDataFileName)
        {
            Debug.Log("Saving MetaData file: " + metaDataFileName);

            string path = MakeMetaDataFilePath(metaDataFileName);

            if (metaData != null && metaData.Validate())
            {
                var dataText = ISVR_MetaData_Serializer.Serialize(metaData);

                FileStream stream;

                if (!File.Exists(path))
                    stream = File.Create(path);
                else
                    stream = File.OpenWrite(path);

                //clear out the previous contents of the metadata file
                stream.SetLength(0);

                byte[] info = new System.Text.UTF8Encoding(true).GetBytes(dataText);
                stream.Write(info, 0, info.Length);

                stream.Close();

                //OpenFolder(Path.GetDirectoryName(path));

                return true;
            }

            Debug.LogError("Cannot save metadata at " + path);
            return false;
        }

        private static string MakeMetaDataFilePath(string fileName)
        {
            var path = ISVR_WorkingTree.StagingRootPath;
            return Path.Combine(path, fileName + ".json");
        }

        public static void OpenFolder(string path)
        {
            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Trouble opening the folder, " + path + ", Maybe because it ain't a path?");
                Debug.LogException(e);
            }
        }

    }
}