using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Game.Environment;

namespace MonsterFarm.Game.Entites
{
    public class Player
    {
        public Player(Map map)
        {
            Pawn = new Pawn(map);
            Controller = new Controller();
        }
        public Pawn Pawn { get; private set; }
        public Controller Controller { get; private set; }
        public Animation Animation {
            get => Pawn.Animation;
            private set => Pawn.Animation = value; }
        public Map Map
        {
            get => Pawn.Map;
            set => Pawn.Map = value;
        }

        public Player LoadContent(ContentManager content, GraphicsDevice graphicsDevice) {

            Controller.Pawn = Pawn.LoadContent(content, graphicsDevice);
            Animation = new Animation(content.Load<Texture2D>(@"Entities/player"));
            Animation.AddFrames("down", 0, 8, 32, 48, TimeSpan.FromSeconds(.15));
            Animation.AddFrames("up", 8, 8, 32, 48, TimeSpan.FromSeconds(.15));
            Animation.AddFrames("left", 16, 8, 32, 48, TimeSpan.FromSeconds(.15));
            Animation.AddFrames("right", 16, 8, 32, 48, TimeSpan.FromSeconds(.15), true);
            Animation.Sequence = "down";
            return this;
        }

        public void Update(GameTime gameTime) {
            Pawn.Update(gameTime);
            Controller.Update(gameTime);
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport) {
            Pawn.Render(spriteBatch, viewport);
        }
    }
}
