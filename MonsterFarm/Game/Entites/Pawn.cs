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
        private Vector2 _offset;
        private Vector2? _destination;
        private Vector2 _tileDim;
        private int _speed = 3;
        private Texture2D _placeholder;
        private Queue<Vector2> _path;

        public Pawn(Map map){
            Map = map;
            Position = new Vector2(9, 9);
            _destination = null;
            _tileDim = new Vector2(Map.TileWidth, Map.TileHeight);
            _offset = new Vector2();
            _path = new Queue<Vector2>();
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
        public Vector2 Position { get; set; }
        public Vector2 RenderPosition { get; private set; }
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
        public bool CanWalkTo(Vector2 destination)
        {
            return CanWalkTo((int)destination.X, (int)destination.Y);
        }

        public void AddPath(Vector2 destination){
            Vector2 temp = new Vector2(15 * 27, 15 * 27);//todo: ok this bad
            if(CanWalkTo(destination + temp)){
                List<Vector2> searchPath = PathFinding.BFS(Position + temp, destination + temp, Map.WalkableMap);
                foreach(Vector2 point in searchPath){
                    _path.Enqueue(point - temp);
                }
                Walking = true;
            }
        }

        public void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before Update");
            if (_destination != null){
                //Pawn has somewhere to go
                Vector2 dim = new Vector2(Map.TileWidth, Map.TileHeight);
                Vector2 delta = ((Vector2)_destination * dim) - ((Position * dim) + _offset);
                if (Math.Abs(delta.X) < _speed && Math.Abs(delta.Y) < _speed){
                    //Pawn is close enough to desitination
                    Map.Shift(delta);
                    Position = (Vector2)_destination;
                    _offset = new Vector2();
                    _destination = null;
                    foreach(KeyValuePair<string, Transition> transition in Map.Transitions){
                        if(Position==transition.Value.Entry){
                            WorldState.transition(this, transition.Key, transition.Value.Exit);
                        }
                    }
                } else {
                    //Move Pawn closer to destination
                    Vector2 shift = ((Vector2)_destination - Position) * new Vector2(_speed, _speed);
                    _offset += shift;
                    Map.Shift(-shift);

                }
            } else if (_path.Count > 0) {
                Walking = true;
                _destination = _path.Dequeue();
                Vector2 p = (Vector2)_destination - Position;
                if (p.X < 0) Direction = "left";
                if (p.X > 0) Direction = "right";
                if (p.Y < 0) Direction = "up";
                if (p.Y > 0) Direction = "down";
                if (Animation != null) Animation.Start();
            } else {
                Walking = false;
                if (Animation != null) Animation.Stop();
            }
            if (Animation != null) Animation.Update(gameTime);
            RenderPosition = new Vector2(
                Map.Offset.X + (Position.X * Map.TileWidth) + _offset.X,
                Map.Offset.Y + (Position.Y * Map.TileWidth) + _offset.Y - 16
            );
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport){
            if (!_initialized) throw new Exception("Must call LoadContent before Render");
            spriteBatch.DrawString(testfont, Position.ToString(), new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(testfont, Map.Offset.ToString(), new Vector2(20, 40), Color.Red);
            if(Animation != null){
                Animation.Render(RenderPosition, spriteBatch);
            } else {
                spriteBatch.Draw(_placeholder, new Rectangle((int)RenderPosition.X, (int)RenderPosition.Y, 32, 48), Color.White);
            }
        }
    }
}
