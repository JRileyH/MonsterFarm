using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static Dictionary<string, StaticMap> maps;
        Player player;
        public static Map activeMap;

        public static KeyboardHandler keyboardHandler = new KeyboardHandler();

        public static void transition(Pawn pawn, string map, Vector2 entry){
            if(maps.ContainsKey(map)){
                pawn.Stop();
                pawn.Map = maps[map];
                pawn.Position = entry;
                activeMap = maps[map];
            }
        }

        public WorldState(UserInterface ui) : base(ui)
        {
            _name = "World";
            maps = new Dictionary<string, StaticMap> {
                {"tavern", new StaticMap("tavern")},
                {"street", new StaticMap("street")}
            };
            activeMap = maps["street"];
            player = new Player(activeMap);
        }

        public override State LoadContent(ContentManager content, GraphicsDevice graphicsDevice, bool useRenderTarget = false){
            foreach(StaticMap map in maps.Values){
                map.LoadContent(content, graphicsDevice);
            }
            player.LoadContent(content, graphicsDevice);

            return base.LoadContent(content, graphicsDevice, useRenderTarget);
        }

        public override State Start(){
            return base.Start();
        }

        public override void Stop(){
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            keyboardHandler.Update(gameTime);
            activeMap.Update(gameTime);
            player.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch, Viewport viewport)
        {
            base.Render(spriteBatch, viewport);
            activeMap.Render(spriteBatch, viewport);
            player.Render(spriteBatch, viewport);
            activeMap.RenderOverlay(spriteBatch, viewport);
        }
    }
}
