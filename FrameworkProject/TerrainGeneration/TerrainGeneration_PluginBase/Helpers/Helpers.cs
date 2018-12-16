using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGeneration_Core
{
    public abstract class Helpers
    {
        private static Helpers helpers;
        public static Helpers Instance { get { return helpers; } set { helpers = value; } }

        public abstract int[][] ResizePixels(int[][] pixels, int w1, int h1, int w2, int h2);
        public abstract void SaveImageFile(string filename, int[][] imageData);
    }
}
