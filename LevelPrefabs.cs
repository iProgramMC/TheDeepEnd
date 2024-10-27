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
            "W..#...#####..CW" +
            "WC.............W" +
            "WTT..^TWWWWT...W" +
            "W^^^^WWWv......W" +
            "WWWWWWW......^^W" +
            "WWWWWW....WWWWWW";
        public const string Prefab2 =
            "BMMMMMM..MMMMMMB" +
            "B.......S......B" +
            "B.#>...TT...<#.B" +
            "B.#>...^^...<#.B" +
            "BC#>..<##>..<#CB" +
            "B.#>........<#.B" +
            "B.TTT.^^^^.TTT.B" +
            "B.....####.....B" +
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
            "B.......C.#.W..B" +
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
            "MC..$.....MMMMMM" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab5 =
            "MMMMMMM..MMMMMMM" +
            "MC.....S...#v#.M" +
            "M..^..TTTT.....M" +
            "M.<#>.^^^^..#..M" +
            "M..v..#######..M" +
            "M......vv......M" +
            "MTT..........TTM" +
            "M^^^........^^^M" +
            "MMMMMM....MMMMMM" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab6 =
            "MMMMMMM..MMMMMMM" +
            "M......S.......M" +
            "M.###########..M" +
            "M.#.........#.<M" +
            "M.#T#######.#..M" +
            "M.#.#..#>...#..M" +
            "M.#T#.....#T#>.M" +
            "M.#.#..####.#..M" +
            "M..$#C....#$..PM" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab7 =
            "MMMMMMM..MMMMMMM" +
            "M#>....S.....<#M" +
            "M....^####^....M" +
            "M..MMMMMMMMMM..M" +
            "M..##......##>.M" +
            "M.<##.#..#T##..M" +
            "M....T#..#....#M" +
            "M..C..#..##>P..M" +
            "M....$#..##>...M" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab8 =
            "MMMMMMM..MMMMMMM" +
            "M......S.MMMMMMM" +
            "M......MMMM...PM" +
            "M.....MM.......M" +
            "M....MM........M" +
            "M...MM...M....^M" +
            "M..MM...MM...^MM" +
            "M..M...MM...^MMM" +
            "M..C..MM...^MMMM" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab9 =
            "MMMMMMM..MMMMMMM" +
            "M>....MS.......M" +
            "M>.^..MMMMMMMM.M" +
            "M>.M.....v...v.M" +
            "M>.MMMMM...M...M" +
            "M>..MMMMMMMMMMMM" +
            "MMM...........<M" +
            "MMMMM^^MMMMM..<M" +
            "MMMMMMM.......<M" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab10 =
            "MMMMMMM..MMMMMMM" +
            "M..............M" +
            "M.C..C....C..C.M" +
            "M..S...........M" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab11 =
            "MMMMMMM..MMMMMMM" +
            "M.S.......<MMMMM" +
            "MMMMM>.CC.<MvvvM" +
            "M...M>....<M.P.M" +
            "M...M>....<M...M" +
            "M...M>.^^.<M...M" +
            "M......BB....TTM" +
            "M...BBBBBBBB...M" +
            "M..BB..vv..BB..M" +
            "M..............M" +
            "M....TB..BT.N..M" +
            "M..C..B..B....MM" +
            "M....$B..B$.^.MM" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab12 =
            "MMMMMMMS.MMMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM.C..MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM.C..M...MM" +
            "MMMMMM......P.MM" +
            "MMMMMM....M...MM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM.C..MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM.C..MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMM....MMMMMM" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab13 =
            "MMMM...S.MMMMMMM" +
            "M....MMMMMMMMMMM" +
            "M.MMM........MMM" +
            "M.M............M" +
            "M.M..M^..^M....M" +
            "M.MT.<M..M>...TM" +
            "M.M..<M..M>....M" +
            "M.M..<M..M>..P.M" +
            "M..$.<M..M>...$M" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab14 =
            "MMMMMMM..MMMMMMM" +
            "M......S......<M" +
            "M.....MMMM.....M" +
            "M..MMMMvvMMMMM.M" +
            "M...........vv.M" +
            "M..^..M..M.....M" +
            "MMMMMMM..MMMMMMM";
        public const string Prefab15 =
            "MMMMMMM..MMMMMMM" +
            "MM>.....SM.N...M" +
            "MM>..TMMMMMMMM.M" +
            "MMM^...........M" +
            "MMMMMMM..MM^^MMM" +
            "M.........MMMM.M" +
            "M>....^.......<M" +
            "M>..MMMMMMMM..<M" +
            "M>...C.....C..<M" +
            "MMMMMMM..MMMMMMM";

        public static Prefab[] Prefabs = new Prefab[]
        {
            new Prefab { data = Prefab1, height = 12 },
            new Prefab { data = Prefab2, height = 12 },
            new Prefab { data = Prefab3, height = 14 },
            new Prefab { data = Prefab4, height = 9 },
            new Prefab { data = Prefab5, height = 10 },
            new Prefab { data = Prefab6, height = 10 },
            new Prefab { data = Prefab7, height = 10 },
            new Prefab { data = Prefab8, height = 10 },
            new Prefab { data = Prefab9, height = 10 },
            new Prefab { data = Prefab10, height = 5 }, // secret bonus !
            new Prefab { data = Prefab11, height = 14 },
            new Prefab { data = Prefab12, height = 20 },
            new Prefab { data = Prefab13, height = 10 },
            new Prefab { data = Prefab14, height = 7 },
            new Prefab { data = Prefab15, height = 10 },
        };

        public static eTileType CharToTileType(char chr)
        {
            switch (chr)
            {
                case 'N': // Broom Stick Spawner
                case 'P': // Pumpkin Spawner
                case 'C': // Candy Spawner
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
            if (exclude == -1) exclude = 0;
            int chosen = (exclude + Prefabs.Length - 1) % Prefabs.Length;
            exclude = chosen;
            return Prefabs[chosen]; 
        
            //int chosen;
            //do {
                //chosen = r.Next(Prefabs.Length);
            //}
            //while (chosen == exclude && Prefabs.Length > 1);
            //exclude = chosen;
            //return Prefabs[chosen];
        }
    }
}
