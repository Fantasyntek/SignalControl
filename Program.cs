using System;

namespace SignalControl
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SignalControlGame())
                game.Run();
        }
    }
}
