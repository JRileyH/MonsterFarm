﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterFarm.UI.Elements
{
    /// <summary>
    /// An horizontal line, used to separate between different sections of a panel or to emphasize headers.
    /// </summary>
    public class HorizontalLine : Element
    {
        // frame width in texture size, in percents.
        static Vector2 FRAME_WIDTH = new Vector2(0.2f, 0f);

        /// <summary>Default styling for the horizontal line. Note: loaded from UI theme xml file.</summary>
        new public static StyleSheet DefaultStyle = new StyleSheet();

        /// <summary>
        /// Create the horizontal line.
        /// </summary>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        public HorizontalLine(Anchor anchor, Vector2? offset = null) :
            base(Vector2.Zero, anchor, offset)
        {
            // locked by default, so we won't do events etc.
            Locked = true;

            // update style
            UpdateStyle(DefaultStyle);

            // get line texture and set default height
            Texture2D texture = Resources.HorizontalLineTexture;
            _size.Y = texture.Height * 1.75f;
        }

        /// <summary>
        /// Create default horizontal line.
        /// </summary>
        public HorizontalLine() : this(Anchor.Auto)
        {
        }

        /// <summary>
        /// Draw the element.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw on.</param>
        /// <param name="phase">The phase we are currently drawing.</param>
        override protected void DrawElement(SpriteBatch spriteBatch, DrawPhase phase)
        {
            // get line texture
            Texture2D texture = Resources.HorizontalLineTexture;

            // draw panel
            UserInterface.Active.DrawUtils.DrawSurface(spriteBatch, texture, _destRect, FRAME_WIDTH, 1, FillColor);

            // call base draw function
            base.DrawElement(spriteBatch, phase);
        }
    }
}
