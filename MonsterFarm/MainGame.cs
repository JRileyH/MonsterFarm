using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game;
using MonsterFarm.Game.Entites;
using MonsterFarm.Game.Environment;
using MonsterFarm.Game.States;
using MonsterFarm.UI;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Desktop
{
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<string, State> allStates;
        State activeState;
        State lastState;


        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1422,
                PreferredBackBufferHeight = 800
            };
            Content.RootDirectory = "Content";

            allStates = new Dictionary<string, State>();


        }

        protected override void Initialize()
        {
            UserInterface.Initialize(Content);

            allStates["World"] = new WorldState(new UserInterface());
            allStates["Menu"] = new MenuState(new UserInterface());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            allStates["World"].LoadContent(Content, GraphicsDevice);
            allStates["Menu"].LoadContent(Content, GraphicsDevice, true);

            activeState = allStates["World"].Start();

        }

        protected override void UnloadContent()
        {

        }

        private void chooseState(string state){
            foreach (KeyValuePair<string, State> _state in allStates){
                _state.Value.Stop();
            }
            activeState = allStates[state].Start();
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

            if (state.IsKeyDown(Keys.O))
                chooseState("World");
            if (state.IsKeyDown(Keys.P))
                chooseState("Menu");

            activeState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            UserInterface.Active.Draw(spriteBatch);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            activeState.Render(spriteBatch, GraphicsDevice.Viewport);
            spriteBatch.End();

            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
