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

namespace MonsterFarm.Game.Environment
{
    public class ProceduralMap
    {
        Random rnd = new Random();
        private bool _initialized = false;
        private string _root;
        private string _start;
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

        public ProceduralMap(string start, int size)
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

            _start = start;
            _scroll = new Vector2(0, 0);
            _background = new Background("WaterTile");
            _tileGroups = new Coordinate<TileGroup>();
            _size = size;
            _build(start, "start", new Vector2(0, 0), 0, new string[]{});
        }

        private string _randomOption(string connector, string[] offlimits = null, bool cap = false){
            offlimits = offlimits ?? new string[0];
            List<string> completeList = _connectors[connector];
            List<string> trimmedList = new List<string>(completeList);
            foreach(string option in completeList){
                if (cap && option.Length > 7) {
                    trimmedList.Remove(option);
                } else {
                    foreach (string offlimit in offlimits) {
                        if (option == offlimit) {
                            trimmedList.Remove(option);
                        }
                    }
                }
            }
            return trimmedList[rnd.Next(trimmedList.Count)];
        }

        private void _build(string id, string from, Vector2 position, int depth, string[] exlude){
            TileGroup tileGroup = new TileGroup(id, position);
            _tileGroups.Add(position.X, position.Y, tileGroup);
            Debug.WriteLine("- " + position.X + ", " + position.Y + ": " + id);
            foreach (string connector in tileGroup.Connectors){
                Vector2 updatePosition = position + __[connector];
                if (connector != from){
                    if (_tileGroups.Get(updatePosition.X, updatePosition.Y) != null)
                    {
                        Debug.WriteLine("x " + updatePosition.X + ", " + updatePosition.Y);
                        break;
                    }
                    if (depth >= _size || _tileGroups.Get(updatePosition.X, updatePosition.Y) != null) {
                        _build(_randomOption(connector, cap: true), _[connector], updatePosition, depth + 1, new string[]{});
                    } else {
                        string[] exlusions = new string[1 + exlude.Length];
                        Array.Copy(new string[]{from}, exlusions, 1);
                        Array.Copy(exlude, 0, exlusions, 1, exlude.Length);
                        _build(_randomOption(connector, exlude), _[connector], updatePosition, depth + 1, exlusions);
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
