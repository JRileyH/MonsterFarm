using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonsterFarm.UI.Elements
{
    /// <summary>
    /// A special panel used as the root panel that covers the entire screen.
    /// This panel is used internally to serve as the constant root element in the entities tree.
    /// </summary>
    public class RootPanel : Panel
    {
        /// <summary>
        /// Create the root panel.
        /// </summary>
        public RootPanel() :
            base(Vector2.Zero, PanelSkin.None, Anchor.Center, Vector2.Zero)
        {
            Padding = Vector2.Zero;
            ShadowColor = Color.Transparent;
            OutlineWidth = 0;
            ClickThrough = true;
        }

        /// <summary>
        /// Override the function to calculate the destination rectangle, so the root panel will always cover the entire screen.
        /// </summary>
        /// <returns>Rectangle in the size of the whole screen.</returns>
        override public Rectangle CalcDestRect()
        {
            int width = UserInterface.Active.ScreenWidth;
            int height = UserInterface.Active.ScreenHeight;
            return new Rectangle(0, 0, width, height);
        }

        /// <summary>
        /// Update dest rect and internal dest rect, but only if needed (eg if something changed since last update).
        /// </summary>
        override public void UpdateDestinationRectsIfDirty()
        {
            // if dirty, update destination rectangles
            if (IsDirty)
            {
                UpdateDestinationRects();
            }
        }

        /// <summary>
        /// Draw this element and its children.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to use for drawing.</param>
        override public void Draw(SpriteBatch spriteBatch)
        {
            // if not visible skip
            if (!Visible)
            {
                return;
            }

            // do before draw event
            OnBeforeDraw(spriteBatch);

            // calc desination rect
            UpdateDestinationRectsIfDirty();

            // get sorted children list
            List<Element> childrenSorted = GetSortedChildren();

            // draw all children
            foreach (Element child in childrenSorted)
            {
                child.Draw(spriteBatch);
            }

            // do after draw event
            OnAfterDraw(spriteBatch);
        }
    }
}
