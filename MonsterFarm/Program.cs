using System;
using Microsoft.Xna.Framework.Graphics;
using MonsterFarm.Game.Util;

namespace MonsterFarm.Desktop
{
    public static class Global{
        public static readonly Random rnd = new Random();
        public static KeyboardHandler keyboardHandler = new KeyboardHandler();
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
