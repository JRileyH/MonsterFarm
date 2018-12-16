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
        private string _root;
        private string _textureName;
        private Vector2 _offset;
        private Texture2D _tileTexture;

        public Background(string textureName)
        {
            _root = @"Environment/MapTextures/";
            _textureName = textureName;
        }

        public Background LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _tileTexture = content.Load<Texture2D>(_root+_textureName);
            TileWidth = _tileTexture.Width;
            TileHeight = _tileTexture.Height;
            _initialized = true;
            return this;
        }

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public void Shift(int x, int y) {
            _offset -= new Vector2(x, y);
            if (_offset.X >= TileWidth) _offset.X -= TileWidth;
            if (_offset.X <= TileWidth) _offset.X += TileWidth;
            if (_offset.Y >= TileHeight) _offset.Y -= TileHeight;
            if (_offset.Y <= TileHeight) _offset.Y += TileHeight;
        }
        public void Shift(Point _amt) { Shift(_amt.X, _amt.Y); }
        public void Shift(Vector2 _amt) { Shift((int)_amt.X, (int)_amt.Y); }

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
                    spriteBatch.Draw(_tileTexture, new Rectangle((x * TileWidth)+(int)_offset.X, (y * TileHeight)+(int)_offset.Y, TileWidth, TileHeight), Color.White);
                    y++;
                }
                x++;
            }
        }
    }
}
