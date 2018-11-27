using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Environment;
using MonsterFarm.Game.States;
using MonsterFarm.Game.Util;
using static MonsterFarm.Game.Util.KeyboardHandler;

namespace MonsterFarm.Game.Entites
{
    public class Pawn
    {
        private bool _initialized = false;
        private Map _map;
        private Point _offset;
        private Point? _destination;
        private Point _tileDim;
        private int _speed = 2;
        private Texture2D _placeholder;
        private Queue<Point> _path;

        public Pawn(Map map){
            _map = map;
            Position = new Point(11, 3);
            _destination = null;
            _tileDim = new Point(_map.TileWidth, _map.TileHeight);
            _offset = new Point();
            _path = new Queue<Point>();
            Walking = false;
        }

        public Pawn LoadContent(ContentManager content, GraphicsDevice graphicsDevice){
            if (_initialized) throw new Exception("Cannot call LoadContent twice");
            _placeholder = content.Load<Texture2D>(@"Environment/MapTextures/WaterTile");
            _initialized = true;
            return this;
        }

        public Animation Animation { get; set; }
        public Point Position { get; private set; }
        public bool Walking { get; private set; }
        public string Direction {
            get { return Animation.Sequence;  }
            set {
                if(Animation != null) {
                    Animation.Sequence = value;
                } 
            }
        }

        public bool CanWalkTo(int x, int y){
            return x > 0 &&
            y > 0 &&
            x < _map.WalkableMap.GetLength(0) &&
            y < _map.WalkableMap.GetLength(1) &&
            _map.WalkableMap[x, y];
        }
        public bool CanWalkTo(Point destination){
            return CanWalkTo(destination.X, destination.Y);
        }

        public void AddPath(Point destination){
            if(CanWalkTo(destination)){
                //todo: find path and queue them all
                _path.Enqueue(destination);
                Walking = true;
            }
        }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before Update");
            if (_destination != null){
                Point dim = new Point(_map.TileWidth, _map.TileHeight);
                Point delta = ((Point)_destination * dim) - ((Position * dim) + _offset);
                if (Math.Abs(delta.X) < _speed && Math.Abs(delta.Y) < _speed){
                    Position = (Point)_destination;
                    _offset = new Point();
                    _destination = null;
                } else {
                    _offset += ((Point)_destination - Position) * new Point(_speed, _speed);
                }
            } else if (_path.Count > 0) {
                Walking = true;
                _destination = _path.Dequeue();
                if (Animation != null) Animation.Start();
            } else {
                Walking = false;
                if (Animation != null) Animation.Stop();
            }
            if (Animation != null) Animation.Update(gameTime);
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport){
            if (!_initialized) throw new Exception("Must call LoadContent before Render");
            Vector2 rPos = new Vector2(
                _map.Offset.X + (_map.TileWidth * Position.X) + _offset.X,
                _map.Offset.Y + (_map.TileHeight * Position.Y) + _offset.Y - 16
            );
            if(Animation != null){
                Animation.Render(rPos, spriteBatch);
            } else {
                spriteBatch.Draw(_placeholder, new Rectangle((int)rPos.X, (int)rPos.Y, 32, 48), Color.White);
            }
        }
    }
}
