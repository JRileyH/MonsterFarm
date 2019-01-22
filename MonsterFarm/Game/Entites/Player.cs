using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Desktop;
using MonsterFarm.Game.Environment;
using MonsterFarm.Game.States;

namespace MonsterFarm.Game.Entites
{
    public class Player
    {
        public Player(Map map)
        {
            Pawn = new Pawn(map);
            Controller = new Controller();
            Pawn.ReachDestination += (dest) =>
            {
                if (Map.GetType().Equals(typeof(StaticMap))) {
                    foreach (KeyValuePair<string, Transition> transition in Map.Transitions) {
                        if (dest == transition.Value.Entry) {
                            WorldState.transition(Pawn, transition.Key, transition.Value.Exit);
                            break;
                        }
                    }
                }
                if (Map.GetType().Equals(typeof(ProceduralMap))) {
                    if (((ProceduralMap)Map).Warp == dest) {
                        ((ProceduralMap)Map).Rebuild();
                        dest = Map.Start;
                    }
                }
            };
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
        Vector2 _centerScreen = new Vector2();

        public Player LoadContent(ContentManager content, GraphicsDevice graphicsDevice) {
            Controller.Pawn = Pawn.LoadContent(content, graphicsDevice);
            //Animation = new Animation(content.Load<Texture2D>(@"Entities/player"));
            Animation = new Animation(Global.Wardrobe.getSheet("Elf","Male"));
            Animation.AddFrames("down", 0, 8, 16, 24, TimeSpan.FromSeconds(.15));
            Animation.AddFrames("up", 8, 8, 16, 24, TimeSpan.FromSeconds(.15));
            Animation.AddFrames("left", 16, 8, 16, 24, TimeSpan.FromSeconds(.15));
            Animation.AddFrames("right", 16, 8, 16, 24, TimeSpan.FromSeconds(.15), true);
            Animation.Sequence = "down";
            _centerScreen = new Vector2(
                (graphicsDevice.Viewport.Width / 2) - (Pawn.Width / 2),
                (graphicsDevice.Viewport.Height / 2) - (Pawn.Height / 2)
            );
            return this;
        }

        public void Update(GameTime gameTime) {
            Pawn.Update(gameTime);
            if(Pawn.RenderPosition != _centerScreen) {
                Vector2 correctionShift = _centerScreen - Pawn.RenderPosition;
                Pawn.Map.Shift(correctionShift);
            }
            Controller.Update(gameTime);
        }

        public void Render(SpriteBatch spriteBatch, Viewport viewport) {
            Pawn.Render(spriteBatch, viewport);
        }
    }
}
