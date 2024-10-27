using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDeepEnd
{
    public struct Prefab
    {
        public string data;
        public int height;
    }

    // LevelWidth X 12 prefabs of level.
    public class LevelPrefabs
    {
        public const string Prefab1 =
            "WWWWWWW..WWWWWWW" +
            "Wvvvv#.S.......W" +
            "W....#####.....W" +
            "W..............W" +
            "W.T#T.##..T.^^^W" +
            "W.<#>..#^^^^###W" +
            "W..#...#####...W" +
            "W..............W" +
            "WTT..^TWWWWT...W" +
            "W^^^^WWWv......W" +
            "WWWWWWW......^^W" +
            "WWWWWW....WWWWWW";
        public const string Prefab2 =
            "BMMMMMM..MMMMMMB" +
            "B.......S......B" +
            "B.#>...TT...<#.B" +
            "B.#>...^^...<#.B" +
            "B.#>..<##>..<#.B" +
            "B.#>........<#.B" +
            "B.TTT.^^^^.TTT.B" +
            "B^^^..####..^^^B" +
            "B###.T####T.###B" +
            "BBB..<####>..P.B" +
            "BBB....vv......B" +
            "BMMMMM....MMMMMB";
        public const string Prefab3 =
            "BMMMMMM..MMMMMMB" +
            "B>.......#.....B" +
            "B>..$..S.#..^..B" +
            "B>..######TWWW.B" +
            "B>.....vv..WWW.B" +
            "B>........$WWW.B" +
            "BWWW^^WWWWWWWW.B" +
            "Bv#####vv......B" +
            "B..............B" +
            "B...^.......^..B" +
            "B...#######.W.TB" +
            "B.........#.W..B" +
            "B.........#^W^^B" +
            "BMMMMM....MMMMMB";
        public const string Prefab4 =
            "MMMMMMM..MMMMMMM" +
            "M.....MS.....<MM" +
            "M..^..MMMMM..<MM" +
            "M..M$....v...<MM" +
            "M..MMMMM...MMMMM" +
            "M...v..MMMMM.N.M" +
            "M..............M" +
            "M...$.....MMMMMM" +
            "MMMMMMM..MMMMMMM";

        public static Prefab[] Prefabs = new Prefab[]
        {
            new Prefab { data = Prefab1, height = 12 },
            new Prefab { data = Prefab2, height = 12 },
            new Prefab { data = Prefab3, height = 14 },
            new Prefab { data = Prefab4, height = 9 },
        };

        public static eTileType CharToTileType(char chr)
        {
            switch (chr)
            {
                case 'N': // Broom Stick Spawner
                case 'P': // Pumpkin Spawner
                case 'S': // Player Spawn
                case '.': return eTileType.None;
                case '#': return eTileType.Block;
                case 'T': return eTileType.JumpThrough;
                case '^': return eTileType.Deadly_UP;
                case '>': return eTileType.Deadly_RIGHT;
                case '<': return eTileType.Deadly_LEFT;
                case 'v': return eTileType.Deadly_DOWN;
                case 'B': return eTileType.Brick;
                case 'M': return eTileType.Stone;
                case 'W': return eTileType.WhiteBrick;
                case '$': return eTileType.Spring;
                default: throw new Exception("unknown character '" + chr + "'!");
            }
        }

        public static Prefab GetRandomPrefab(Random r, ref int exclude)
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
