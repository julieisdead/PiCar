namespace PiCar
{
    internal enum FontAwesomeIconCode
    {
        Check,
        Remove,
        Plus,
        More,
        Refresh,
        ArrowDown,
        ArrowUp,
        ArrowLeft,
        ArrowRight
    }

    /// <summary>
    /// Get more icons from https://zavoloklom.github.io/material-design-iconic-font/
    /// Tip: Just copy and past the icon picture here to get the icon
    /// </summary>
    internal class FontAwesomeIcon
    {
        public string GetIcon(FontAwesomeIconCode value)
        {
            switch (value)
            {
                case FontAwesomeIconCode.Check:
                    return "\uf26b";
                case FontAwesomeIconCode.Remove:
                    return "\uf136";
                case FontAwesomeIconCode.Plus:
                    return "\uf278";
                case FontAwesomeIconCode.Refresh:
                    return "\uf1b9";
                case FontAwesomeIconCode.More:
                    return "\uf19c";
                case FontAwesomeIconCode.ArrowDown:
                    return "\uf2fe";
                case FontAwesomeIconCode.ArrowUp:
                    return "\uf303";
                case FontAwesomeIconCode.ArrowLeft:
                    return "\uf2ff";
                case FontAwesomeIconCode.ArrowRight:
                    return "\uf301";
            }
            return string.Empty;
        }
    }
}
