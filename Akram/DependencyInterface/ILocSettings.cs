using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.DependencyInterface
{
    public interface ILocSettings
    {
        bool CheckGpsEnable();

        void OpenSettings();
    }
}
