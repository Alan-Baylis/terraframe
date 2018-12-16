//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Linq;
//using System.Text;
//using TerrainGeneration_PluginBase;

//namespace TerrainGeneration_Vegetation
//{
//    class Turtle
//    {
//        public static TerrainOutputImage drawProduction(string production, int width, int height, int originX, int originY, float strokeX, float strokeY, int angle)
//        {
//            Bitmap bitmap = new Bitmap(width, height);
//            Graphics graphics = Graphics.FromImage(bitmap);

//            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

//            graphics.Clear(Color.White);
//            Pen pen = new Pen(Color.Green, 1);
//            Stack<GraphicsState> states = new Stack<GraphicsState>();

//            graphics.TranslateTransform(0, height);
//            graphics.ScaleTransform(1, -1);
//            graphics.TranslateTransform(originX, originY);

//            foreach (char c in production)
//            {
//                switch (c)
//                {
//                    case 'F':
//                        graphics.DrawLine(pen, 0, 0, strokeX, strokeY);
//                        graphics.TranslateTransform(strokeX, strokeY);
//                        break;
//                    case 'X':
//                        break;
//                    case '+':
//                        graphics.RotateTransform(-angle);
//                        break;
//                    case '-':
//                        graphics.RotateTransform(angle);
//                        break;
//                    case '[':
//                        states.Push(graphics.Save());
//                        break;
//                    case ']':
//                        graphics.Restore(states.Pop());
//                        break;
//                }
//            }

//            TerrainOutputImage terrainOutputHeightMap = new TerrainOutputImage();
//            terrainOutputHeightMap.Title = "Creation";
//            terrainOutputHeightMap.Key = "creation";

//            terrainOutputHeightMap.ImageData = new int[bitmap.Width][];

//            for (int i = 0; i < bitmap.Width; i++)
//            {
//                terrainOutputHeightMap.ImageData[i] = new int[bitmap.Height];
//                for (int j = 0; j < bitmap.Height; j++)
//                {
//                    Color color = bitmap.GetPixel(i, j);
//                    terrainOutputHeightMap.ImageData[i][j] = (color.R << 24) | (color.G << 16) | (color.B << 8) | 0xff;
//                }
//            }
//            return terrainOutputHeightMap;
//        }
//    }
//}
