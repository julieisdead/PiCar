using System;
using System.Collections.Generic;
using System.Text;
using Android.Content.PM;

namespace PiCar
{
    interface IOrientate
    {
        void SetOrientation(ScreenOrientation orientation);
    }
}
