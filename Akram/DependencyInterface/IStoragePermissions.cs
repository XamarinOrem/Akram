using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Akram.DependencyInterface
{
    public interface IStoragePermissions
    {
        void GetContactPermissions();

        void GetScannerPermissions(); 

        void GetLocationPermissions();
    }
}
