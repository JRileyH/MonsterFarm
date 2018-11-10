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

namespace MonsterFarm.Game.Environment
{
    public class ProceduralMap
    {
        private bool _initialized = false;
        private string _root;
        private Coordinate<TileGroup> _tileGroups;

        private Vector2 _scroll;
        private Background _background;
        private Dictionary<string, List<string>> _connectors = new Dictionary<string, List<string>>{
            { "t2", new List<string>() },
            { "l2", new List<string>() },
            { "r2", new List<string>() },
            { "b2", new List<string>() }
        };
        private Dictionary<string, string> _ = new Dictionary<string, string> {
            {"t2", "b2"},
            {"l2", "r2"},
            {"r2", "l2"},
            {"b2", "t2"}
        };

        public ProceduralMap()
        {
            _root = @"Content/Environment/MapLibrary/";
            string[] allmaps = Directory.GetFiles(@"Content/Environment/MapLibrary/", "*.tmx");
            for (int i = 0; i < allmaps.Length; i++){
                allmaps[i] = allmaps[i].Replace(_root, "").Replace(".tmx", "");
                if(allmaps[i].Contains("t2")){
                    _connectors["b2"].Add(allmaps[i]);
                }
                if (allmaps[i].Contains("l2")){
                    _connectors["r2"].Add(allmaps[i]);
                }
                if (allmaps[i].Contains("r2")){
                    _connectors["l2"].Add(allmaps[i]);
                }
                if (allmaps[i].Contains("b2")){
                    _connectors["t2"].Add(allmaps[i]);
                }
            }

            _scroll = new Vector2(0, 0);
            _background = new Background("WaterTile");
            _tileGroups = new Coordinate<TileGroup>();
            _build(10);

        }

        private void _build(int numberOfRooms){
            _tileGroups.Add(0, 0, new TileGroup(new Vector2(0, 0)));
            for (int i = 0; i <= numberOfRooms; i++)
            {
                bool stillLooking = true;
                while(stillLooking){
                    TileGroup t = _tileGroups.Get();
                    TileGroup[] options = {
                        new TileGroup(t.X+1, t.Y),
                        new TileGroup(t.X-1, t.Y),
                        new TileGroup(t.X, t.Y+1),
                        new TileGroup(t.X, t.Y-1)
                    };
                    options = options.OrderBy(o => new Random().Next()).ToArray();
                    foreach(TileGroup option in options){
                        if(_tileGroups.Get(option.X, option.Y) == null){
                            _tileGroups.Add(option.X, option.Y, option);
                            stillLooking = false;
                            break;
                        }
                    }
                }
            }
        }

        public void Scroll(int x, int y){
            _scroll.X = x;
            _scroll.Y = y;
        }

        public ProceduralMap LoadContent(ContentManager content, GraphicsDevice graphicsDevice) {
            _background.LoadContent(content, graphicsDevice);
            foreach(TileGroup tileGroup in _tileGroups){
                tileGroup.LoadContent(content);
            }
            _initialized = true;
            return this;
        }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before using ProceduralMap");
            _background.Shift(_scroll);
            _background.Update(gameTime);
            foreach (TileGroup tileGroup in _tileGroups){
                tileGroup.Shift(_scroll);
                tileGroup.Update(gameTime);
            }
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport) {
            if (!_initialized) throw new Exception("Must call LoadContent before using ProceduralMap");
            _background.Render(spriteBatch, viewport);
            foreach (TileGroup tileGroup in _tileGroups){
                tileGroup.Render(spriteBatch, viewport);
            }
        }
    }
}
