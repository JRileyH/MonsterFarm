using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;

namespace MonsterFarm.Game.Environment
{
    public class ProceduralMap
    {
        Random rnd = new Random();
        private bool _initialized = false;
        private string _start;
        private List<TileGroup> _tileGroups;
        private Vector2 _scroll;
        private int _size;
        private Background _background;

        private Dictionary<string, string[]> _connectors = new Dictionary<string, string[]>{
            {"t2", new string[]{
                    "t2-b2-v1",
                    "t2-l2-b2-v1",
                    "t2-l2-r2-b2-v1",
                    "t2-l2-r2-v1",
                    "t2-l2-v1",
                    "t2-r2-b2-v1",
                    "t2-r2-v1",
                    "t2-v1"
                }
            },
            {"l2", new string[]{
                    "l2-b2-v1",
                    "l2-r2-b2-v1",
                    "l2-r2-v1",
                    "l2-v1",
                    "t2-l2-b2-v1",
                    "t2-l2-r2-b2-v1",
                    "t2-l2-r2-v1",
                    "t2-l2-v1"
                }
            },
            {"r2", new string[]{
                    "l2-r2-b2-v1",
                    "l2-r2-v1",
                    "r2-b2-v1",
                    "r2-v1",
                    "t2-l2-r2-b2-v1",
                    "t2-l2-r2-v1",
                    "t2-r2-b2-v1",
                    "t2-r2-v1"
                }
            },
            {"b2", new string[]{
                    "b2-v1",
                    "l2-b2-v1",
                    "l2-r2-b2-v1",
                    "r2-b2-v1",
                    "t2-b2-v1",
                    "t2-l2-b2-v1",
                    "t2-l2-r2-b2-v1",
                    "t2-r2-b2-v1"
                }
            },
        };

        public ProceduralMap(string start, int size)
        {
            _start = start;
            _scroll = new Vector2(0, 0);
            _background = new Background();
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
                        string[] options = _connectors[_findConnector(connector)];
                        _build(options[rnd.Next(options.Length)], connector, newOffset, depth + 1);
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
