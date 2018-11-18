using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Environment;
using MonsterFarm.UI;

namespace MonsterFarm.Game.States
{
    public class WorldState : State
    {
        ProceduralMap proceduralMap;

        public WorldState(UserInterface ui) : base(ui)
        {
            proceduralMap = new ProceduralMap();
        }

        public override State LoadContent(ContentManager content, GraphicsDevice graphicsDevice, bool useRenderTarget = false){
            proceduralMap.LoadContent(content, graphicsDevice);
            return base.LoadContent(content, graphicsDevice, useRenderTarget);
        }

        public override State Start(){
            proceduralMap.Scroll(0, 0);
            return base.Start();
        }

        public override void Stop(){
            proceduralMap.Scroll(0, 0);
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            // Poll for current keyboard state
            KeyboardState state = Keyboard.GetState();

            // Move our sprite based on arrow keys being pressed:
            if (state.IsKeyDown(Keys.D))
                proceduralMap.Scroll(10, 0);
            if (state.IsKeyDown(Keys.A))
                proceduralMap.Scroll(-10, 0);
            if (state.IsKeyDown(Keys.W))
                proceduralMap.Scroll(0, -10);
            if (state.IsKeyDown(Keys.S))
                proceduralMap.Scroll(0, 10);
            if (state.IsKeyDown(Keys.Space))
                proceduralMap.Scroll(0, 0);

            proceduralMap.Update(gameTime);

            base.Update(gameTime);

        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            base.Render(spriteBatch, viewport);
            proceduralMap.Render(spriteBatch, viewport);
        }
    }
}
