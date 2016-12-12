using System;
using Xamarin.Forms;

namespace PiCar
{
    internal enum CarButtonState
    {
        Down,
        Up
    }

    internal class CarButton : Button
    {
        public CarButtonState State;
        public event EventHandler Touched;
        public const string Typeface = "FontAwesome";
        private FontAwesomeIconCode _fontAwesomeIcon;

        public FontAwesomeIconCode FontAwesomeIcon
        {
            get { return _fontAwesomeIcon; }
            set
            {
                FontAwesomeIcon icon = new FontAwesomeIcon();
                FontFamily = Typeface;
                Text = icon.GetIcon(value);
                _fontAwesomeIcon = value;
            }
        }

        public void RaiseTouched(bool down)
        {
            State = down ? CarButtonState.Down : CarButtonState.Up;
            Touched?.Invoke(this, EventArgs.Empty);
        }
    }
}