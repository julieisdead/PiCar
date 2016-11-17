using Xamarin.Forms;

namespace PiCar
{
    public class FontAwesomeButton : Button
    {
        //Must match the exact "Name" of the font which you can get by double clicking the TTF in Windows
        public const string Typeface = "FontAwesome";
        private FontAwesomeIconCode _fontAwesomeIcon;

        public FontAwesomeIconCode FontAwesomeIcon
        {
            get { return _fontAwesomeIcon; }
            set
            {
                FontFamily = Typeface;
                switch (value)
                {
                    case FontAwesomeIconCode.Check:
                        Text = Icon.Check;
                        break;
                    case FontAwesomeIconCode.Remove:
                        Text = Icon.Remove;
                        break;
                    case FontAwesomeIconCode.Plus:
                        Text = Icon.Plus;
                        break;
                    case FontAwesomeIconCode.Refresh:
                        Text = Icon.Refresh;
                        break;
                    case FontAwesomeIconCode.More:
                        Text = Icon.More;
                        break;
                    default:
                        Text = string.Empty;
                        break;
                }
                _fontAwesomeIcon = value;
            }
        }

        /// <summary>
        /// Get more icons from http://fortawesome.github.io/Font-Awesome/cheatsheet/
        /// Tip: Just copy and past the icon picture here to get the icon
        /// </summary>
        public static class Icon
        {
            public static readonly string Check = "\uf26b";
            public static readonly string Remove = "\uf136";
            public static readonly string Plus = "\uf278";
            public static readonly string More = "\uf19c";
            public static readonly string Refresh = "\uf1b9";
        }

        public enum FontAwesomeIconCode
        {
            Check,
            Remove,
            Plus,
            More,
            Refresh
        }
    }
}
