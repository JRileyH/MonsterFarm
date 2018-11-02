using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game;
using MonsterFarm.Game.Entites;
using MonsterFarm.Game.Environment;
using MonsterFarm.UI;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Desktop
{
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TileGroup[] tgs;
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900
            };
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            UserInterface.Initialize(Content);
            UserInterface.Active.UseRenderTarget = true;
            tgs = new TileGroup[100];
            string tgid = "r2-v1";
            for (int i = 0; i < 100; i++){
                tgs[i] = new TileGroup(tgid, new Vector2(27*32*i, 0)).LoadContent(Content);
                switch(tgid){
                    case "r2-v1":
                        tgid = "t2-l2-r2-b2-v1";
                        break;
                    case "t2-l2-r2-b2-v1":
                        tgid = "l2-v1";
                        break;
                    case "l2-v1":
                        tgid = "r2-v1";
                        break;
            }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UserInterface.Active.Update(gameTime);

            foreach (TileGroup tg in tgs){
                tg.Update(gameTime);
            }

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            UserInterface.Active.Draw(spriteBatch);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            foreach(TileGroup tg in tgs){
                tg.Render(spriteBatch, GraphicsDevice.Viewport);
            }
            spriteBatch.End();

            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
