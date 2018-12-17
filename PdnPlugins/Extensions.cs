using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdnPlugins
{
    public static class Extensions
    {
        public static int Convolution(Surface src, int[][] kernel, int x, int y)
        {
            if (x < 1 || y < 1 || x > src.Width - 2 || y > src.Height - 2)
            {
                return 0;
            }

            int c = 0;
            for(int i=0; i<3; i++)
                for (int j = 0; j < 3; j++)
                {
                    c += kernel[i][j] * src[x + 1 - i, y + 1 - j].Luminance();
                }
            return c;
        }

        public static byte Luminance(this ColorBgra pixel)
        {
            return (byte) Math.Round(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
        }

    }
}
