using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;

namespace MonsterFarm.Game.Environment
{

    /*class TileNode
    {
        private int _value;
        private Vector2 _position;
        private Vector2 _dimensions;
        private Rectangle _crop;

        public TileNode(int value, Vector2 start, Vector2 position, Vector2 dimensions)
        {
            _value = value;
            _position = new Vector2((position.X * dimensions.X) + start.X, (position.Y * dimensions.Y) + start.Y);
            _dimensions = dimensions;
            _crop = new Rectangle((int)(value % 6 * dimensions.X), (int)(Math.Floor((double)value / 6) * dimensions.Y), (int)dimensions.X, (int)dimensions.Y);

        }
        public int Value { get { return _value; } }
        public int X { get { return (int)_position.X; } }
        public int Y { get { return (int)_position.Y; } }
        public Vector2 Position { get { return _position; } }
        public int Width { get { return (int)_dimensions.X; } }
        public int Height { get { return (int)_dimensions.Y; } }
        public Rectangle Crop { get { return _crop; } }
    }*/

    public class Map
    {
        private static ContentManager _content;
        private static string _root;

        private bool _initialized = false;
        //private TileNode[,] _tiles;
        private Vector2 _offset;

        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private readonly int _tileWidth;
        private readonly int _tileHeight;
        private int[,] _tileList;

        public Map(string startingMap)
        {
            _root = @"Content/Environment/MapLibrary/";
            _offset = new Vector2(0, 0);
            XmlDocument mdoc = new XmlDocument();
            mdoc.Load(_root + startingMap);
            XmlNode map = mdoc.SelectNodes("map")[0];
            foreach(XmlAttribute attr in map.Attributes){
                _mapWidth = Convert.ToInt32(map.Attributes.GetNamedItem("width").Value);
                _mapHeight = Convert.ToInt32(map.Attributes.GetNamedItem("height").Value);
                _tileList = new int[_mapWidth,_mapHeight];
                _tileWidth = Convert.ToInt32(map.Attributes.GetNamedItem("tilewidth").Value);
                _tileHeight = Convert.ToInt32(map.Attributes.GetNamedItem("tileheight").Value);
            }
            XmlNodeList tilesets = map.SelectNodes("tileset");
            foreach(XmlNode tileset in tilesets){
                XmlDocument tsdoc = new XmlDocument();
                tsdoc.Load(_root + tileset.Attributes.GetNamedItem("source").Value);
                //TODO: Do I need to do anything here?
            }
            XmlNodeList layers = map.SelectNodes("layer");
            foreach (XmlNode layer in layers){
                string data_csv = layer.SelectNodes("data")[0].InnerText;
                string[] rows = data_csv.Split('\n');
                int y = 0;
                foreach(string row in rows){
                    Debug.WriteLine(row);
                    string[] cols = row.Split(',');
                    int x = 0;
                    foreach(string col in cols){
                        Debug.WriteLine(x + ", " + y+": "+col);
                        if(col != "0") _tileList[x, y] = Convert.ToInt32(col);
                        x++;
                    }
                    y++;
                }
            }

            //_tileDimensions = tileDimensions;
        }

        /*public Map Initialize(TileType[,] tileTypes)
        {
            _initialized = true;
            _dimensions = new Vector2(tileTypes.GetLength(0), tileTypes.GetLength(1));
            _tiles = new TileNode[(int)_dimensions.X, (int)_dimensions.Y];
            for (int x = 0; x < _dimensions.X; x++) {
                for (int y = 0; y < _dimensions.Y; y++) {
                    _tiles[x, y] = new TileNode(tileTypes[x, y], _offset, new Vector2(x, y), new Vector2(_tileDimensions.X, _tileDimensions.Y));
                }
            }
            return this;
        }
        public virtual Map Initialize(MapFloor mapFloor) {
            return this.Initialize(mapFloor.TileMap);
        }*/

        public Map Initialize() {
            _initialized = true;
            return this;
        }

        //public int TileWidth { get { return (int)_tileDimensions.X; } }
        //public int TileHeight { get { return (int)_tileDimensions.Y; } }

        public void LoadContent(ContentManager content) {
            if (!_initialized) throw new Exception("You must call Initialize to use Map");
            //_root = "Environment/MapTextures/";
            _content = content;
        }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("You must call Initialize to use Map");
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport) {
            /*if (!_initialized) throw new Exception("You must call Initialize to use Map");
            for (int x = 0; x < _dimensions.X; x++) {
                if (x * TileWidth > viewport.Width - _offset.X) break;
                for (int y = 0; y < _dimensions.Y; y++) {
                    if (y * TileHeight > viewport.Height - _offset.Y) break;
                    TileNode tile = _tiles[x, y];
                    if (tile.Position.X >= 0 && tile.Position.Y >= 0 && tile.Type != TileType.None) {
                        spriteBatch.Draw(IslandTextureSheet, tile.Position, tile.Crop, Color.White);
                    }
                }
            }*/
        }
    }
}
