using System;
using Xamarin.Forms;

namespace PiCar
{
    public class CarButton : Button
    {
        public bool State;
        public event EventHandler Touched;

        public void RaiseTouched(bool state)
        {
            State = state;
            Touched?.Invoke(this, EventArgs.Empty);
        }
    }
}