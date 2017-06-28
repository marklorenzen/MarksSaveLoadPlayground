namespace Strata
{
    using UnityEngine;
    using System.Collections;
    using System.IO;

    public class DownloadUnzipAndApplyImage : MonoBehaviour
    {
        //void OnGUI()
        //{
        //    if (GUI.Button(new Rect(0, 300, 100, 100), "Do it!"))
        //    {
        //        StartCoroutine(Load());
        //    }
        //}

        IEnumerator Load()
        {
            string zipPath = Application.temporaryCachePath + "/tempZip.zip";
            string exportPath = Application.temporaryCachePath + "/unzip";
            string imagePath = exportPath + "/twitter_icon.png";

            
            WWW www = new WWW("https://dl.dropboxusercontent.com/u/56297224/twitter_icon.png.zip");
            //WWW www = new WWW("http://www.etonica.com/images/DownloadGraphic.gif");

            yield return www;

            var data = www.bytes;
            File.WriteAllBytes(zipPath, data);
            ZipUtil.Unzip(zipPath, exportPath);

            var tex = new Texture2D(1, 1);

            var imageData = File.ReadAllBytes(imagePath);
            tex.LoadImage(imageData);

            var image = GetComponent<UnityEngine.UI.RawImage>();
            if (image)
                image.texture = tex;

            File.Delete(zipPath);
            Directory.Delete(exportPath, true);
        }
    }
}
