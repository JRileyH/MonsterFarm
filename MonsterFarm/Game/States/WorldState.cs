using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.Entites;
using MonsterFarm.Game.Environment;
using MonsterFarm.Game.Util;
using MonsterFarm.UI;

namespace MonsterFarm.Game.States
{
    public class WorldState : State
    {
        //Player player;
        //ProceduralMap proceduralMap;
        static Dictionary<string, StaticMap> maps;
        Pawn pawn;
        Controller controller;
        Animation playerAnimation;

        public static KeyboardHandler keyboardHandler = new KeyboardHandler();

        public static void transition(Pawn pawn, string map, Point entry){
            if(maps.ContainsKey(map)){
                pawn.Stop();
                pawn.Map = maps[map];
                pawn.Position = entry;
            }
        }

        public WorldState(UserInterface ui) : base(ui)
        {
            _name = "World";
            //proceduralMap = new ProceduralMap();
            //player = new Player(proceduralMap);
            maps = new Dictionary<string, StaticMap> {
                {"tavern", new StaticMap("tavern")},
                {"street", new StaticMap("street")}
            };
            pawn = new Pawn(maps["street"]);
            controller = new Controller();
        }

        public override State LoadContent(ContentManager content, GraphicsDevice graphicsDevice, bool useRenderTarget = false){
            //proceduralMap.LoadContent(content, graphicsDevice);
            //player.LoadContent(content, graphicsDevice);
            foreach(StaticMap map in maps.Values){
                map.LoadContent(content, graphicsDevice);
            }
            controller.Pawn = pawn.LoadContent(content, graphicsDevice);

            playerAnimation = new Animation(content.Load<Texture2D>(@"Entities/player"));
            playerAnimation.AddFrames("down", 0, 8, 32, 48, TimeSpan.FromSeconds(.15));
            playerAnimation.AddFrames("up", 8, 8, 32, 48, TimeSpan.FromSeconds(.15));
            playerAnimation.AddFrames("left", 16, 8, 32, 48, TimeSpan.FromSeconds(.15));
            playerAnimation.AddFrames("right", 16, 8, 32, 48, TimeSpan.FromSeconds(.15), true);
            playerAnimation.Sequence = "down";

            pawn.Animation = playerAnimation;

            return base.LoadContent(content, graphicsDevice, useRenderTarget);
        }

        public override State Start(){
            //proceduralMap.Scroll(0, 0);
            return base.Start();
        }

        public override void Stop(){
            //proceduralMap.Scroll(0, 0);
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            keyboardHandler.Update(gameTime);

            //proceduralMap.Update(gameTime);

            //player.Update(gameTime);

            pawn.Map.Update(gameTime);
            pawn.Update(gameTime);
            controller.Update(gameTime);
            base.Update(gameTime);

        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            base.Render(spriteBatch, viewport);
            //proceduralMap.Render(spriteBatch, viewport);
            //player.Render(spriteBatch, viewport);
            pawn.Map.Render(spriteBatch, viewport);
            pawn.Render(spriteBatch, viewport);
            pawn.Map.RenderOverlay(spriteBatch, viewport);
        }
    }
}
