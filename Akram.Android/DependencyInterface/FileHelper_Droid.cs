using System.IO;
using Akram.DependencyInterface;
using Akram.Droid.DependencyInterface;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper_Droid))]
namespace Akram.Droid.DependencyInterface
{
    public class FileHelper_Droid : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}