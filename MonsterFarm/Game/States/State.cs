using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.UI;

namespace MonsterFarm.Game.States
{
    public abstract class State
    {
        protected bool _initialized = false;
        protected bool _running = false;
        protected UserInterface _ui;

        public State(UserInterface ui)
        {
            _ui = ui;
        }

        public virtual State Start(){
            UserInterface.Active = _ui;
            _running = true;
            return this;
        }
        public virtual void Stop(){
            _running = false;
        }

        public virtual State LoadContent(ContentManager content, GraphicsDevice graphicsDevice, bool useRenderTarget = false)
        {
            _ui.UseRenderTarget = useRenderTarget;
            _initialized = true;
            return this;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!_initialized) throw new Exception("Must GenerateWorld before updating");
        }



        public virtual void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (!_initialized) throw new Exception("Must GenerateWorld before rendering");
        }
    }
}
