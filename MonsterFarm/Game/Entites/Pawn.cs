using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Desktop;
using MonsterFarm.Game.Environment;
using MonsterFarm.Game.States;
using MonsterFarm.Game.Util;
using static MonsterFarm.Game.Util.KeyboardHandler;

namespace MonsterFarm.Game.Entites
{
    public class Pawn
    {
        private SpriteFont testfont;
        private bool _initialized = false;
        private Point _offset;
        private Point? _destination;
        private Point _tileDim;
        private int _speed = 3;
        private Texture2D _placeholder;
        private Queue<Point> _path;

        public Pawn(Map map){
            Map = map;
            Position = new Point(9, 9);
            _destination = null;
            _tileDim = new Point(Map.TileWidth, Map.TileHeight);
            _offset = new Point();
            _path = new Queue<Point>();
            Walking = false;
        }

        public Pawn LoadContent(ContentManager content, GraphicsDevice graphicsDevice){
            if (_initialized) throw new Exception("Cannot call LoadContent twice");
            testfont = content.Load<SpriteFont>("UI/fonts/Regular");
            _placeholder = content.Load<Texture2D>(@"Environment/MapTextures/WaterTile");
            _initialized = true;
            return this;
        }

        public Map Map { get; set; }
        public Animation Animation { get; set; }
        public Point Position { get; set; }
        public bool Walking { get; private set; }
        public void Stop(){
            Walking = false;
            if (Animation != null) Animation.Stop();
            _path.Clear();
        }
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
            x < Map.WalkableMap.GetLength(0) &&
            y < Map.WalkableMap.GetLength(1) &&
            Map.WalkableMap[x, y];
        }
        public bool CanWalkTo(Point destination){
            return CanWalkTo(destination.X, destination.Y);
        }

        public void AddPath(Point destination){
            Point temp = new Point(15 * 27, 15 * 27);//todo: ok this bad
            if(CanWalkTo(destination + temp)){
                List<Point> searchPath = PathFinding.BFS(Position + temp, destination + temp, Map.WalkableMap);
                foreach(Point point in searchPath){
                    _path.Enqueue(point - temp);
                }
                Walking = true;
            }
        }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before Update");
            if (_destination != null){
                //Pawn has somewhere to go
                Point dim = new Point(Map.TileWidth, Map.TileHeight);
                Point delta = ((Point)_destination * dim) - ((Position * dim) + _offset);
                if (Math.Abs(delta.X) < _speed && Math.Abs(delta.Y) < _speed){
                    Map.Shift(delta);
                    Position = (Point)_destination;
                    _offset = new Point();
                    _destination = null;
                    foreach(KeyValuePair<string, Transition> transition in Map.Transitions){
                        if(Position==transition.Value.Entry){
                            WorldState.transition(this, transition.Key, transition.Value.Exit);
                        }
                    }
                } else {
                    _offset += ((Point)_destination - Position) * new Point(_speed, _speed);
                }
            } else if (_path.Count > 0) {
                Walking = true;
                _destination = _path.Dequeue();
                Point p = (Point)_destination - Position;
                if (p.X < 0) Direction = "left";
                if (p.X > 0) Direction = "right";
                if (p.Y < 0) Direction = "up";
                if (p.Y > 0) Direction = "down";
                if (Animation != null) Animation.Start();
                Map.Scroll(p * new Point(-_speed, -_speed));
            } else {
                Walking = false;
                if (Animation != null) Animation.Stop();
                Map.Scroll(new Point());
            }
            if (Animation != null) Animation.Update(gameTime);
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport){
            if (!_initialized) throw new Exception("Must call LoadContent before Render");
            spriteBatch.DrawString(testfont, Position.ToString(), new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(testfont, Map.Offset.ToString(), new Vector2(20, 40), Color.Red);
            Vector2 rPos = new Vector2(
                Map.Offset.X + (Map.TileWidth * Position.X) + _offset.X,
                Map.Offset.Y + (Map.TileHeight * Position.Y) + _offset.Y - 16
            );
            if(Animation != null){
                Animation.Render(rPos, spriteBatch);
            } else {
                spriteBatch.Draw(_placeholder, new Rectangle((int)rPos.X, (int)rPos.Y, 32, 48), Color.White);
            }
        }
    }
}
