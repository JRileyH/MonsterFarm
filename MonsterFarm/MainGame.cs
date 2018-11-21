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
using MonsterFarm.Game.Util;
using MonsterFarm.UI;
using MonsterFarm.UI.Elements;
using static MonsterFarm.Game.Util.KeyboardHandler;

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

            Global.keyboardHandler.Subscribe(KeyTrigger.Release, Keys.Escape, Exit);
            Global.keyboardHandler.Subscribe(KeyTrigger.Release, Keys.P, ()=>{
                chooseState("Menu");
            });
            Global.keyboardHandler.Subscribe(KeyTrigger.Release, Keys.O, ()=>{
                chooseState("World");
            });

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
            if (activeState.Name != state){
                lastState = activeState;
                foreach (KeyValuePair<string, State> _state in allStates)
                {
                    _state.Value.Stop();
                }
                activeState = allStates[state].Start();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Global.keyboardHandler.Update(gameTime);
            UserInterface.Active.Update(gameTime);
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
