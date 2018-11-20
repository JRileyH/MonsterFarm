using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Environment;

namespace MonsterFarm.Game.Entites
{
    public class Player : Entity
    {
        private ProceduralMap _map;
        private TileGroup _tileGroup;
        private Point _currentTile;
        private Point _destinationTile;
        private Point _offset;
        private bool _walking;
        private int _speed = 3;

        public Player(ProceduralMap map)
        {
            _root = @"Entities/";
            _textureName = "player";
            _map = map;
            _currentTile = new Point(10, 9);
            _offset = new Point(0, 0);
            _walking = false;

        }
        public override Entity LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _animation = new Animation(content.Load<Texture2D>(_root + _textureName));
            _animation.AddFrames("down", 0, 8, 32, 48, TimeSpan.FromSeconds(.15));
            _animation.AddFrames("up", 8, 8, 32, 48, TimeSpan.FromSeconds(.15));
            _animation.AddFrames("left", 16, 8, 32, 48, TimeSpan.FromSeconds(.15));
            _animation.AddFrames("right", 16, 8, 32, 48, TimeSpan.FromSeconds(.15), true);
            _animation.Sequence = "down";
            _tileGroup = _map.TileGroup(0, 0);
            return base.LoadContent(content, graphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            // Poll for current keyboard state
            KeyboardState state = Keyboard.GetState();
            Point movement = new Point(0, 0);

            if (_walking) {
                movement = (_destinationTile - _currentTile) * new Point(_speed, _speed);
                _offset += movement;
                if (Math.Abs(_offset.X) >= _tileGroup.TileWidth || Math.Abs(_offset.Y) >= _tileGroup.TileHeight)
                {
                    _offset.X = 0;
                    _offset.Y = 0;
                    _walking = false;
                    if (state.IsKeyUp(Keys.A) && state.IsKeyUp(Keys.D) && state.IsKeyUp(Keys.W) && state.IsKeyUp(Keys.S)) _animation.Stop();
                    _currentTile = _destinationTile;
                }
            } else {
                // Move our sprite based on arrow keys being pressed:
                if (state.IsKeyDown(Keys.D)) {
                    _animation.Sequence = "right";
                    if(_currentTile.X + 1 >= _tileGroup.WalkableMap.GetLength(0)){
                        _tileGroup = _map.TileGroup(_tileGroup.X + 1, _tileGroup.Y);
                        _currentTile.X = -1;
                        _destinationTile = new Point(0, _currentTile.Y);
                        _walking = true;
                    } else if (_tileGroup.WalkableMap[_currentTile.X + 1, _currentTile.Y]) {
                        _animation.Start();
                        _destinationTile = new Point(_currentTile.X + 1, _currentTile.Y);
                        _walking = true;
                    }
                } else if (state.IsKeyDown(Keys.A)) {
                    _animation.Sequence = "left";
                    if (_currentTile.X - 1 <= 0) {
                        _tileGroup = _map.TileGroup(_tileGroup.X - 1, _tileGroup.Y);
                        _currentTile.X = _tileGroup.WalkableMap.GetLength(0)+1;
                        _destinationTile = new Point(_tileGroup.WalkableMap.GetLength(0), _currentTile.Y);
                        _walking = true;
                    } else if (_tileGroup.WalkableMap[_currentTile.X - 1, _currentTile.Y]) {
                        _animation.Start();
                        _destinationTile = new Point(_currentTile.X - 1, _currentTile.Y);
                        _walking = true;
                    }
                } else if (state.IsKeyDown(Keys.W)) {
                    _animation.Sequence = "up";
                    if (_currentTile.Y - 1 <= 0) {
                        _tileGroup = _map.TileGroup(_tileGroup.X, _tileGroup.Y - 1);
                        _currentTile.Y = _tileGroup.WalkableMap.GetLength(1) + 1;
                        _destinationTile = new Point(_currentTile.X, _tileGroup.WalkableMap.GetLength(1));
                        _walking = true;
                    } else if (_tileGroup.WalkableMap[_currentTile.X, _currentTile.Y - 1]) {
                        _animation.Start();
                        _destinationTile = new Point(_currentTile.X, _currentTile.Y - 1);
                        _walking = true;
                    }
                } else if (state.IsKeyDown(Keys.S)) {
                    _animation.Sequence = "down";
                    if (_currentTile.Y + 1 >= _tileGroup.WalkableMap.GetLength(1)) {
                        _tileGroup = _map.TileGroup(_tileGroup.X, _tileGroup.Y + 1);
                        _currentTile.Y = -1;
                        _destinationTile = new Point(_currentTile.X, 0);
                        _walking = true;
                    } else if (_tileGroup.WalkableMap[_currentTile.X, _currentTile.Y + 1]) {
                        _animation.Start();
                        _destinationTile = new Point(_currentTile.X, _currentTile.Y + 1);
                        _walking = true;
                    }
                }
            }

            _position.X = (int)_tileGroup.Offset.X + (_tileGroup.TileWidth * _currentTile.X) + _offset.X;
            _position.Y = (int)_tileGroup.Offset.Y + (_tileGroup.TileHeight * _currentTile.Y)+ _offset.Y - 16;
            _map.Scroll(movement.X, movement.Y);

            base.Update(gameTime);
        }
    }
}
