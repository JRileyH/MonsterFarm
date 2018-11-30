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

        public ProceduralMap(string name) : base(name)
        {
            _bluePrint = new List<Vector2>();
            _build((int)Math.Floor((double)_mapSize / 2));
            _background = new Background("WaterTile");
        }

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

        public override Map LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            if (_initialized) throw new Exception("Cannot call LoadContent twice");
            _background.LoadContent(content, graphicsDevice);
            string[,] tileGroups = new string[_mapSize * 2, _mapSize * 2];
            _tiles = new List<TileNode>();
            _overlay = new List<TileNode>();

            //Set public gettable values
            TileWidth = 32;//todo: dont you hard code, you filth
            TileHeight = 32;
            XCount = 27 * 2 * _mapSize;
            YCount = 27 * 2 * _mapSize;
            Width = XCount * TileWidth;
            Height = YCount * TileHeight;
            WalkableMap = new bool[XCount, YCount];

            foreach (Vector2 v in _bluePrint){
                tileGroups[(int)v.X + _mapSize, (int)v.Y + _mapSize] = "";
            }
            for(int y = 0; y < tileGroups.GetLength(1); y++) {
                for (int x = 0; x < tileGroups.GetLength(0); x++) {
                    if(tileGroups[x, y] != null) {
                        //TODO: the order of this matters because its stupid. Alphebetize at some point
                        List<string> connections = new List<string>();
                        if (y < tileGroups.GetLength(1)-1 && tileGroups[x, y + 1] != null) connections.Add("b2");
                        if (x > 0 && tileGroups[x - 1, y] != null) connections.Add("l2");
                        if (x < tileGroups.GetLength(0) && tileGroups[x + 1, y] != null) connections.Add("r2");
                        if (y > 0 && tileGroups[x, y - 1] != null) connections.Add("t2");
                        string s = string.Join("-", connections.ToArray());
                        s = s.Length > 0 ? s : "b2-l2-r2-t2";
                        TmxMap map = new TmxMap(_root + "v1/" + s + ".tmx");
                        foreach (TmxTileset tileset in map.Tilesets){
                            tileset.LoadContent(content);//todo: figure out if this is loading content redudently
                        }
                        foreach (TmxLayer layer in map.Layers){
                            if (layer.Name == "Walkable"){
                                foreach (TmxLayerTile tile in layer.Tiles){
                                    WalkableMap[(x*27) + tile.X, (y*27) + tile.Y] = tile.Gid != 0;
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
                                            Point shift = new Point((x - _mapSize) * 27, (y - _mapSize) * 27);
                                            if (layer.Name == "Overlay"){
                                                _overlay.Add(new TileNode(tileset, new Vector2(tile.X + shift.X, tile.Y + shift.Y), gid));
                                            } else {
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
            _initialized = true;
            return this;
        }

        public override void Update(GameTime gameTime)
        {
            Offset += _scroll;
            _background.Shift(-_scroll);
            _background.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            _background.Render(spriteBatch, viewport);
            base.Render(spriteBatch, viewport);
        }
    }
}
