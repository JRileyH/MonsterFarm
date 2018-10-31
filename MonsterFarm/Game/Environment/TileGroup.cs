using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Utils.Tiled;

namespace MonsterFarm.Game.Environment
{
    public class ButtHole{

    }
    public class TileGroup
    {
        private TmxMap _map;
        private Texture2D[] _tilesets;
        private string _root;
        private string _contentRoot;
        private bool _initialized = false;

        public TileGroup()
        {
            _contentRoot = @"Environment/MapTextures/";
            _root = @"Content/Environment/MapLibrary/";
            _map = new TmxMap(_root + "t2-l2-r2-b2.tmx");
            _tilesets = new Texture2D[_map.Tilesets.Count];
        }

        public TileGroup LoadContent(ContentManager content){
            for (int i = 0; i < _map.Tilesets.Count; i++){
                _tilesets[i] = content.Load<Texture2D>(_contentRoot + _map.Tilesets[i].Name);
            }
            _initialized = true;
            return this;
        }

        public int Width { get { return _map.Width; } }
        public int Height { get { return _map.Height; } }
        public int TileWidth { get { return _map.TileWidth; } }
        public int TileHeight { get { return _map.TileHeight; } }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before using TileGroup");
            //Do Updates
        }

        public void Render(SpriteBatch spriteBatch){
            if (!_initialized) throw new Exception("Must call LoadContent before using TileGroup");
            //Do Drawing
            for (var i = 0; i < _map.Layers[0].Tiles.Count; i++)
            {
                int gid = _map.Layers[0].Tiles[i].Gid;

                // Empty tile, do nothing
                if (gid == 0) break;
                int tileFrame = gid - 1;
                int column = tileFrame % 15;
                int row = (int)Math.Floor((double)tileFrame / (double)15);

                float x = (i % Width) * TileWidth;
                float y = (float)Math.Floor(i / (double)Width) * TileHeight;

                Rectangle tilesetRec = new Rectangle(TileWidth * column, TileHeight * row, TileWidth, TileHeight);

                spriteBatch.Draw(_tilesets[0], new Rectangle((int)x, (int)y, TileWidth, TileHeight), tilesetRec, Color.White);

            }
        }
    }
}
