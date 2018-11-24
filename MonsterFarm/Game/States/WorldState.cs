using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Entites;
using MonsterFarm.Game.Environment;
using MonsterFarm.Game.Util;
using MonsterFarm.UI;

namespace MonsterFarm.Game.States
{
    public class WorldState : State
    {
        //Player player;
        //ProceduralMap proceduralMap;
        StaticMap tavern;
        Pawn pawn;
        Controller controller;
        Animation playerAnimation;

        public static KeyboardHandler keyboardHandler = new KeyboardHandler();

        public WorldState(UserInterface ui) : base(ui)
        {
            _name = "World";
            //proceduralMap = new ProceduralMap();
            //player = new Player(proceduralMap);
            tavern = new StaticMap();
            pawn = new Pawn(tavern);
            controller = new Controller();
        }

        public override State LoadContent(ContentManager content, GraphicsDevice graphicsDevice, bool useRenderTarget = false){
            //proceduralMap.LoadContent(content, graphicsDevice);
            //player.LoadContent(content, graphicsDevice);
            tavern.LoadContent(content, graphicsDevice);
            controller.Pawn = pawn.LoadContent(content, graphicsDevice);

            playerAnimation = new Animation(content.Load<Texture2D>(@"Entities/player"));
            playerAnimation.AddFrames("down", 0, 8, 32, 48, TimeSpan.FromSeconds(.15));
            playerAnimation.AddFrames("up", 8, 8, 32, 48, TimeSpan.FromSeconds(.15));
            playerAnimation.AddFrames("left", 16, 8, 32, 48, TimeSpan.FromSeconds(.15));
            playerAnimation.AddFrames("right", 16, 8, 32, 48, TimeSpan.FromSeconds(.15), true);
            playerAnimation.Sequence = "down";

            pawn.Animation = playerAnimation;

            return base.LoadContent(content, graphicsDevice, useRenderTarget);
        }

        public override State Start(){
            //proceduralMap.Scroll(0, 0);
            return base.Start();
        }

        public override void Stop(){
            //proceduralMap.Scroll(0, 0);
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            keyboardHandler.Update(gameTime);

            //proceduralMap.Update(gameTime);

            //player.Update(gameTime);

            tavern.Update(gameTime);
            pawn.Update(gameTime);
            controller.Update(gameTime);
            base.Update(gameTime);

        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            base.Render(spriteBatch, viewport);
            //proceduralMap.Render(spriteBatch, viewport);
            //player.Render(spriteBatch, viewport);
            tavern.Render(spriteBatch, viewport);
            pawn.Render(spriteBatch, viewport);
            tavern.RenderOverlay(spriteBatch, viewport);
        }
    }
}
