using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Desktop;
using MonsterFarm.Utils.Tiled;
using static MonsterFarm.Utils.Tiled.Layer;

namespace MonsterFarm.Game.Environment
{
    public class ProceduralMap : Map
    {
        private int _mapSize = 15;
        private List<Vector2> _bluePrint;
        private Background _background;
        private Texture2D _warpTexture;
        private Vector2 _warpPosition;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;

        public ProceduralMap(string name) : base(name)
        {
            _background = new Background("WaterTile");
        }

        public Vector2 Warp { get; private set; }

        private List<Vector2> _available(Vector2 v)
        {
            List<Vector2> options = new List<Vector2>();
            Vector2[] _options = {
                new Vector2(v.X, v.Y+1),
                new Vector2(v.X, v.Y-1),
                new Vector2(v.X+1, v.Y),
                new Vector2(v.X-1, v.Y)
            };
            foreach (Vector2 option in _options)
            {
                if (!_bluePrint.Contains(option))
                {
                    options.Add(option);
                }
            }
            return options;
        }

        private List<Vector2> _limit(int limit)
        {
            List<Vector2> options = new List<Vector2>();
            foreach (Vector2 option in _bluePrint)
            {
                if (4 - _available(option).Count < limit)
                {
                    options.Add(option);
                }
            }
            if (options.Count == 0) return _bluePrint;
            return options;
        }

        private void _build(int numberOfRooms)
        {
            _bluePrint = new List<Vector2>();
            _bluePrint.Add(new Vector2(0, 0));
            _bluePrint.Add(new Vector2(1, 0));
            _bluePrint.Add(new Vector2(-1, 0));
            _bluePrint.Add(new Vector2(0, 1));
            _bluePrint.Add(new Vector2(0, -1));
            for (int i = 1; i <= numberOfRooms; i++)
            {
                bool stillLooking = true;
                while (stillLooking)
                {
                    int lerp = i > numberOfRooms / 2 ? 3 : 2;
                    List<Vector2> limit = _limit(lerp);
                    Vector2 sample = limit[Global.rnd.Next(limit.Count)];
                    List<Vector2> options = _available(sample);
                    options = options.OrderBy(o => Global.rnd.Next()).ToList();
                    if (options.Count > 0)
                    {
                        foreach (Vector2 option in options)
                        {
                            if (option.X > -_mapSize && option.X < _mapSize && option.Y > -_mapSize && option.Y < _mapSize)
                            {
                                _bluePrint.Add(options[0]);
                                stillLooking = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Rebuild() {
            _tiles = new List<TileNode>();
            _overlay = new List<TileNode>();
            _build((int)Math.Floor((double)_mapSize / 2));
            string[,] tileGroups = new string[_mapSize * 2, _mapSize * 2];
            foreach (Vector2 v in _bluePrint)
            {
                tileGroups[(int)v.X + _mapSize, (int)v.Y + _mapSize] = "";
            }
            List<Vector2> walkablePoints = new List<Vector2>();
            for (int y = 0; y < tileGroups.GetLength(1); y++)
            {
                for (int x = 0; x < tileGroups.GetLength(0); x++)
                {
                    if (tileGroups[x, y] != null)
                    {
                        //TODO: the order of this matters because its stupid. Alphebetize at some point
                        List<string> connections = new List<string>();
                        if (y < tileGroups.GetLength(1) - 1 && tileGroups[x, y + 1] != null) connections.Add("b2");
                        if (x > 0 && tileGroups[x - 1, y] != null) connections.Add("l2");
                        if (x < tileGroups.GetLength(0) && tileGroups[x + 1, y] != null) connections.Add("r2");
                        if (y > 0 && tileGroups[x, y - 1] != null) connections.Add("t2");
                        string s = string.Join("-", connections.ToArray());
                        s = s.Length > 0 ? s : "b2-l2-r2-t2";
                        TmxMap map = new TmxMap(_root + "v1/" + s + ".tmx");
                        foreach (TmxTileset tileset in map.Tilesets)
                        {
                            tileset.LoadContent(_content);//todo: figure out if this is loading content redudently
                        }
                        foreach (TmxLayer layer in map.Layers)
                        {
                            if (layer.Name == "Walkable")
                            {
                                foreach (TmxLayerTile tile in layer.Tiles)
                                {
                                    Vector2 p = new Vector2((x * 27) + tile.X, (y * 27) + tile.Y);
                                    bool walkable = tile.Gid != 0;
                                    WalkableMap[(int)p.X, (int)p.Y] = walkable;
                                    if (walkable) walkablePoints.Add(p - GlobalTileModifier);
                                }
                            }
                            else
                            {
                                foreach (TmxLayerTile tile in layer.Tiles)
                                {
                                    int gid = tile.Gid;
                                    if (gid == 0) continue;
                                    int tileSetIndex = 0;
                                    while (tileSetIndex <= map.Tilesets.Count)
                                    {
                                        if (tileSetIndex == map.Tilesets.Count) break;
                                        TmxTileset tileset = map.Tilesets[tileSetIndex++];
                                        if (gid < tileset.FirstGid + tileset.TileCount)
                                        {
                                            Vector2 shift = new Vector2((x - _mapSize) * 27, (y - _mapSize) * 27);
                                            if (layer.Name == "Overlay")
                                            {
                                                _overlay.Add(new TileNode(tileset, new Vector2(tile.X + shift.X, tile.Y + shift.Y), gid));
                                            }
                                            else
                                            {
                                                _tiles.Add(new TileNode(tileset, new Vector2(tile.X + shift.X, tile.Y + shift.Y), gid));
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Find start and warp spot
            Start = Warp = walkablePoints[Global.rnd.Next(walkablePoints.Count)];
            while (Warp == Start)
            {
                Warp = walkablePoints[Global.rnd.Next(walkablePoints.Count)];
            }
            Offset = Start * new Vector2(-32, -32) + new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);
            _warpPosition = new Vector2(32, 32) * (Warp + Offset);
            walkablePoints = null;
            tileGroups = null;
            Debug.WriteLine("Start: " + Start + " Warp: " + Warp);
        }

        public override Map LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            if (_initialized) throw new Exception("Cannot call LoadContent twice");
            _content = content;
            _graphicsDevice = graphicsDevice;
            _background.LoadContent(content, graphicsDevice);
            _warpTexture = content.Load<Texture2D>(@"Environment/MapTextures/LavaTile");

            //Set public gettable values
            GlobalTileModifier = new Vector2(_mapSize * 27, _mapSize * 27);
            TileWidth = 32;//todo: dont you hard code, you filth
            TileHeight = 32;
            XCount = 27 * 2 * _mapSize;
            YCount = 27 * 2 * _mapSize;
            Width = XCount * TileWidth;
            Height = YCount * TileHeight;
            WalkableMap = new bool[XCount, YCount];

            Rebuild();

            _initialized = true;
            return this;
        }

        public override void Shift(Vector2 _amt)
        {
            base.Shift(_amt);
            _background.Shift(_amt);
        }

        public override void Update(GameTime gameTime)
        {
            _background.Update(gameTime);
            base.Update(gameTime);
            _warpPosition = (new Vector2(32, 32) * Warp) + Offset;
        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            _background.Render(spriteBatch, viewport);
            base.Render(spriteBatch, viewport);
            spriteBatch.Draw(_warpTexture, _warpPosition, Color.White);
        }
    }
}
