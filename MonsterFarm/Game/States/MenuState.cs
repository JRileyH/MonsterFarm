using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Desktop;
using MonsterFarm.Game.Entites;
using MonsterFarm.UI;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Game.States
{
    public class MenuState : State
    {
        Menagerie menagerie;
        MonsterInfo monsterInfo;
        string[] monsterPool = new string[]{
            "ss1",
            "es2",
            "bf2",
            "ge1",
            "fg1",
            "cf2"
        };

        public MenuState(UserInterface ui) : base(ui)
        {
            //Background stuff
            Button addRandomButton = new Button("Add", anchor: Anchor.TopLeft, size: new Vector2(150, 50));
            addRandomButton.OnClick = (Element btn) => {
                menagerie.AddMonster(new Monster(monsterPool[Global.rnd.Next(0, 6)]));
            };
            _ui.AddElement(addRandomButton);

            Button showMenagerie = new Button("Show", anchor: Anchor.AutoInline, size: new Vector2(150, 50));
            showMenagerie.OnClick = (Element btn) => {
                menagerie.Show();
            };
            _ui.AddElement(showMenagerie);

            menagerie = new Menagerie(ui);
            monsterInfo = new MonsterInfo(ui);
            menagerie.OnSelect = (Monster monster) => {
                monsterInfo.Show(monster);
            };
            menagerie.OnClose = (Monster monster) => {
                monsterInfo.Hide();
            };
            monsterInfo.OnKill = (Monster monster) => {
                menagerie.RemoveMonster(monster);
            };

            //TODO: get this stuff from disk;
            menagerie.AddMonster(new Monster("ss1"));
            menagerie.AddMonster(new Monster("es2"));
            menagerie.AddMonster(new Monster("bf2"));
            menagerie.AddMonster(new Monster("ge1"));
            menagerie.AddMonster(new Monster("fg1"));
            menagerie.AddMonster(new Monster("cf2"));
        }

        public override State LoadContent(ContentManager content, GraphicsDevice graphicsDevice, bool useRenderTarget = true)
        {
            return base.LoadContent(content, graphicsDevice, useRenderTarget);
        }

        public override State Start()
        {
            menagerie.Show();
            return base.Start();
        }

        public override void Stop()
        {
            menagerie.Hide();
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            base.Render(spriteBatch, viewport);
        }
    }
}
