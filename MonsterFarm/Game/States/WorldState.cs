using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Entites;
using MonsterFarm.Game.Environment;
using MonsterFarm.UI;

namespace MonsterFarm.Game.States
{
    public class WorldState : State
    {
        Player player;
        ProceduralMap proceduralMap;

        public WorldState(UserInterface ui) : base(ui)
        {
            proceduralMap = new ProceduralMap();
            player = new Player(proceduralMap);
        }

        public override State LoadContent(ContentManager content, GraphicsDevice graphicsDevice, bool useRenderTarget = false){
            proceduralMap.LoadContent(content, graphicsDevice);
            player.LoadContent(content, graphicsDevice);
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

            proceduralMap.Update(gameTime);

            player.Update(gameTime);

            base.Update(gameTime);

        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            base.Render(spriteBatch, viewport);
            proceduralMap.Render(spriteBatch, viewport);
            player.Render(spriteBatch, viewport);
        }
    }
}
