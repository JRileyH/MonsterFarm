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
        Random rnd = new Random();
        private bool _initialized = false;
        private string _root;
        private Coordinate<TileGroup> _tileGroups;

        private Vector2 _scroll;
        private int _size;
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
        private Dictionary<string, Vector2> __ = new Dictionary<string, Vector2> {
            {"t2", new Vector2( 0, -1)},
            {"l2", new Vector2(-1,  0)},
            {"r2", new Vector2( 1,  0)},
            {"b2", new Vector2( 0,  1)}
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
            Debug.WriteLine(_tileGroups.ToString());

        }

        private void _build(int numberOfRooms){
            Vector2 checkPos = new Vector2(0, 0);
            _tileGroups.Add(0, 0, new TileGroup(checkPos));
            float randomCompare;
            float rndStart = 0.2f, rndEnd = 0.01f;
            for (int i = 0; i < numberOfRooms; i++){
                float perc = i / ((float)numberOfRooms - 1);
                randomCompare = rndStart + (rndEnd - rndStart) * perc;
                checkPos = _getNewPos();
                Debug.WriteLine("Wrote: "+checkPos);
                _tileGroups.Add(checkPos.X, checkPos.Y, new TileGroup(checkPos));
                Debug.WriteLine(_tileGroups.Get(checkPos.X, checkPos.Y).X+", "+ _tileGroups.Get(checkPos.X, checkPos.Y).Y);
                Debug.WriteLine("-----");
            }
        }
        private Vector2 _getNewPos(){
            TileGroup branchOff = _tileGroups.Get();
            Debug.WriteLine("Found: "+branchOff.X+", "+branchOff.Y);
            Vector2[] options = {
                new Vector2(branchOff.X,branchOff.Y+1),
                new Vector2(branchOff.X,branchOff.Y-1),
                new Vector2(branchOff.X+1,branchOff.Y),
                new Vector2(branchOff.X-1,branchOff.Y)
            };
            options = options.OrderBy(i => rnd.Next()).ToArray();
            foreach(Vector2 option in options){
                Debug.WriteLine("Check: " + option);
                if(_tileGroups.Get(option.X, option.Y) == null) return option;
            }
            return _getNewPos();
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
