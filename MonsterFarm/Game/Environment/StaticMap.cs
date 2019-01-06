using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Utils.Tiled;
using static MonsterFarm.Utils.Tiled.Layer;

namespace MonsterFarm.Game.Environment
{
    public class StaticMap : Map
    {
        public StaticMap(string name) : base(name)
        {
            _root += "static/";
        }

        public new StaticMap LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            if (_initialized) throw new Exception("Cannot call LoadContent twice");
            base.LoadContent(content, graphicsDevice);

            TmxMap map = new TmxMap(_root + _name + ".tmx");

            foreach (TmxLayer layer in map.Layers)
            {
                if (layer.Name == "Start") {
                    foreach (TmxLayerTile tile in layer.Tiles){
                        if (tile.Gid != 0) {
                            Start = new Vector2(tile.X, tile.Y);
                            break;
                        }
                    }
                } else if(layer.Name.StartsWith("-X-")) {
                    string[] transitionInfo = layer.Name.Remove(0,3).Split('-');
                    string[] transitionCoordinate = transitionInfo[1].Split(',');
                    foreach (TmxLayerTile tile in layer.Tiles){
                        if (tile.Gid != 0){
                            Transitions[transitionInfo[0]] = new Transition(new Vector2(tile.X, tile.Y), new Vector2(int.Parse(transitionCoordinate[0]), int.Parse(transitionCoordinate[1])));
                            break;
                        }
                    }
                }
            }
            _initialized = true;
            return this;
        }
    }
}
