using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheDeepEnd
{
    public enum eTimerState
    {
        NotRunning,
        Running,
        Finished
    }
    public class Hax
    {
        public static SoundEffect LoadSoundEffect(string path, bool isStereo = false)
        {
            return new SoundEffect(File.ReadAllBytes(path), 44100, isStereo ? AudioChannels.Stereo : AudioChannels.Mono);
        }

        public static Texture2D LoadTexture2D(GraphicsDevice gd, string an)
        {
            Stream stream = new FileStream(an, FileMode.Open);
            Texture2D result = Texture2D.FromStream(gd, stream);
            stream.Close();
            return result;
        }

        public static eTimerState Timer(ref float timer, GameTime gt)
        {
            if (timer > 0)
            {
                timer -= Elapsed(gt);
                if (timer < 0)
                {
                    timer = 0;
                    return eTimerState.Finished;
                }

                return eTimerState.Running;
            }

            return eTimerState.NotRunning;
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

        public static bool IsDeadly(eCollisionType ct)
        {
            switch (ct)
            {
                case eCollisionType.Deadly:
                case eCollisionType.DeadlyUp:
                case eCollisionType.DeadlyDown:
                case eCollisionType.DeadlyLeft:
                case eCollisionType.DeadlyRight:
                    return true;
            }

            return false;
        }
        public static Color HSVToRGB(float a1, float a2, float a3)
        {
            int rOut, gOut, bOut;
	        if (a2 <= 0.0f)
	        {
		        rOut = gOut = bOut = (int)(a3 * 255);
	        }
	        else if (a1 < 360.0f)
	        {
		        int   v10 = (int)(a1 / 60);
		        float v11 = a1 / 60 - v10;

		        float v6  = (1.0f - a2) * a3;
		        float v14 = (1.0f - (a2 * v11)) * a3;
		        float v12 = (1.0f - ((1.0f - v11) * a2)) * a3;

		        switch (v10)
		        {
		            case 0:
			            rOut = (int)(a3  * 255);
			            gOut = (int)(v12 * 255);
			            bOut = (int)(v6  * 255);
			            return Color.FromNonPremultiplied(rOut, gOut, bOut, 255);
		            case 1:
			            rOut = (int)(v14 * 255);
			            gOut = (int)(a3  * 255);
			            bOut = (int)(v6  * 255);
			            return Color.FromNonPremultiplied(rOut, gOut, bOut, 255);
		            case 2:
			            rOut = (int)(v6  * 255);
			            gOut = (int)(a3  * 255);
			            bOut = (int)(v12 * 255);
			            return Color.FromNonPremultiplied(rOut, gOut, bOut, 255);
		            case 3:
			            rOut = (int)(v6  * 255);
			            gOut = (int)(v14 * 255);
			            bOut = (int)(a3  * 255);
			            return Color.FromNonPremultiplied(rOut, gOut, bOut, 255);
		            case 4:
			            rOut = (int)(v12 * 255);
			            gOut = (int)(v6  * 255);
			            bOut = (int)(a3  * 255);
			            return Color.FromNonPremultiplied(rOut, gOut, bOut, 255);
		            default:
			            rOut = (int)(a3  * 255);
			            gOut = (int)(v6  * 255);
			            bOut = (int)(v14 * 255);
			            return Color.FromNonPremultiplied(rOut, gOut, bOut, 255);
		        }
	        }
	        else
	        {
		        float v6 = (1.0f - a2) * a3;

		        rOut = (int)(a3 * 255);
		        gOut = (int)(v6 * 255);
		        bOut = (int)(v6 * 255);
	        }

            return Color.FromNonPremultiplied(rOut, gOut, bOut, 255);
        }

        public static float Smoothstep(float x)
        {
            return x * x * (3 - 2 * x);
        }
    }
}
