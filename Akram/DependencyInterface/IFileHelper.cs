using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.DependencyInterface
{
    // this is the interface that will get the path of the local folder from the platfrom either android or iOS in order
    // to save the database file on that path.
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}
