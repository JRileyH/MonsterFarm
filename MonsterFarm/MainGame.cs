using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Entites;

using MonsterFarm.UI;
using MonsterFarm.UI.Elements;

namespace MonsterFarm.Desktop
{
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Panel MonsterListPanel;
        Panel MonsterSelectionPanel;
        Panel MonsterBreedingPanel;

        List<Monster> MonsterList = new List<Monster>();


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

            MonsterList.Add(new Monster("es2"));
            MonsterList.Add(new Monster("ss1"));
            MonsterList.Add(new Monster("bp2"));

            MonsterListPanel = new Panel(new Vector2(500, 700), anchor: Anchor.CenterLeft, offset: new Vector2(30, 0));
            MonsterListPanel.AddChild(new Header("Monster List:"));
            MonsterListPanel.AddChild(new HorizontalLine());
            MonsterListPanel.AddChild(new LineSpace(2));

            MonsterSelectionPanel = new Panel(new Vector2(500, 300), offset: new Vector2(0, -160));
            deselectMonster();
            MonsterBreedingPanel = new Panel(new Vector2(500, 300), offset: new Vector2(0, 160));

            foreach(Monster m in MonsterList){
                createMonsterRow(m);
            }

            UserInterface.Active.AddElement(MonsterListPanel);
            UserInterface.Active.AddElement(MonsterSelectionPanel);
            UserInterface.Active.AddElement(MonsterBreedingPanel);

            base.Initialize();
        }

        public Monster selectedMonster = null;

        public void deselectMonster()
        {
            selectedMonster = null;
            MonsterSelectionPanel.ClearChildren();
            MonsterSelectionPanel.AddChild(new Paragraph("Select a Monster from the list!", anchor: Anchor.Center));
        }

        public void selectMonster(ref Monster monster){
            selectedMonster = monster;
            MonsterSelectionPanel.ClearChildren();
            MonsterSelectionPanel.AddChild(new Image(Resources.IconTextures[monster.monsterIcon], size: new Vector2(120, 120)));
            MonsterSelectionPanel.AddChild(new Icon(monster.familyIcon, anchor: Anchor.AutoInline, offset: new Vector2(10, 0)));
            MonsterSelectionPanel.AddChild(new Paragraph(monster.name, anchor: Anchor.AutoInline, size: new Vector2(200, 50), offset: new Vector2(10, 30)));
            Button monsterDeselectButton = new Button("X", anchor: Anchor.AutoInline, size: new Vector2(40, 40), textScale: 0.7f, nobreak: true);
            monsterDeselectButton.OnClick = (UI.Elements.Element btn) => {
                deselectMonster();
            };
            MonsterSelectionPanel.AddChild(monsterDeselectButton);
            HorizontalLine hl = new HorizontalLine(Anchor.TopRight, offset: new Vector2(0, 50));
            hl.Size = new Vector2(300, 8);
            MonsterSelectionPanel.AddChild(hl);
            MonsterSelectionPanel.AddChild(new MulticolorParagraph("{{RED}}HP{{WHITE}}: {{YELLOW}}290{{WHITE}}/400{{BLUE}}\nM{{WHITE}}P: 17/17", Anchor.Center, offset: new Vector2(0,-30)));
        }

        public void createMonsterRow(Monster monster){
            MonsterListPanel.AddChild(new Icon(monster.monsterIcon, anchor: Anchor.AutoInline, background: true));
            MonsterListPanel.AddChild(new Icon(monster.familyIcon, anchor: Anchor.AutoInline, offset: new Vector2(10, 0)));
            MonsterListPanel.AddChild(new Paragraph(monster.name, anchor: Anchor.AutoInline, size: new Vector2(230, 50), offset: new Vector2(10, 30)));
            Button monsterSelectButton = new Button("Select", anchor: Anchor.AutoInline, size: new Vector2(90, 40), textScale: 0.7f, nobreak: true);
            MonsterListPanel.AddChild(monsterSelectButton);
            MonsterListPanel.AddChild(new HorizontalLine(Anchor.Auto, offset: new Vector2(0, 15)));
            monsterSelectButton.OnClick = (UI.Elements.Element btn) => {
                selectMonster(ref monster);
            };
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            UserInterface.Active.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
