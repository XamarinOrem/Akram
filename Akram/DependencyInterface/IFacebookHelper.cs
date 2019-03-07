using Akram.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.DependencyInterface
{
    public interface IFacebookHelper
    {
        void Start(Action<FacebookResponse> callback);
    }
}
