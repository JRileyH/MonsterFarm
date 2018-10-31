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
        TileGroup tileGroup;
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

            tileGroup = new TileGroup().LoadContent(Content).LoadContent(Content);

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            UserInterface.Active.Draw(spriteBatch);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            tileGroup.Render(spriteBatch);
            spriteBatch.End();

            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
