using System;
using Xamarin.Forms;

namespace PiCar
{
    public enum CarButtonState
    {
        Down,
        Up
    }

    public class CarButton : Button
    {
        public CarButtonState State;
        public event EventHandler Touched;

        public void RaiseTouched(bool down)
        {
            State = down ? CarButtonState.Down : CarButtonState.Up;
            Touched?.Invoke(this, EventArgs.Empty);
        }
    }
}