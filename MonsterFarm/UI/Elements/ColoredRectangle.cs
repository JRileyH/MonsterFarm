﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.UI.DataTypes;

namespace MonsterFarm.UI.Elements
{
    /// <summary>
    /// A colored rectangle with outline.
    /// </summary>
    public class ColoredRectangle : Element
    {
        /// <summary>
        /// Default rectangle styling. Override this dictionary to change the way default rectangles appear.
        /// </summary>
        new public static StyleSheet DefaultStyle = new StyleSheet();

        /// <summary>
        /// Create the rectangle with outline and fill color + outline width.
        /// </summary>
        /// <param name="fillColor">Rectangle fill color.</param>
        /// <param name="outlineColor">Rectangle outline color.</param>
        /// <param name="outlineWidth">Rectangle outline width (0 for no outline).</param>
        /// <param name="size">Rectangle size in pixels.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from position anchor.</param>
        public ColoredRectangle(Color fillColor, Color? outlineColor = null, int outlineWidth = 1, Vector2? size = null, Anchor anchor = Anchor.Auto, Vector2? offset = null) :
            base(size, anchor, offset)
        {
            UpdateStyle(DefaultStyle);
            FillColor = fillColor;
            OutlineColor = outlineColor ?? Color.Black;
            OutlineWidth = outlineWidth;
        }

        /// <summary>
        /// Create the rectangle with outline and fill colors.
        /// </summary>
        /// <param name="fillColor">Rectangle fill color.</param>
        /// <param name="outlineColor">Rectangle outline color.</param>
        /// <param name="size">Rectangle size in pixels.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from position anchor.</param>
        public ColoredRectangle(Color fillColor, Color outlineColor, Vector2 size, Anchor anchor = Anchor.Auto, Vector2? offset = null) :
            base(size, anchor, offset)
        {
            UpdateStyle(DefaultStyle);
            FillColor = fillColor;
            OutlineColor = outlineColor;
        }

        /// <summary>
        /// Create the rectangle with just fill color.
        /// </summary>
        /// <param name="fillColor">Rectangle fill color.</param>
        /// <param name="size">Rectangle size in pixels.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from position anchor.</param>
        public ColoredRectangle(Color fillColor, Vector2 size, Anchor anchor = Anchor.Auto, Vector2? offset = null) :
            base(size, anchor, offset)
        {
            UpdateStyle(DefaultStyle);
            FillColor = fillColor;
            OutlineWidth = 0;
        }

        /// <summary>
        /// Create default colored rectangle.
        /// </summary>
        public ColoredRectangle() : this(Color.White)
        {
        }

        /// <summary>
        /// Create the rectangle with default styling.
        /// </summary>
        /// <param name="size">Rectangle size in pixels.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from position anchor.</param>
        public ColoredRectangle(Vector2 size, Anchor anchor = Anchor.Auto, Vector2? offset = null) :
            base(size, anchor, offset)
        {
            UpdateStyle(DefaultStyle);
        }

        /// <summary>
        /// Draw the element.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw on.</param>
        /// <param name="phase">The phase we are currently drawing.</param>
        override protected void DrawElement(SpriteBatch spriteBatch, DrawPhase phase)
        {
            // get outline width
            int outlineWidth = OutlineWidth;

            // draw outline
            if (outlineWidth > 0)
            {
                Rectangle outlineDest = _destRect;
                outlineDest.X -= outlineWidth;
                outlineDest.Y -= outlineWidth;
                outlineDest.Width += outlineWidth * 2;
                outlineDest.Height += outlineWidth * 2;
                spriteBatch.Draw(Resources.WhiteTexture, outlineDest, OutlineColor);
            }

            // get fill color
            Color fill = FillColor;

            // draw the rectangle
            spriteBatch.Draw(Resources.WhiteTexture, _destRect, fill);

            // call base draw function
            base.DrawElement(spriteBatch, phase);
        }
    }
}
