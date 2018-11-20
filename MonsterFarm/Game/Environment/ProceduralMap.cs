using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Collections;
using MonsterFarm.Utils.DataStructures;
using System.Linq;
using MonsterFarm.Desktop;

namespace MonsterFarm.Game.Environment
{
    public class ProceduralMap
    {
        private bool _initialized = false;
        private int _mapSize = 64;
        private List<Vector2> _bluePrint;
        private TileGroup[,] _tileGroups;
        private Vector2 _scroll;
        private Background _background;

        public ProceduralMap(){
            _scroll = new Vector2(0, 0);
            _background = new Background("WaterTile");
            _tileGroups = new TileGroup[_mapSize*2, _mapSize*2];
            _bluePrint = new List<Vector2>();
            _build(15);
        }

        private List<Vector2> _available(Vector2 v){
            List<Vector2> options = new List<Vector2>();
            Vector2[] _options = {
                new Vector2(v.X, v.Y+1),
                new Vector2(v.X, v.Y-1),
                new Vector2(v.X+1, v.Y),
                new Vector2(v.X-1, v.Y)
            };
            foreach (Vector2 option in _options){
                if (!_bluePrint.Contains(option)){
                    options.Add(option);
                }
            }
            return options;
        }

        private List<Vector2> _limit(int limit){
            List<Vector2> options = new List<Vector2>();
            foreach(Vector2 option in _bluePrint){
                if(4 - _available(option).Count < limit){
                    options.Add(option);
                }
            }
            if (options.Count == 0) return _bluePrint;
            return options;
        }

        private void _build(int numberOfRooms){
            _bluePrint.Add(new Vector2(0, 0));
            _bluePrint.Add(new Vector2(1, 0));
            _bluePrint.Add(new Vector2(-1, 0));
            _bluePrint.Add(new Vector2(0, 1));
            _bluePrint.Add(new Vector2(0, -1));
            for (int i = 1; i <= numberOfRooms; i++){
                bool stillLooking = true;
                while(stillLooking){
                    int lerp = i > numberOfRooms / 2 ? 3 : 2;
                    List<Vector2> limit = _limit(lerp);
                    Vector2 sample = limit[Global.rnd.Next(limit.Count)];
                    List<Vector2> options = _available(sample);
                    options = options.OrderBy(o => Global.rnd.Next()).ToList();
                    if(options.Count > 0){
                        foreach(Vector2 option in options){
                            if(option.X > -_mapSize && option.X < _mapSize && option.Y > -_mapSize && option.Y < _mapSize){
                                _bluePrint.Add(options[0]);
                                stillLooking = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public TileGroup TileGroup(int x, int y){
            return _tileGroups[x + _mapSize, y + _mapSize];
        }

        public void Scroll(int x, int y){
            _scroll.X = x;
            _scroll.Y = y;
        }

        public ProceduralMap LoadContent(ContentManager content, GraphicsDevice graphicsDevice) {
            _background.LoadContent(content, graphicsDevice);
            foreach (Vector2 v in _bluePrint){
                _tileGroups[(int)v.X + _mapSize, (int)v.Y + _mapSize] = new TileGroup(v);
            }
            foreach (TileGroup tileGroup in _tileGroups){
                if (tileGroup != null){
                    List<string> connections = new List<string>();
                    if (_tileGroups[tileGroup.X+_mapSize, tileGroup.Y+1+_mapSize] != null) connections.Add("b2");
                    if (_tileGroups[tileGroup.X - 1 + _mapSize, tileGroup.Y + _mapSize] != null) connections.Add("l2");
                    if (_tileGroups[tileGroup.X + 1 + _mapSize, tileGroup.Y + _mapSize] != null) connections.Add("r2");
                    if (_tileGroups[tileGroup.X + _mapSize, tileGroup.Y - 1 + _mapSize] != null) connections.Add("t2");
                    string s = string.Join("-",connections.ToArray());
                    s = s.Length > 0 ? s : "b2-l2-r2-t2";
                    tileGroup.LoadContent(content, s);
                }
            }
            _initialized = true;
            return this;
        }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before using ProceduralMap");
            _background.Shift(_scroll);
            _background.Update(gameTime);
            foreach (TileGroup tileGroup in _tileGroups){
                if (tileGroup != null) tileGroup.Shift(_scroll);
                if (tileGroup != null) tileGroup.Update(gameTime);
            }
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport) {
            if (!_initialized) throw new Exception("Must call LoadContent before using ProceduralMap");
            _background.Render(spriteBatch, viewport);
            foreach (TileGroup tileGroup in _tileGroups){
                if (tileGroup != null) tileGroup.Render(spriteBatch, viewport);
            }
        }
    }
}
