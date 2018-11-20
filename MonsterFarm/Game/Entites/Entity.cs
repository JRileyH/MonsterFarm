using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonsterFarm.Game.Entites
{
    public class Entity
    {
        private bool _initialized = false;
        protected string _root;
        protected string _textureName;
        protected Texture2D _textureSheet;
        protected Animation _animation;
        protected Vector2 _position;

        public Entity()
        {
            _position = new Vector2(0, 0);
        }
        public virtual Entity LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {

            _initialized = true;
            return this;
        }

        public virtual void Update(GameTime gameTime){
            if (!_initialized) throw new Exception("Must call LoadContent before updating Entity");

            _animation.Update(gameTime);
        }

        public virtual void Render(SpriteBatch spriteBatch, Viewport viewport){
            if (!_initialized) throw new Exception("Must call LoadContent before rendering Entity");
            _animation.Render(_position, spriteBatch);
        }
    }
}
