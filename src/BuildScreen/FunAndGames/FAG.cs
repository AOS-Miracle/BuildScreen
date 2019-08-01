using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BuildScreen.FunAndGames
{
    public static class FAG
    {
        private static bool FunAndGames; //FunAndGames
        private static string[,,] PreventOverrun = new string[,,]{};

        public static void ActivateFunAndGames()
        {
            FunAndGames = File.Exists("FunAndGames");
        }

    }
}
