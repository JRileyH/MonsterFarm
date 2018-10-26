using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game;
using MonsterFarm.Game.Entites;

using MonsterFarm.UI;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Desktop
{
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Menagerie Menagerie;
        MonsterInfo MonsterInfo;
        Random rng = new Random();


        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900
            };
            Content.RootDirectory = "Content";

        }

        string[] MonsterPool = new string[]{
            "ss1",
            "es2",
            "bf2",
            "ge1",
            "fg1",
            "cf2"
        };

        protected override void Initialize()
        {
            UserInterface.Initialize(Content);
            UserInterface.Active.UseRenderTarget = true;

            //Background stuff
            Button addRandomButton = new Button("Add", anchor: Anchor.TopLeft, size: new Vector2(150,50));
            addRandomButton.OnClick = (Element btn) => {
                Menagerie.AddMonster(new Monster(MonsterPool[rng.Next(0,6)]));
            };
            UserInterface.Active.AddElement(addRandomButton);

            Button showMenagerie = new Button("Show", anchor: Anchor.AutoInline, size: new Vector2(150, 50));
            showMenagerie.OnClick = (Element btn) => {
                Menagerie.Show();
            };
            UserInterface.Active.AddElement(showMenagerie);

            //UI stuff
            Menagerie = new Menagerie(UserInterface.Active);
            MonsterInfo = new MonsterInfo(UserInterface.Active);
            Menagerie.OnSelect = (Monster monster) => {
                MonsterInfo.Show(monster);
            };
            Menagerie.OnClose = (Monster monster) => {
                MonsterInfo.Hide();
            };
            MonsterInfo.OnKill = (Monster monster) => {
                Menagerie.RemoveMonster(monster);
            };

            Menagerie.AddMonster(new Monster("ss1"));
            Menagerie.AddMonster(new Monster("es2"));
            Menagerie.AddMonster(new Monster("bf2"));
            Menagerie.AddMonster(new Monster("ge1"));
            Menagerie.AddMonster(new Monster("fg1"));
            Menagerie.AddMonster(new Monster("cf2"));

            Menagerie.Show();

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

            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
