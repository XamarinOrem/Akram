using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Akram.DependencyInterface;
using Akram.iOS.DependencyInterface;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper_iOS))]
namespace Akram.iOS.DependencyInterface
{
    public class FileHelper_iOS : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }
    }
}