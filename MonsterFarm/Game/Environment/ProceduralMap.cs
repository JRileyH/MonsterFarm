using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace MonsterFarm.Game.Environment
{
    public class ProceduralMap
    {
        Random rnd = new Random();
        private bool _initialized = false;
        private string _root;
        private string _start;
        private List<TileGroup> _tileGroups;
        private Vector2 _scroll;
        private int _size;
        private Background _background;
        private Dictionary<string, List<string>> _connectors = new Dictionary<string, List<string>>{
            { "t2", new List<string>() },
            { "l2", new List<string>() },
            { "r2", new List<string>() },
            { "b2", new List<string>() }
        };

        public ProceduralMap(string start, int size)
        {
            _root = @"Content/Environment/MapLibrary/";
            string[] allmaps = Directory.GetFiles(@"Content/Environment/MapLibrary/", "*.tmx");
            for (int i = 0; i < allmaps.Length; i++){
                allmaps[i] = allmaps[i].Replace(_root, "").Replace(".tmx", "");
                if(allmaps[i].Contains("t2")){
                    _connectors["t2"].Add(allmaps[i]);
                }
                if (allmaps[i].Contains("l2")){
                    _connectors["l2"].Add(allmaps[i]);
                }
                if (allmaps[i].Contains("r2")){
                    _connectors["r2"].Add(allmaps[i]);
                }
                if (allmaps[i].Contains("b2")){
                    _connectors["b2"].Add(allmaps[i]);
                }
            }

            _start = start;
            _scroll = new Vector2(0, 0);
            _background = new Background("WaterTile");
            _tileGroups = new List<TileGroup>();
            _size = size;
            _build(start, "start", new Vector2(0, 0), 0);
        }

        private string _findConnector(string input){
            switch(input){
                case "t2":
                    return "b2";
                case "l2":
                    return "r2";
                case "r2":
                    return "l2";
                case "b2":
                    return "t2";
            }
            return "err";
        }

        private string _randomOption(string connector, string[] offlimits){
            List<string> optionList = _connectors[_findConnector(connector)];
            foreach(string option in optionList){
                foreach(string offlimit in offlimits){
                    if(option == offlimit){
                        optionList.Remove(option);
                    }
                }
            }
            return optionList[rnd.Next(optionList.Count)];
        }

        private void _build(string id, string from, Vector2 offset, int depth){
            TileGroup tileGroup = new TileGroup(id, offset);
            foreach(string connector in tileGroup.Connectors){
                if(connector != _findConnector(from)){
                    Vector2 newOffset = offset;//todo: copy this object?
                    switch (connector){
                        case "t2":
                            newOffset.Y -= tileGroup.Height;
                            break;
                        case "l2":
                            newOffset.X -= tileGroup.Width;
                            break;
                        case "r2":
                            newOffset.X += tileGroup.Width;
                            break;
                        case "b2":
                            newOffset.Y += tileGroup.Height;
                            break;
                    }
                    if (depth >= _size) {
                        _build(_findConnector(connector)+"-v1", connector, newOffset, depth + 1);
                    } else {

                        _build(_randomOption(connector, new string[]{}), connector, newOffset, depth + 1);
                    }
                }
            }
            _tileGroups.Add(tileGroup);
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
