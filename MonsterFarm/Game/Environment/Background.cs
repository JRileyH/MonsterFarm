using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterFarm.Game.Environment
{
    public class Background
    {
        private bool _initialized = false;
        private Vector2 _offset;
        private Texture2D _tile;
        private int _tileWidth;
        private int _tileHeight;

        public Background()
        {
            _tileWidth = 32;
            _tileHeight = 32;
        }

        public Background LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Texture2D tileSet = content.Load<Texture2D>(@"Environment/MapTextures/ground");
            Rectangle crop = new Rectangle(32, 128, TileWidth, TileHeight);
            _tile = new Texture2D(graphicsDevice, crop.Width, crop.Height);
            Color[] data = new Color[crop.Width * crop.Height];
            tileSet.GetData(0, crop, data, 0, data.Length);
            _tile.SetData(data);

            _initialized = true;
            return this;
        }

        public int TileWidth { get { return _tileWidth; } }
        public int TileHeight { get { return _tileHeight; } }

        public void Shift(Vector2 _amt)
        {
            _offset -= _amt;
            if (_offset.X >= TileWidth) _offset.X -= TileWidth;
            if (_offset.X <= TileWidth) _offset.X += TileWidth;
            if (_offset.Y >= TileHeight) _offset.Y -= TileHeight;
            if (_offset.Y <= TileHeight) _offset.Y += TileHeight;
        }
        public void Update(GameTime gameTime)
        {
            if (!_initialized) throw new Exception("Must call LoadContent before using TileGroup");

        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (!_initialized) throw new Exception("Must call LoadContent before using TileGroup");
            int x = -2;
            int y = -2;
            while(x * TileWidth <= viewport.Width){
                y = -2;
                while (y * TileHeight <= viewport.Height){
                    spriteBatch.Draw(_tile, new Rectangle((x * TileWidth)+(int)_offset.X, (y * TileHeight)+(int)_offset.Y, TileWidth, TileHeight), Color.White);
                    y++;
                }
                x++;
            }
        }
    }
}
