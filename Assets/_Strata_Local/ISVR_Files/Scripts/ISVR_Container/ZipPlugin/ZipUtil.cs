namespace Strata
{
    using Ionic.Zip;
    using System.IO;
    using System.ComponentModel;

    public class ZipUtil
    {
        public static float progress = 0.0f;

        public static void ExtractFile(string zipToUnpack, string unpackDirectory)
        {
            progress = 0.0f;
            Directory.CreateDirectory(unpackDirectory);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, e) =>
            {
                using (ZipFile zip = ZipFile.Read(zipToUnpack))
                {
                    int step = 0;

                    foreach (ZipEntry file in zip)
                    {
                        file.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                        step++;
                        progress = step / (zip.Count * 1.0f);
                    }
                }
            };

            worker.RunWorkerAsync();
        }

        public static void Unzip(string zipFilePath, string location)
        {
            Directory.CreateDirectory(location);

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {
                zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void Zip(string zipFileName, params string[] files)
        {
            string path = Path.GetDirectoryName(zipFileName);
            Directory.CreateDirectory(path);

            using (ZipFile zip = new ZipFile())
            {
                foreach (string file in files)
                {
                    zip.AddFile(file, "");
                }
                zip.Save(zipFileName);
            }
        }
    }
}