using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeffJamGame
{
    public class Hax
    {
        public static Texture2D LoadTexture2D(GraphicsDevice gd, string an)
        {
            Stream stream = new FileStream(an, FileMode.Open);
            Texture2D result = Texture2D.FromStream(gd, stream);
            stream.Close();
            return result;
        }

        public static float Elapsed(GameTime gt)
        {
            return (float) gt.ElapsedGameTime.TotalSeconds;
        }

        public static float UnboundedLerp(float a, float b, float perc)
        {
            return a + (b - a) * perc;
        }

        public static float Lerp(float a, float b, float perc)
        {
            if (perc >= 1.0f) return b;
            if (perc <= 0.0f) return a;
            return a + (b - a) * perc;
        }

        public static void SetFloatWithTarget(ref float outDest, float target, float amount)
        {
            if (outDest > target)
            {
                // make it less
                outDest -= amount;
                if (outDest < target)
                    outDest = target;
            }
            else
            {
                // make it more
                outDest += amount;
                if (outDest > target)
                    outDest = target;
            }
        }
    }
}
