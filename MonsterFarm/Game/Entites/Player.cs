using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Environment;
using MonsterFarm.Game.States;
using static MonsterFarm.Game.Util.KeyboardHandler;

namespace MonsterFarm.Game.Entites
{
    public class Player : Entity
    {
        private ProceduralMap _map;
        private bool _walking;
        private TileGroup _tileGroup;
        private Point _currentTileCoordinates;
        private Point _destinationTileCoordinates;
        private Point _offset;
        private SpriteFont _font;

        public Player(ProceduralMap map)
        {
            _root = @"Entities/";
            _textureName = "player";
            _map = map;
            _walking = false;
            _currentTileCoordinates = new Point(10, 9);
            _offset = new Point(0, 0);
        }
        public override Entity LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _font = content.Load<SpriteFont>("UI/fonts/Regular");
            _animation = new Animation(content.Load<Texture2D>(_root + _textureName));
            _animation.AddFrames("down", 0, 8, 32, 48, TimeSpan.FromSeconds(.15));
            _animation.AddFrames("up", 8, 8, 32, 48, TimeSpan.FromSeconds(.15));
            _animation.AddFrames("left", 16, 8, 32, 48, TimeSpan.FromSeconds(.15));
            _animation.AddFrames("right", 16, 8, 32, 48, TimeSpan.FromSeconds(.15), true);
            _animation.Sequence = "down";
            _tileGroup = _map.TileGroup(0, 0);
            return base.LoadContent(content, graphicsDevice);
        }

        void startWalking(string direction, Point destination){
            _animation.Sequence = direction;
            if (_tileGroup.WalkableMap[destination.X, destination.Y]){
                _destinationTileCoordinates = destination;
                _animation.Start();
                _walking = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Point movement = new Point(0, 0);
            if (_walking)
            {
                movement = (_destinationTileCoordinates - _currentTileCoordinates) * new Point(4, 4);
                _offset += movement;
                if (Math.Abs(_offset.X) >= _tileGroup.TileWidth || Math.Abs(_offset.Y) >= _tileGroup.TileHeight)
                {
                    _offset = new Point(0, 0);
                    _currentTileCoordinates = _destinationTileCoordinates;
                    _animation.Stop();
                    _walking = false;
                }
            }

            WorldState.keyboardHandler.Subscribe(KeyTrigger.Hold, Keys.A, ()=>{
                if (!_walking){
                    Point destination = _currentTileCoordinates + new Point(-1, 0);
                    if (destination.X <= 0){
                        _tileGroup = _map.TileGroup(_tileGroup.X - 1, _tileGroup.Y);
                        _currentTileCoordinates.X = _tileGroup.WalkableMap.GetLength(0);
                        _offset.X = _tileGroup.TileWidth;
                        destination = new Point(_tileGroup.WalkableMap.GetLength(0)-1, destination.Y);
                    }
                    startWalking("left", destination);
                }
            });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Hold, Keys.D, () => {
                if (!_walking){
                    Point destination = _currentTileCoordinates + new Point(1, 0);
                    if (destination.X >= _tileGroup.WalkableMap.GetLength(0)){
                        _tileGroup = _map.TileGroup(_tileGroup.X + 1, _tileGroup.Y);
                        _currentTileCoordinates.X = -1;
                        destination = new Point(0, destination.Y);
                    }
                    startWalking("right", destination);
                }
            });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Hold, Keys.W, () => {
                if (!_walking){
                    Point destination = _currentTileCoordinates + new Point(0, -1);
                    if (destination.Y <= 0){
                        _tileGroup = _map.TileGroup(_tileGroup.X, _tileGroup.Y - 1);
                        _currentTileCoordinates.Y = _tileGroup.WalkableMap.GetLength(1);
                        _offset.Y = _tileGroup.TileHeight;
                        destination = new Point(destination.X, _tileGroup.WalkableMap.GetLength(1) - 1);
                    }
                    startWalking("up", destination);
                }
            });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Hold, Keys.S, () => {
                if (!_walking){
                    Point destination = _currentTileCoordinates + new Point(0, 1);
                    if (destination.Y >= _tileGroup.WalkableMap.GetLength(1)){
                        _tileGroup = _map.TileGroup(_tileGroup.X, _tileGroup.Y + 1);
                        _currentTileCoordinates.Y = -1;
                        destination = new Point(destination.X, 0);
                    }
                    startWalking("down", destination);
                }
            });

            _position.X = (int)_tileGroup.Offset.X + (_tileGroup.TileWidth * _currentTileCoordinates.X) + _offset.X;
            _position.Y = (int)_tileGroup.Offset.Y + (_tileGroup.TileHeight * _currentTileCoordinates.Y) + _offset.Y - 16;
            _map.Scroll(movement.X, movement.Y);

            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            spriteBatch.DrawString(_font, "Tile: " + _currentTileCoordinates.X + ", " + _currentTileCoordinates.Y, new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(_font, "Dest: " + _destinationTileCoordinates.X + ", " + _destinationTileCoordinates.Y, new Vector2(20, 40), Color.Red);
            spriteBatch.DrawString(_font, "TGrp: " + _tileGroup.X + ", " + _tileGroup.Y, new Vector2(20, 60), Color.Red);

            base.Render(spriteBatch, viewport);
        }
    }
}
