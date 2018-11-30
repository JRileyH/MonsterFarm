using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Game.Util;
using MonsterFarm.Utils.Tiled;
using static MonsterFarm.Utils.Tiled.Layer;

namespace MonsterFarm.Game.Environment
{
    public class TileNode
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

    public struct Transition
    {
        public Transition(Point entry, Point exit)
        {
            Entry = entry;
            Exit = exit;
        }
        public Point Entry { get; private set; }
        public Point Exit { get; private set; }
    };

    public abstract class Map
    {
        protected bool _initialized = false;
        protected string _root = @"Content/Environment/MapLibrary/";
        protected string _name;
        protected Vector2 _scroll;

        protected List<TileNode> _tiles;
        protected List<TileNode> _overlay;

        public Map(string name)
        {
            Offset = new Vector2(200, 60);
            _scroll = new Vector2(0, 0);
            _name = name;
            Transitions = new Dictionary<string, Transition>();
            if (name=="street"){
                Transitions["tavern"] = new Transition(new Point(14, 5), new Point(8, 17));
            }
            if(name=="tavern"){
                Transitions["street"] = new Transition(new Point(8, 17), new Point(14, 5));
            }
        }

        public int XCount { get; protected set; }
        public int YCount { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int TileWidth { get; protected set; }
        public int TileHeight { get; protected set; }
        public Vector2 Offset { get; set; }
        public bool[,] WalkableMap { get; protected set; }
        public Dictionary<string, Transition> Transitions { get; protected set; }

        public void Shift(int x, int y)
        {
            Offset += new Vector2(x, y);

        }
        public void Shift(Point p) { Shift(p.X, p.Y); }
        public void Shift(Vector2 v) { Shift((int)v.X, (int)v.Y); }

        public void Scroll(int x, int y)
        {
            _scroll.X = x;
            _scroll.Y = y;
        }
        public void Scroll(Point p) { Scroll(p.X, p.Y); }
        public void Scroll(Vector2 v) { Scroll((int)v.X, (int)v.Y); }

        public virtual Map LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            if (_initialized) throw new Exception("Cannot call LoadContent twice");
            TmxMap map = new TmxMap(_root+_name+".tmx");

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
            //if (Offset.X + Width > 0 && Offset.X < viewport.Width && Offset.Y + Height > 0 && Offset.Y < viewport.Height){
                foreach (TileNode tile in _tiles){
                    Vector2 rPos = tile.Position + Offset;
                    if (rPos.X + TileWidth > 0 && rPos.X < viewport.Width && rPos.Y + TileHeight > 0 && rPos.Y < viewport.Height){
                        spriteBatch.Draw(tile.Texture, rPos, tile.Crop, tile.Color);
                    }
                }
            //}
        }

        public virtual void RenderOverlay(SpriteBatch spriteBatch, Viewport viewport){
            if (!_initialized) throw new Exception("Must call LoadContent before Render");
            //if (Offset.X + Width > 0 && Offset.X < viewport.Width && Offset.Y + Height > 0 && Offset.Y < viewport.Height){
                foreach (TileNode tile in _overlay){
                    Vector2 rPos = tile.Position + Offset;
                    if (rPos.X + TileWidth > 0 && rPos.X < viewport.Width && rPos.Y + TileHeight > 0 && rPos.Y < viewport.Height){
                        spriteBatch.Draw(tile.Texture, rPos, tile.Crop, tile.Color);
                    }
                }
            //}
        }
    }
}
