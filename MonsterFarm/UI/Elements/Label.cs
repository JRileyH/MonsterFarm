using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonsterFarm.UI.DataTypes;

namespace MonsterFarm.UI.Elements
{
    /// <summary>
    /// Label element is a subclass of Paragraph. Basically its the same, but with a different
    /// default styling, and serves as a sugarcoat to quickly create labels for widgets.
    /// </summary>
    public class Label : Paragraph
    {

        /// <summary>Default styling for labels. Note: loaded from UI theme xml file.</summary>
        new public static StyleSheet DefaultStyle = new StyleSheet();

        /// <summary>
        /// Create the label.
        /// </summary>
        /// <param name="text">Label text.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="size">Optional label size.</param>
        /// <param name="offset">Offset from anchor position.</param>
        public Label(string text, Anchor anchor = Anchor.Auto, Vector2? size = null, Vector2? offset = null) :
            base(text, anchor, size: size, offset: offset)
        {
            UpdateStyle(DefaultStyle);
        }

        /// <summary>
        /// Create label with default params and empty text.
        /// </summary>
        public Label() : this(string.Empty)
        {
        }
    }
}
