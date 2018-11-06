using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Utils.Tiled;
using static MonsterFarm.Utils.Tiled.Core;
using static MonsterFarm.Utils.Tiled.Layer;
using static MonsterFarm.Utils.Tiled.TmxTileset;

namespace MonsterFarm.Game.Environment
{
    class TileNode
    {
        public TileNode(TmxTileset tileset, Vector2 position, int value)
        {
            Texture = tileset.Texture;
            value -= tileset.FirstGid;
            Position = new Vector2(position.X * tileset.TileWidth, position.Y * tileset.TileHeight);
            Crop = new Rectangle((int)(value % tileset.Columns * tileset.TileWidth), (int)(value / tileset.Columns) * tileset.TileHeight, tileset.TileWidth, tileset.TileHeight);
        }
        public Texture2D Texture { get; }
        public Vector2 Position { get; }
        public Rectangle Crop { get; }
        public Color Color { get { return Color.White; } }
    }

    public class TileGroup
    {
        private TmxMap _map;
        private string _root;
        private bool _initialized = false;
        private Vector2 _offset;
        private List<TileNode> _tiles;

        public TileGroup(Vector2 position)
        {
            _root = @"Content/Environment/MapLibrary/";
            _map = new TmxMap(_root + "test-v1.tmx");
            _tiles = new List<TileNode>();
            Width = _map.Width * _map.TileWidth;
            Height = _map.Height * _map.TileHeight;
            _offset = position * new Vector2(Width, Height);
            X = (int)position.X;
            Y = (int)position.Y;
            Connectors = new string[] { };
        }

        public TileGroup LoadContent(ContentManager content){
            foreach(TmxTileset tileset in _map.Tilesets){
                tileset.LoadContent(content);
            }
            foreach(TmxLayer layer in _map.Layers){
                foreach (TmxLayerTile tile in layer.Tiles){
                    int gid = tile.Gid;
                    if (gid == 0) continue;
                    int tileSetIndex = 0;
                    while(tileSetIndex <= _map.Tilesets.Count){
                        if (tileSetIndex == _map.Tilesets.Count) break;
                        TmxTileset tileset = _map.Tilesets[tileSetIndex++];
                        if (gid < tileset.FirstGid + tileset.TileCount){
                            _tiles.Add(new TileNode(tileset, new Vector2(tile.X, tile.Y), gid));
                            break;
                        }
                    }
                }
            }
            _initialized = true;
            return this;
        }

        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public int XCount { get { return _map.Width; } }
        public int YCount { get { return _map.Height; } }
        public int TileWidth { get { return _map.TileWidth; } }
        public int TileHeight { get { return _map.TileHeight; } }
        public string[] Connectors { get; }

        public void Shift(Vector2 _amt){
            _offset -= _amt;
        }
        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before using TileGroup");

        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (!_initialized) throw new Exception("Must call LoadContent before using TileGroup");
            if (_offset.X + Width > 0 && _offset.X < viewport.Width && _offset.Y + Height > 0 && _offset.Y < viewport.Height){
                foreach (TileNode tile in _tiles) {
                    Vector2 rPos = tile.Position + _offset;
                    if (rPos.X + TileWidth > 0 && rPos.X < viewport.Width && rPos.Y + TileHeight > 0 && rPos.Y < viewport.Height){
                        spriteBatch.Draw(tile.Texture, rPos, tile.Crop, tile.Color);
                    }
                }
            }
        }
    }
}
