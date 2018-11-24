using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Game.Environment;

namespace MonsterFarm.Game.Entites
{
    public class Pawn
    {
        private bool _initialized = false;
        private Map _map;
        private Point _offset;
        private Point? _destination;
        private int _speed = 2;
        private Texture2D _placeholder;
        private Queue<Point> _path;

        public Pawn(Map map){
            _map = map;
            Position = new Point(11, 3);
            _destination = null;
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

        private Point _velocity(){
            Point dim = new Point(_map.TileWidth, _map.TileHeight);
            Point delta = ((Point)_destination * dim) - ((Position * dim) + _offset);
            double rad = Math.Atan2(delta.Y, delta.X);
            return new Point(
                Math.Abs(delta.X) >= _speed ? (int)Math.Round(Math.Cos(rad) * _speed) : 0,
                Math.Abs(delta.Y) >= _speed ? (int)Math.Round(Math.Sin(rad) * _speed) : 0
            );
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
                _path.Enqueue(destination);
                Walking = true;
            }
        }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before Update");
            if (_destination != null){
                Point vel = _velocity();
                if (vel.X == 0 && vel.Y == 0){
                    Position = (Point)_destination;
                    _offset = new Point();
                    _destination = null;
                } else {
                    _offset += vel;
                }
            } else if (_path.Count > 0) {
                Walking = true;
                _destination = _path.Dequeue();
                if (_destination != Position)
                {
                    Point direction = (Point)_destination - Position;
                    if (Animation != null) Animation.Start();
                } else {
                    if (Animation != null) Animation.Reset();
                }
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
