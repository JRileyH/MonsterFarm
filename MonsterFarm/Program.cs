using System;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Game.Entites;
using MonsterFarm.Game.Util;

namespace MonsterFarm.Desktop
{
    public static class Global{
        public static readonly Random rnd = new Random();
        public static KeyboardHandler keyboardHandler = new KeyboardHandler();

        public static Wardrobe Wardrobe = new Wardrobe(@"Entities/Races/");
    }
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
}
