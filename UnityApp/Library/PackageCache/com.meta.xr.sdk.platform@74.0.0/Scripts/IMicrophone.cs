//This file is deprecated.  Use the high level voip system instead:
// https://developer.oculus.com/documentation/unity/ps-voip/

using UnityEngine;
using System.Collections;
using System;

namespace Oculus.Platform
{
    public interface IMicrophone
    {
        void Start();

        void Stop();

        float[] Update();
    }
}
