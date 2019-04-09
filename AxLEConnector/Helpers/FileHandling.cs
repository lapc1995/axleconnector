using System;
using System.IO;
using Android.Media;
namespace AxLEConnector.Helpers
{
    public class FileHandling
    {
        public string GetStoragePath()
        {
#if __ANDROID__
            Java.IO.File path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            Java.IO.File dir = new Java.IO.File(path.AbsolutePath + "/mulAccData");
            dir.Mkdirs();
            return dir.AbsolutePath;
#elif __IOS__
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#endif
            throw new Exception("Platform not supported");
        }

        public string SetFile(string filename)
        {
#if __ANDROID__
            Java.IO.File path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            Java.IO.File dir = new Java.IO.File(path.AbsolutePath + "/mulAccData");
            dir.Mkdirs();
            Java.IO.File file = new Java.IO.File(dir, DateTime.UtcNow.ToBinary() + filename + ".csv");
            if (!file.Exists())
            {
                file.CreateNewFile();
                file.Mkdir();
            }

            MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { file.AbsolutePath }, null, null);

            return file.AbsolutePath;
#elif __IOS__
            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)), DateTime.UtcNow.ToBinary() + filename + ".csv");
#endif
        throw new Exception("Platform not supported");
        }
    }
}
