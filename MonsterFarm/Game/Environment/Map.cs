using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Game.Util;
using MonsterFarm.Utils.Tiled;
using static MonsterFarm.Utils.Tiled.Layer;

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

    public abstract class Map
    {
        private bool _initialized = false;
        private string _root = @"Content/Environment/MapLibrary/";
        private List<TileNode> _tiles;
        private List<TileNode> _overlay;

        public Map()
        {
            Offset = new Vector2(350, 60);
        }

        public int XCount { get; private set; }
        public int YCount { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public Vector2 Offset { get; private set; }
        public bool[,] WalkableMap { get; private set; }

        public virtual Map LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            if (_initialized) throw new Exception("Cannot call LoadContent twice");
            TmxMap map = new TmxMap(@"Content/Environment/MapLibrary/static/tavern.tmx");

            //Init data structures
            _tiles = new List<TileNode>();
            _overlay = new List<TileNode>();
            WalkableMap = new bool[map.Width,map.Height];

            //Set public gettable values
            XCount = map.Width;
            YCount = map.Height;
            TileWidth = map.TileWidth;
            TileHeight = map.TileHeight;
            Width = map.Width * map.TileWidth;
            Height = map.Height * map.TileHeight;

            foreach (TmxTileset tileset in map.Tilesets){
                tileset.LoadContent(content);
            }
            foreach (TmxLayer layer in map.Layers){
                if (layer.Name == "Walkable"){
                    foreach (TmxLayerTile tile in layer.Tiles){
                        WalkableMap[tile.X, tile.Y] = tile.Gid != 0;
                    }
                } else {
                    foreach (TmxLayerTile tile in layer.Tiles){
                        int gid = tile.Gid;
                        if (gid == 0) continue;
                        int tileSetIndex = 0;
                        while (tileSetIndex <= map.Tilesets.Count){
                            if (tileSetIndex == map.Tilesets.Count) break;
                            TmxTileset tileset = map.Tilesets[tileSetIndex++];
                            if (gid < tileset.FirstGid + tileset.TileCount){
                                if (layer.Name == "Overlay"){
                                    _overlay.Add(new TileNode(tileset, new Vector2(tile.X, tile.Y), gid));
                                } else {
                                    _tiles.Add(new TileNode(tileset, new Vector2(tile.X, tile.Y), gid));
                                }
                                break;
                            }
                        }
                    }
                }
            }
            _initialized = true;
            return this;
        }

        public virtual void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before Update");

        }

        public virtual void Render(SpriteBatch spriteBatch, Viewport viewport){
            if (!_initialized) throw new Exception("Must call LoadContent before Render");
            if (Offset.X + Width > 0 && Offset.X < viewport.Width && Offset.Y + Height > 0 && Offset.Y < viewport.Height){
                foreach (TileNode tile in _tiles){
                    Vector2 rPos = tile.Position + Offset;
                    if (rPos.X + TileWidth > 0 && rPos.X < viewport.Width && rPos.Y + TileHeight > 0 && rPos.Y < viewport.Height){
                        spriteBatch.Draw(tile.Texture, rPos, tile.Crop, tile.Color);
                    }
                }
            }
        }

        public virtual void RenderOverlay(SpriteBatch spriteBatch, Viewport viewport){
            if (!_initialized) throw new Exception("Must call LoadContent before Render");
            if (Offset.X + Width > 0 && Offset.X < viewport.Width && Offset.Y + Height > 0 && Offset.Y < viewport.Height){
                foreach (TileNode tile in _overlay){
                    Vector2 rPos = tile.Position + Offset;
                    if (rPos.X + TileWidth > 0 && rPos.X < viewport.Width && rPos.Y + TileHeight > 0 && rPos.Y < viewport.Height){
                        spriteBatch.Draw(tile.Texture, rPos, tile.Crop, tile.Color);
                    }
                }
            }
        }
    }
}
