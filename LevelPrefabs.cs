using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffJamGame
{
    // LevelWidth X 12 prefabs of level.
    public class LevelPrefabs
    {
        public const int prefabHeight = 12;

        public const string Prefab1 =
            "#..............#" +
            "#..............#" +
            "##########.....#" +
            "#..............#" +
            "#T...T#>..T.^^^#" +
            "#..^..#^^^^#####" +
            "#.....######...#" +
            "#..............#" +
            "#.TT..T####T...#" +
            "#^^^^^##v......#" +
            "#######....^^..#" +
            "######....######";
        //public const string Prefab2 =
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################" +
        //    "################";
        //public const string Prefab3 =
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^" +
        //    "^^^^^^^^^^^^^^^^";

        public static string[] Prefabs = new string[]
        {
            Prefab1,
            //Prefab2,
            //Prefab3,
        };

        public static eTileType CharToTileType(char chr)
        {
            switch (chr)
            {
                case '.': return eTileType.None;
                case '#': return eTileType.Block;
                case 'T': return eTileType.JumpThrough;
                case '^': return eTileType.Deadly_UP;
                case '>': return eTileType.Deadly_RIGHT;
                case '<': return eTileType.Deadly_LEFT;
                case 'v': return eTileType.Deadly_DOWN;
                default: throw new Exception("unknown character '" + chr + "'!");
            }
        }

        public static string GetRandomPrefab(Random r, ref int exclude)
        {
            int chosen;
            do {
                chosen = r.Next(Prefabs.Length);
            }
            while (chosen == exclude && Prefabs.Length > 1);
            exclude = chosen;
            return Prefabs[chosen];
        }
    }
}
