using System;

namespace MonsterFarm.Desktop
{
    public static class Global{
        public static readonly Random rnd = new Random();
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
