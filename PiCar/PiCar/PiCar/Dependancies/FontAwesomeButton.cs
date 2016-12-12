using Xamarin.Forms;

namespace PiCar
{
    internal class FontAwesomeButton : Button
    {
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

    }
}