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
    }
}
