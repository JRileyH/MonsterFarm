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
        ProceduralMap proceduralMap;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            proceduralMap = new ProceduralMap().LoadContent(Content, GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UserInterface.Active.Update(gameTime);

            // Poll for current keyboard state
            KeyboardState state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            // Move our sprite based on arrow keys being pressed:
            if (state.IsKeyDown(Keys.Right))
                proceduralMap.Scroll(10, 0);
            if (state.IsKeyDown(Keys.Left))
                proceduralMap.Scroll(-10, 0);
            if (state.IsKeyDown(Keys.Up))
                proceduralMap.Scroll(0, -10);
            if (state.IsKeyDown(Keys.Down))
                proceduralMap.Scroll(0, 10);
            if (state.IsKeyDown(Keys.Space))
                proceduralMap.Scroll(0, 0);

            proceduralMap.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            UserInterface.Active.Draw(spriteBatch);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            proceduralMap.Render(spriteBatch, GraphicsDevice.Viewport);
            spriteBatch.End();

            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
