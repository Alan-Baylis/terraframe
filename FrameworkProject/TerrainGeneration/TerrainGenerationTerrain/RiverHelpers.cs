using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_HeightMap
{
    class RiverHelpers
    {
        private const int CELL_SIZE = 8;

        public static Vector[][] getVelocityField(float?[][] heightMap)
        {
            Vector[][] waterVectorField = new Vector[heightMap.Length][];

            for(int x = 0; x < heightMap.Length; x++)
            {
                waterVectorField[x] = new Vector[heightMap[x].Length];
                for (int y = 0;  y < heightMap[x].Length; y++)
                {
                    waterVectorField[x][y] = getCellVector(heightMap, x, y);
                }
            }

            return waterVectorField;
        }

        //public static TerrainOutputImage getVectorFieldImage(Vector[][] vectorField)
        //{
        //    int width = CELL_SIZE * vectorField.Length;
        //    int height = CELL_SIZE * vectorField[0].Length;

        //    Pen linePen = new Pen(Color.Black, 1);
        //    Pen directionPen = new Pen(Color.Red, 1);
        //    Bitmap bitmap = new Bitmap(width, height);
        //    Graphics g = Graphics.FromImage(bitmap);
        //    g.SmoothingMode = SmoothingMode.HighQuality;
        //    g.Clear(Color.White);

        //    for(int x = 0; x < vectorField.Length; x++)
        //    {
        //        for (int y = 0; y < vectorField[x].Length; y++)
        //        {
        //            drawVector(x * CELL_SIZE + CELL_SIZE/2, y * CELL_SIZE + CELL_SIZE/2, vectorField[x][y], g, linePen, directionPen);
        //        }
        //    }

        //    TerrainOutputImage vectorFieldOutputImage = new TerrainOutputImage();
        //    vectorFieldOutputImage.Title = "Vector field";
        //    vectorFieldOutputImage.Key = "vectorField";

        //    vectorFieldOutputImage.ImageData = new int[bitmap.Width][];

        //    for (int i = 0; i < bitmap.Width; i++)
        //    {
        //        vectorFieldOutputImage.ImageData[i] = new int[bitmap.Height];
        //        for (int j = 0; j < bitmap.Height; j++)
        //        {
        //            Color color = bitmap.GetPixel(i, j);
        //            vectorFieldOutputImage.ImageData[i][j] = (color.R << 24) | (color.G << 16) | (color.B << 8) | 0xff;
        //        }
        //    }
        //    return vectorFieldOutputImage;
        //}

        //private static void drawVector(int posX, int posY, Vector vector, Graphics g, Pen linePen, Pen directionPen)
        //{
        //    float amplification = (CELL_SIZE/2)-1;

        //    GraphicsState state = g.Save();

        //    g.TranslateTransform(posX, posY);
        //    g.DrawLine(directionPen, 0, 0, vector.Vx * amplification, vector.Vy * amplification);
        //    g.DrawLine(linePen, 0, 0, vector.Vx * -amplification, vector.Vy * -amplification);

        //    g.Restore(state);
        //}

        private static void getLowestNeighbour(float?[][] heightMap, int x, int y, out int finalX, out int finalY)
        {
            float? currentHeight = 9999;
            int nX;
            int nY;
            finalX = x;
            finalY = y;

            float? neighbourHeight = TerrainHelpers.getLeft(heightMap, x, y, out nX, out nY);
            if(neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getLeftLeftTop(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getTopLeft(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getTopTopLeft(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getTop(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getTopTopRight(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getTopRight(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getRightRightTop(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getRight(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getRightRightBottom(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getBottomRight(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getBottomBottomRight(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getBottom(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getBottomBottomLeft(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getBottomLeft(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }

            neighbourHeight = TerrainHelpers.getLeftLeftBottom(heightMap, x, y, out nX, out nY);
            if (neighbourHeight != null && neighbourHeight < currentHeight)
            {
                currentHeight = neighbourHeight;
                finalX = nX;
                finalY = nY;
            }
        }

        private static Vector getCellVector(float?[][] heightMap, int x, int y)
        {
            float currentVx = 0;
            float currentVy = 0;

            int nX;
            int nY;

            List<Vector> vectors = new List<Vector>();

            float? currentValue = TerrainHelpers.getTopLeft(heightMap, x, y, out nX, out nY);
            if(currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentValue = (float)Math.Cos(Math.PI / 4) * currentValue;
                currentVx = -(float)currentValue;
                currentVy = -(float)currentValue;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getLeft(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentVx = -(float)currentValue;
                currentVy = 0;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getBottomLeft(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentValue = (float)Math.Cos(Math.PI / 4) * currentValue;
                currentVx = -(float)currentValue;
                currentVy = (float)currentValue;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getTop(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentVx = 0;
                currentVy = -(float)currentValue;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getBottom(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentVx = 0;
                currentVy = (float)currentValue;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getTopRight(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentValue = (float)Math.Cos(Math.PI / 4) * currentValue;
                currentVx = (float)currentValue;
                currentVy = -(float)currentValue;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getRight(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentVx = (float)currentValue;
                currentVy = 0;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getBottomRight(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                currentValue = (float)Math.Cos(Math.PI / 4) * currentValue;
                currentVx = (float)currentValue;
                currentVy = (float)currentValue;
                vectors.Add(new Vector(currentVx, currentVy));
            }









            currentValue = TerrainHelpers.getTopTopLeft(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = -smaller;
                currentVy = -larger;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getLeftLeftTop(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = -larger;
                currentVy = -smaller;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getTopTopRight(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = smaller;
                currentVy = -larger;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getRightRightTop(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = larger;
                currentVy = -smaller;
                vectors.Add(new Vector(currentVx, currentVy));
            }









            currentValue = TerrainHelpers.getRightRightBottom(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = larger;
                currentVy = smaller;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getBottomBottomRight(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = smaller;
                currentVy = larger;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getBottomBottomLeft(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = -smaller;
                currentVy = larger;
                vectors.Add(new Vector(currentVx, currentVy));
            }

            currentValue = TerrainHelpers.getLeftLeftBottom(heightMap, x, y, out nX, out nY);
            if (currentValue != null && currentValue < heightMap[x][y])
            {
                currentValue = heightMap[x][y] - currentValue;
                float larger = (float)(Math.Cos(Math.Atan(0.5)) * currentValue);
                float smaller = (float)(Math.Sin(Math.Atan(0.5)) * currentValue);
                currentVx = -larger;
                currentVy = smaller;
                vectors.Add(new Vector(currentVx, currentVy));
            }






            Vector finalVector = new Vector(0, 0);
            foreach(Vector vector in vectors)
            {
                finalVector.Vx += vector.Vx;
                finalVector.Vy += vector.Vy;
            }

            finalVector.normalize();

            return finalVector;
        } 

        public static List<GeoPoint> traceRivers(float?[][] gaussHeightMap, List<List<GeoPoint>> ridges, int riverParticlesCount, Random random)
        {
            List<GeoPoint> availableGeoPoints = new List<GeoPoint>();
            foreach (List<GeoPoint> ridge in ridges) {
                availableGeoPoints.AddRange(ridge);
            }

            Dictionary<string, GeoPoint> riverSources = new Dictionary<string, GeoPoint>();
            while(riverSources.Count <= riverParticlesCount)
            {
                GeoPoint foundGeoPoint = availableGeoPoints[random.Next(availableGeoPoints.Count)];
                string geoPointSignature = "" + foundGeoPoint.X + "#" + foundGeoPoint.Y;
                if (!riverSources.ContainsKey(geoPointSignature))
                {
                    riverSources.Add(geoPointSignature, foundGeoPoint);
                } 
            }

            List<GeoPoint> allRiverPoints = new List<GeoPoint>();

            foreach(GeoPoint riverSource in riverSources.Values)
            {
                HashSet<string> riverSignatures = new HashSet<string>();
                List<GeoPoint> river = new List<GeoPoint>();
                traceRiver(gaussHeightMap, riverSource, river, riverSignatures, 0);

                allRiverPoints.AddRange(river);
            }

            return allRiverPoints;
        }

        public static void traceRiver(float?[][] gaussHeightMap, GeoPoint riverSource, List<GeoPoint> river, HashSet<string> riverSignatures, int iterationCount)
        {
            Vector cellVector = getCellVector(gaussHeightMap, riverSource.X, riverSource.Y);

            float angle = getVectorAngle(cellVector);
            Console.WriteLine("x: " + cellVector.Vx + "   y: " + cellVector.Vy + "   angle: " + angle);

            float pie = (float)(2 * Math.PI) / 32;
            float sidePie = pie * 2;
            angle -= pie;

            GeoPoint geoPoint1 = null;
            GeoPoint geoPoint2 = null;

            if(angle < sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X+1,riverSource.Y);
            }

            //split
            else if(angle < 2 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X + 2, riverSource.Y + 1);
                geoPoint2 = new GeoPoint(riverSource.X + 1, riverSource.Y);
            }
            else if (angle < 2 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X + 2, riverSource.Y + 1);
                geoPoint2 = new GeoPoint(riverSource.X + 1, riverSource.Y + 1);
            }


            else if(angle < 3 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X+1, riverSource.Y+1);
            }

            //split
            else if (angle < 4 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X + 1, riverSource.Y + 2);
                geoPoint2 = new GeoPoint(riverSource.X + 1, riverSource.Y + 1);
            }
            else if (angle < 4 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X + 1, riverSource.Y + 2);
                geoPoint2 = new GeoPoint(riverSource.X, riverSource.Y + 1);
            }

            else if (angle < 5 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X, riverSource.Y+1);
            }

            //split
            else if (angle < 6 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 1, riverSource.Y + 2);
                geoPoint2 = new GeoPoint(riverSource.X, riverSource.Y + 1);
            }
            else if (angle < 6 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 1, riverSource.Y + 2);
                geoPoint2 = new GeoPoint(riverSource.X - 1, riverSource.Y + 1);
            }

            else if (angle < 7 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X-1, riverSource.Y+1);
            }

            //split
            else if (angle < 8 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 2, riverSource.Y + 1);
                geoPoint2 = new GeoPoint(riverSource.X - 1, riverSource.Y + 1);
            }
            else if (angle < 8 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 2, riverSource.Y + 1);
                geoPoint2 = new GeoPoint(riverSource.X - 1, riverSource.Y);
            }

            else if (angle < 9 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X-1, riverSource.Y);
            }

            //split
            else if (angle < 10 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 2, riverSource.Y - 1);
                geoPoint2 = new GeoPoint(riverSource.X - 1, riverSource.Y);
            }
            else if (angle < 10 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 2, riverSource.Y - 1);
                geoPoint2 = new GeoPoint(riverSource.X - 1, riverSource.Y - 1);
            }

            else if (angle < 11 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X-1, riverSource.Y-1);
            }

            //split
            else if (angle < 12 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 1, riverSource.Y - 2);
                geoPoint2 = new GeoPoint(riverSource.X - 1, riverSource.Y - 1);
            }
            else if (angle < 12 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X - 1, riverSource.Y - 2);
                geoPoint2 = new GeoPoint(riverSource.X, riverSource.Y - 1);
            }

            else if (angle < 13 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X, riverSource.Y-1);
            }

            //split
            else if (angle < 14 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X + 1, riverSource.Y - 2);
                geoPoint2 = new GeoPoint(riverSource.X, riverSource.Y - 1);
            }
            else if (angle < 14 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X + 1, riverSource.Y - 2);
                geoPoint2 = new GeoPoint(riverSource.X + 1, riverSource.Y - 1);
            }

            else if (angle < 15 * sidePie)
            {
                geoPoint1 = new GeoPoint(riverSource.X+1, riverSource.Y-1);
            }

            //split
            else if (angle < 16 * sidePie - pie)
            {
                geoPoint1 = new GeoPoint(riverSource.X + 2, riverSource.Y - 1);
                geoPoint2 = new GeoPoint(riverSource.X + 1, riverSource.Y - 1);
            }
            else
            {
                geoPoint1 = new GeoPoint(riverSource.X + 2, riverSource.Y - 1);
                geoPoint2 = new GeoPoint(riverSource.X + 1, riverSource.Y);
            }






            if (geoPoint1 != null && geoPoint1.X >= 0 && geoPoint1.X < gaussHeightMap.Length
               && geoPoint1.Y >= 0 && geoPoint1.Y < gaussHeightMap.Length)
            {
                string signature = "" + geoPoint1.X + "#" + geoPoint1.Y;
                if (!riverSignatures.Contains(signature))
                {
                    river.Add(geoPoint1);
                    geoPoint1.Height = (float)gaussHeightMap[geoPoint1.X][geoPoint1.Y];
                    riverSignatures.Add(signature);
                }
                else
                {
                    //return;
                }
            }
            else
            {
                return;
            }

            if (geoPoint2 != null && geoPoint2.X >= 0 && geoPoint2.X < gaussHeightMap.Length
                && geoPoint2.Y >= 0 && geoPoint2.Y < gaussHeightMap.Length)
            {
                river.Add(geoPoint2);
                geoPoint2.Height = (float)gaussHeightMap[geoPoint2.X][geoPoint2.Y];
            }

            iterationCount++;

            if (iterationCount > gaussHeightMap.Length * 3)
                return;

            traceRiver(gaussHeightMap, geoPoint1, river, riverSignatures, iterationCount);
        }

        private static float getVectorAngle(Vector vector)
        {
            double angle = Math.PI/2 - Math.Atan2(vector.Vx, vector.Vy);
            if(angle < 0)
            {
                angle = (2 * Math.PI + angle);
            }

            return (float)angle; 
        }

        public static void addRiverDeformations(float?[][] heightMap, List<GeoPoint> riverPoints)
        {
            foreach(GeoPoint riverPoint in riverPoints)
            {

            }
        }
    }

    class Vector
    {
        public float Vx { get; set; }
        public float Vy { get; set; }
        public Vector(float vx, float vy)
        {
            Vx = vx;
            Vy = vy;
        }

        public void normalize()
        {
            if (Vx != 0 && Vy != 0)
            {
                float length = (float)Math.Sqrt(Math.Pow(Vx, 2) + Math.Pow(Vy, 2));

                Vx = Vx / length;
                Vy = Vy / length;
            }
        }
    }
}
