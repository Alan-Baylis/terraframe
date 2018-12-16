using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_HeightMap
{
    class TerrainHelpers
    {
        private static Random random;

        public static void generateTerrain(float?[][] heightMap, Random random, float[][] roughness, List<GeoPoint> riverPoints, float?[][] gaussHeight)
        {
            TerrainHelpers.random = random;
            int squareSize = heightMap.Length;

            setInitialValues(heightMap);

            //fillSquares(heightMap, heightMap.length-1, 1.0f);
            Dictionary<string, GeoPoint> riverPointsMap = new Dictionary<string, GeoPoint>();
            foreach (GeoPoint riverPoint in riverPoints)
            {
                string signature = "" + riverPoint.X + "#" + riverPoint.Y;
                //riverPointsMap.Add(signature, riverPoint);
            }


            inverseMD(heightMap, riverPointsMap, gaussHeight);
            fillSquares(heightMap, heightMap.Length - 1, 1.0f, roughness);
        }

        public static GeoPoint[][] calculateDistanceToRidges(float?[][] heightMap)
        {
            GeoPoint[][] geoPoints = new GeoPoint[heightMap.Length][];
            for (int i = 0; i < heightMap.Length; i++)
                geoPoints[i] = new GeoPoint[heightMap.Length];
            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap.Length; y++)
                {
                    GeoPoint geoPoint = new GeoPoint();
                    geoPoint.X = x;
                    geoPoint.Y = y;
                    if (heightMap[x][y] != null)
                    {
                        geoPoint.Ridge = true;
                        geoPoint.Height = (float)heightMap[x][y];
                    }
                    geoPoints[x][y] = geoPoint;
                }
            }

            calculateDistanceRidges(geoPoints);
            //smooth(geoPoints);
            //smooth2(geoPoints, 1000);
            return geoPoints;
        }

        private static void fillSquares(float?[][] heightMap, int lateralSpace, float maxHeightDifference, float[][] roughness)
        {
            int side = heightMap.Length;
            int halfLateralSpace = lateralSpace / 2;

            for (int x = halfLateralSpace; x < side; x += lateralSpace)
            {
                for (int y = 0; y < side; y += lateralSpace)
                {
                    if (heightMap[x][y] == null)
                    {
                        float meanHeight = (float)(heightMap[x - halfLateralSpace][y] + heightMap[x + halfLateralSpace][y]) / 2.0f;
                        heightMap[x][y] = addRandomHeight(meanHeight, maxHeightDifference*roughness[x][y]);
                    }
                }
            }

            for (int x = 0; x < side; x += lateralSpace)
            {
                for (int y = halfLateralSpace; y < side; y += lateralSpace)
                {
                    if (heightMap[x][y] == null)
                    {
                        float meanHeight = (float)(heightMap[x][y - halfLateralSpace] + heightMap[x][y + halfLateralSpace]) / 2.0f;
                        heightMap[x][y] = addRandomHeight(meanHeight, maxHeightDifference*roughness[x][y]);
                    }
                }
            }

            for (int x = halfLateralSpace; x < side; x += lateralSpace)
            {
                for (int y = halfLateralSpace; y < side; y += lateralSpace)
                {
                    if (heightMap[x][y] == null)
                    {
                        float meanHeight = (float)(heightMap[x - halfLateralSpace][y] + heightMap[x + halfLateralSpace][y] + heightMap[x][y - halfLateralSpace] + heightMap[x][y + halfLateralSpace]) / 4.0f;
                        heightMap[x][y] = addRandomHeight(meanHeight, maxHeightDifference*roughness[x][y]);
                    }
                }
            }

            if (lateralSpace > 2)
                fillSquares(heightMap, lateralSpace / 2, maxHeightDifference * 0.55f, roughness);
        }

        private static void fillSquares_(float[][] heightMap, int lateralSpace, float maxHeightDifference)
        {
            int side = heightMap.Length;
            int halfLateralSpace = lateralSpace / 2;

            for (int x = halfLateralSpace; x < side; x += lateralSpace)
            {
                for (int y = halfLateralSpace; y < side; y += lateralSpace)
                {
                    float meanHeight = (heightMap[x - halfLateralSpace][y - halfLateralSpace] + heightMap[x + halfLateralSpace][y - halfLateralSpace] + heightMap[x - halfLateralSpace][y + halfLateralSpace] + heightMap[x + halfLateralSpace][y + halfLateralSpace]) / 4.0f;
                    heightMap[x][y] = addRandomHeight(meanHeight, maxHeightDifference);
                }
            }


            for (int x = halfLateralSpace; x < side; x += lateralSpace)
            {
                for (int y = 0; y < side; y += lateralSpace)
                {
                    float meanCount = 2;
                    float meanHeight = heightMap[x - halfLateralSpace][y] + heightMap[x + halfLateralSpace][y];

                    if (y > 0)
                    {
                        meanHeight += heightMap[x][y - halfLateralSpace];
                        meanCount++;
                    }

                    if (y < side - 1)
                    {
                        meanHeight += heightMap[x][y + halfLateralSpace];
                        meanCount++;
                    }

                    meanHeight = meanHeight / meanCount;

                    heightMap[x][y] = addRandomHeight(meanHeight, maxHeightDifference);
                }
            }

            for (int x = 0; x < side; x += lateralSpace)
            {
                for (int y = halfLateralSpace; y < side; y += lateralSpace)
                {
                    float meanCount = 2;
                    float meanHeight = heightMap[x][y - halfLateralSpace] + heightMap[x][y + halfLateralSpace];

                    if (x > 0)
                    {
                        meanHeight += heightMap[x - halfLateralSpace][y];
                        meanCount++;
                    }

                    if (x < side - 1)
                    {
                        meanHeight += heightMap[x + halfLateralSpace][y];
                        meanCount++;
                    }

                    meanHeight = meanHeight / meanCount;

                    heightMap[x][y] = addRandomHeight(meanHeight, maxHeightDifference);
                }
            }

            if (lateralSpace > 2)
                fillSquares_(heightMap, lateralSpace / 2, maxHeightDifference * 0.6f);
        }

        private static float addRandomHeight(float meanHeight, float maxHeightDifference)
        {
            float height = (float)Math.Abs(meanHeight + (random.NextDouble() * maxHeightDifference - maxHeightDifference / 2.0f));

            if (height > 1.0f)
                return 1;
            if (height < 0)
                return 0;
            return height;
            //return meanHeight;
        }

        private static void setInitialValues(float?[][] heightMap)
        {
            int squareSize = heightMap.Length - 1;

            /*heightMap[0][0] = ((Double)random.nextDouble()).floatValue();
            heightMap[squareSize][0] = ((Double)random.nextDouble()).floatValue();
            heightMap[squareSize][squareSize] = ((Double)random.nextDouble()).floatValue();
            heightMap[0][squareSize] = ((Double)random.nextDouble()).floatValue();*/

            heightMap[0][0] = 0.0f;
            heightMap[squareSize][0] = 0.0f;
            heightMap[squareSize][squareSize] = 0.0f;
            heightMap[0][squareSize] = 0.0f;
        }

        private static void getParentListMD(List<GeoPoint>[][] parentList, int lateralSpace, float maxHeightDifference)
        {
            int side = parentList.Length;
            int halfLateralSpace = lateralSpace / 2;

            for (int x = halfLateralSpace; x < side; x += lateralSpace)
            {
                for (int y = 0; y < side; y += lateralSpace)
                {
                    parentList[x][y].Add(new GeoPoint(x - halfLateralSpace, y, lateralSpace, maxHeightDifference));
                    parentList[x][y].Add(new GeoPoint(x + halfLateralSpace, y, lateralSpace, maxHeightDifference));
                }
            }

            for (int x = 0; x < side; x += lateralSpace)
            {
                for (int y = halfLateralSpace; y < side; y += lateralSpace)
                {
                    parentList[x][y].Add(new GeoPoint(x, y - halfLateralSpace, lateralSpace, maxHeightDifference));
                    parentList[x][y].Add(new GeoPoint(x, y + halfLateralSpace, lateralSpace, maxHeightDifference));
                }
            }

            for (int x = halfLateralSpace; x < side; x += lateralSpace)
            {
                for (int y = halfLateralSpace; y < side; y += lateralSpace)
                {
                    parentList[x][y].Add(new GeoPoint(x - halfLateralSpace, y, lateralSpace, maxHeightDifference));
                    parentList[x][y].Add(new GeoPoint(x + halfLateralSpace, y, lateralSpace, maxHeightDifference));
                    parentList[x][y].Add(new GeoPoint(x, y - halfLateralSpace, lateralSpace, maxHeightDifference));
                    parentList[x][y].Add(new GeoPoint(x, y + halfLateralSpace, lateralSpace, maxHeightDifference));
                }
            }

            if (lateralSpace > 2)
                getParentListMD(parentList, halfLateralSpace, maxHeightDifference * 0.6f);
        }

        private static void inverseMD(float?[][] heightMap, Dictionary<string, GeoPoint> riverPointsMap, float?[][] gaussHeight)
        {
            List<GeoPoint>[][] parentList = (List<GeoPoint>[][])new List<GeoPoint>[heightMap.Length][];
            for (int i = 0; i < heightMap.Length; i++)
                parentList[i] = new List<GeoPoint>[heightMap.Length];
            float?[][] heightMapCopy = new float?[heightMap.Length][];
            for (int i = 0; i < heightMap.Length; i++)
                heightMapCopy[i] = new float?[heightMap.Length];

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    heightMapCopy[x][y] = heightMap[x][y];
                }
            }

            for (int x = 0; x < parentList.Length; x++)
            {
                for (int y = 0; y < parentList[x].Length; y++)
                {
                    parentList[x][y] = new List<GeoPoint>();
                }
            }

            getParentListMD(parentList, parentList.Length - 1, 1);

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    if (heightMapCopy[x][y] != null)
                    {
                        inversePointMD_(heightMap, parentList, parentList[x][y], heightMap[x][y], x, y, x, y, heightMap[x][y], riverPointsMap.ContainsKey(x+"#"+y), gaussHeight);
                    }
                }
            }
        }


        private static void inversePointMD__(float?[][] heightMap, List<GeoPoint>[][] parentList, List<GeoPoint> parents, float? height, int curX, int curY, int initX, int initY, float? initialHeight, bool isRiver, float?[][] gaussHeight)
        {
            if (parents.Count == 0)
                return;

            //float distanceToRidge = (float)Math.sqrt(Math.pow(curX-initX,2) + Math.pow(curY-initY,2));	
            //height = initialHeight - distanceToRidge/(heightMap.length/5.0f);
            //Float newHeights[] = divideHeightUniformlyRandomly(height, parents.size());

            int newHeightsIdx = 0;
            foreach (GeoPoint geoPoint in parents)
            {
                float distanceToRidge = (float)Math.Sqrt(Math.Pow(geoPoint.X - initX, 2) + Math.Pow(geoPoint.Y - initY, 2));
                if (isRiver)
                    height = gaussHeight[geoPoint.X][geoPoint.Y];//initialHeight + distanceToRidge / (heightMap.Length / 5.0f);
                else
                    height = gaussHeight[geoPoint.X][geoPoint.Y];//initialHeight - distanceToRidge / (heightMap.Length / 5.0f);

                bool changed = false;
                if (heightMap[geoPoint.X][geoPoint.Y] == null)
                {
                    changed = true;
                    heightMap[geoPoint.X][geoPoint.Y] = height;
                    geoPoint.DistanceToRidges = distanceToRidge;
                }
                else
                {
                    if (distanceToRidge < geoPoint.DistanceToRidges)
                    {
                        changed = true;
                        heightMap[geoPoint.X][geoPoint.Y] = height;
                        geoPoint.DistanceToRidges = distanceToRidge;
                    }
                }

                if (changed)
                {
                    if (heightMap[geoPoint.X][geoPoint.Y] < 0)
                        heightMap[geoPoint.X][geoPoint.Y] = 0.0f;
                    else if (heightMap[geoPoint.X][geoPoint.Y] > 1)
                        heightMap[geoPoint.X][geoPoint.Y] = 1.0f;
                }

                newHeightsIdx++;
                inversePointMD__(heightMap, parentList, parentList[geoPoint.X][geoPoint.Y], 0.0f, geoPoint.X, geoPoint.Y, initX, initY, initialHeight, isRiver, gaussHeight);
            }
        }


        private static void inversePointMD_(float?[][] heightMap, List<GeoPoint>[][] parentList, List<GeoPoint> parents, float? height, int curX, int curY, int initX, int initY, float? initialHeight, bool isRiver, float?[][] gaussHeight)
        {
            if (parents.Count == 0)
                return;

            //float distanceToRidge = (float)Math.sqrt(Math.pow(curX-initX,2) + Math.pow(curY-initY,2));	
            //height = initialHeight - distanceToRidge/(heightMap.length/5.0f);
            //Float newHeights[] = divideHeightUniformlyRandomly(height, parents.size());

            int newHeightsIdx = 0;
            foreach (GeoPoint geoPoint in parents)
            {
                float distanceToRidge = (float)Math.Sqrt(Math.Pow(geoPoint.X - initX, 2) + Math.Pow(geoPoint.Y - initY, 2));
                if (isRiver)
                    height = gaussHeight[geoPoint.X][geoPoint.Y];//initialHeight + distanceToRidge / (heightMap.Length / 5.0f);
                else
                    height = gaussHeight[geoPoint.X][geoPoint.Y];//initialHeight - distanceToRidge / (heightMap.Length / 5.0f);

                bool changed = false;
                if (heightMap[geoPoint.X][geoPoint.Y] == null)
                {
                    changed = true;
                    heightMap[geoPoint.X][geoPoint.Y] = height;
                    geoPoint.DistanceToRidges = distanceToRidge;
                }
                else
                {
                    if (distanceToRidge < geoPoint.DistanceToRidges)
                    {
                        changed = true;
                        heightMap[geoPoint.X][geoPoint.Y] = height;
                        geoPoint.DistanceToRidges = distanceToRidge;
                    }
                }

                if (changed)
                {
                    if (heightMap[geoPoint.X][geoPoint.Y] < 0)
                        heightMap[geoPoint.X][geoPoint.Y] = 0.0f;
                    else if (heightMap[geoPoint.X][geoPoint.Y] > 1)
                        heightMap[geoPoint.X][geoPoint.Y] = 1.0f;
                }

                newHeightsIdx++;
                inversePointMD_(heightMap, parentList, parentList[geoPoint.X][geoPoint.Y], 0.0f, geoPoint.X, geoPoint.Y, initX, initY, initialHeight, isRiver, gaussHeight);
            }
        }

        private static void inversePointMD(float?[][] heightMap, List<GeoPoint>[][] parentList, List<GeoPoint> parents, float? height, int initX, int initY, float initialHeight)
        {

            if (parents.Count == 0)
                return;

            //height = addRandomHeight(height, parents.get(0).getMaxHeightDifference());
            //height = height * parents.size()-parents.get(0).getMaxHeightDifference();
            //float distanceToRidge = (float)Math.sqrt(Math.pow(parents.get(0).getX()-initX,2) + Math.pow(parents.get(0).getY() - initY,2));		
            //height = initialHeight - (distanceToRidge*0.0001f);
            //Float newHeights[] = divideHeightUniformlyRandomly(height, parents.size());


            /*float existingHeightsSum = 0;
            float meanCount = 0;
            int parentIdx = 0;
            for( GeoPoint geoPoint : parents ){

                if(heightMap[geoPoint.getX()][geoPoint.getY()] != null){
                    height -= (heightMap[geoPoint.getX()][geoPoint.getY()]+newHeights[parentIdx];
                }else{
                    meanCount++;
                }

                parentIdx++;
            }*/

            //if(meanCount != 0){

            /*height = height/meanCount;
			
			if((height*parents.size())/meanCount < 0)
				System.out.println(height);
			height = (height*parents.size())/meanCount;
			if(height < 0){
				height = 0.0f;
				System.out.println("Less than 0: " + height);
			}
			if(height > 1){
				height = 1.0f;
				System.out.println("More than 1: " + height);
			}*/

            int newHeightsIdx = 0;
            foreach(GeoPoint geoPoint in parents)
            {

                float distanceToRidge = (float)Math.Sqrt(Math.Pow(geoPoint.X - initX, 2) + Math.Pow(geoPoint.Y - initY, 2));

                if (heightMap[geoPoint.X][geoPoint.Y] == null)
                {
                    //heightMap[geoPoint.getX()][geoPoint.getY()] = newHeights[newHeightsIdx];
                    heightMap[geoPoint.X][geoPoint.Y] = 1.0f - distanceToRidge / 513.0f;
                    geoPoint.DistanceToRidges = distanceToRidge;
                }
                else
                {
                    if (distanceToRidge < geoPoint.DistanceToRidges)
                    {
                        //heightMap[geoPoint.getX()][geoPoint.getY()] = newHeights[newHeightsIdx];
                        heightMap[geoPoint.X][geoPoint.Y] = 1.0f - distanceToRidge / 513.0f;
                        geoPoint.DistanceToRidges = distanceToRidge;
                    }
                    //heightMap[geoPoint.getX()][geoPoint.getY()] = (heightMap[geoPoint.getX()][geoPoint.getY()] + newHeights[newHeightsIdx])/2.0f;
                }

                if (heightMap[geoPoint.X][geoPoint.Y] < 0)
                    heightMap[geoPoint.X][geoPoint.Y] = 0.0f;
                else if (heightMap[geoPoint.X][geoPoint.Y] > 1)
                    heightMap[geoPoint.X][geoPoint.Y] = 1.0f;


                newHeightsIdx++;
            }
            //}

            foreach (GeoPoint geoPoint in parents)
            {
                inversePointMD(heightMap, parentList, parentList[geoPoint.X][geoPoint.Y], heightMap[geoPoint.X][geoPoint.Y], initX, initY, initialHeight);
            }
        }

        private static float?[] divideHeightUniformlyRandomly(float? height, int part)
        {
            float?[] uniformRandoms = new float?[part];

            /*Float mean = height / part;
            Float sum = 0.0f;

            for (int i=0; i<part / 2; i++) {
                uniformRandoms[i] = random.nextFloat() * mean;

                uniformRandoms[part - i - 1] = mean + random.nextFloat() * mean;

                sum += uniformRandoms[i] + uniformRandoms[part - i -1];
            }
            uniformRandoms[(int)Math.ceil(part/2)] = uniformRandoms[(int)Math.ceil(part/2)] + height - sum;*/

            for (int i = 0; i < part; i++)
            {
                uniformRandoms[i] = height / part;
            }

            return uniformRandoms;


        }

        private static void calculateDistanceRidges(GeoPoint[][] geoPoints)
        {
            List<GeoPoint> cellQueue = new List<GeoPoint>();
            List<GeoPoint> tempCellQueue = new List<GeoPoint>();
            List<GeoPoint> adjCells = new List<GeoPoint>();
            //int curDistance = 1;
            GeoPoint cell;

            for (int col = 0; col < geoPoints.Length; col++)
            {
                for (int line = 0; line < geoPoints.Length; line++)
                {
                    cell = geoPoints[col][line];

                    if (!cellQueue.Contains(cell) && landAndAdjacentToRidges(geoPoints, cell))
                    {
                        cell.DistanceToRidges = getDistance(cell.X, cell.Y, cell.NearestRidge.X, cell.NearestRidge.Y);
                        cell.Height = normalDistribution(cell.DistanceToRidges, 1.0f / 8.0f * geoPoints.Length, cell.NearestRidgeHeight);
                        cellQueue.Add(cell);
                    }
                }
            }

            while (cellQueue.Count != 0)
            {
                //curDistance++;

                //delete tempCellQueue;
                tempCellQueue = new List<GeoPoint>();

                while (cellQueue.Count != 0)
                {
                    adjCells = GeoPoint.getAdjacentCells(geoPoints, cellQueue[cellQueue.Count - 1]);

                    for (int i = 0; i < adjCells.Count; i++)
                    {
                        float distance = getDistance(adjCells[i].X, adjCells[i].Y, cellQueue[cellQueue.Count - 1].NearestRidge.X, cellQueue[cellQueue.Count - 1].NearestRidge.Y);
                        float height = normalDistribution(distance, 1.0f / 15.0f * geoPoints.Length, cellQueue[cellQueue.Count - 1].NearestRidgeHeight);

                        if (adjCells[i].ValidCell && (adjCells[i] != cellQueue[cellQueue.Count - 1].NearestRidge) && (adjCells[i].DistanceToRidges < 0 || (distance < adjCells[i].DistanceToRidges && adjCells[i].NearestRidge == cellQueue[cellQueue.Count - 1]) || (height > adjCells[i].Height && adjCells[i].NearestRidge != cellQueue[cellQueue.Count - 1])))
                        {
                            adjCells[i].NearestRidge = cellQueue[cellQueue.Count - 1].NearestRidge;
                            adjCells[i].DistanceToRidges = distance;
                            adjCells[i].NearestRidgeHeight = cellQueue[cellQueue.Count - 1].NearestRidgeHeight;
                            adjCells[i].Height = height;

                            if (!tempCellQueue.Contains(adjCells[i]))
                                tempCellQueue.Add(adjCells[i]);
                        }
                    }

                    cellQueue.RemoveAt(cellQueue.Count - 1);
                }
                cellQueue = tempCellQueue;
            }
        }

        private static float getDistance(float x, float y, float ix, float iy)
        {
            return (float)Math.Sqrt(Math.Pow(x - ix, 2) + Math.Pow(y - iy, 2));
        }

        private static bool landAndAdjacentToRidges(GeoPoint[][] geoPoints, GeoPoint geoPoint)
        {
            if (!geoPoint.Ridge)
            {
                List<GeoPoint> adjCells = GeoPoint.getAdjacentCells(geoPoints, geoPoint);



                for (int i = 0; i < adjCells.Count; i++)
                {
                    if (adjCells[i].Ridge)
                    {
                        float distance = getDistance(geoPoint.X, geoPoint.Y, adjCells[i].X, adjCells[i].Y);
                        if (geoPoint.NearestRidge != null)
                        {
                            if (geoPoint.DistanceToRidges < distance)
                            {
                                geoPoint.NearestRidgeHeight = adjCells[i].Height;
                                geoPoint.NearestRidge = adjCells[i];
                                geoPoint.DistanceToRidges = distance;
                            }
                        }
                        else
                        {
                            geoPoint.NearestRidgeHeight = adjCells[i].Height;
                            geoPoint.NearestRidge = adjCells[i];
                            geoPoint.DistanceToRidges = distance;
                        }
                    }
                }
            }

            return geoPoint.NearestRidge != null;
        }

        public static void smooth2(GeoPoint[][] heightMap, int steps)
        {
            for (int it = 0; it < steps; it++)
            {
                for (int x = 0; x < heightMap.Length; x++)
                {
                    for (int y = 0; y < heightMap.Length; y++)
                    {
                        List<GeoPoint> neighbours = GeoPoint.getAdjacentCells(heightMap, heightMap[x][y]);
                        float heightSum = 0;
                        foreach(GeoPoint neighbour in neighbours)
                            heightSum += neighbour.Height;

                        heightSum = heightSum / (float)neighbours.Count;

                        if (heightSum > heightMap[x][y].Height)
                            heightMap[x][y].Height = heightSum;
                    }
                }
            }
        }

        public static void smooth(GeoPoint[][] heightMap)
        {
            GeoPoint[][] heightMapCopy = new GeoPoint[heightMap.Length][];
            for(int i = 0; i < heightMapCopy.Length; i++)
            {
                heightMapCopy[i] = new GeoPoint[heightMap.Length];
            }

            for (int x = 0; x < heightMap.Length; x++)
            {
                int lastY = 0;
                float lastHeight = 0;
                for (int y = 0; y < heightMap.Length; y++)
                {
                    GeoPoint geoPoint = new GeoPoint();
                    geoPoint.Height = heightMap[x][y].Height;
                    heightMapCopy[x][y] = geoPoint;
                    if (heightMap[x][y].Height != 0)
                    {
                        interpolate(lastY, y, x, true, lastHeight, heightMap[x][y].Height, heightMap);
                        lastY = y;
                        lastHeight = heightMap[x][y].Height;
                    }
                }

                if (lastY > 0)
                    interpolate(lastY, heightMap.Length - 1, x, true, lastHeight, heightMap[x][heightMap.Length - 1].Height, heightMap);
            }

            for (int y = 0; y < heightMap.Length; y++)
            {
                int lastX = 0;
                float lastHeight = 0;
                for (int x = 0; x < heightMap.Length; x++)
                {
                    if (heightMapCopy[x][y].Height != 0)
                    {
                        interpolate(lastX, x, y, false, lastHeight, heightMapCopy[x][y].Height, heightMap);
                        lastX = x;
                        lastHeight = heightMapCopy[x][y].Height;
                    }
                }

                if (lastX > 0)
                    interpolate(lastX, heightMap.Length - 1, y, false, lastHeight, heightMapCopy[heightMap.Length - 1][y].Height, heightMap);
            }
        }

        public static void interpolate(int lastIdx, int currentIdx, int currentColumn, bool isXRow, float lastValue, float currentValue, GeoPoint[][] heightMap)
        {
            if ((currentIdx - lastIdx) <= 1)
                return;


            float valueInterval = (currentValue - lastValue) / (float)((currentIdx - lastIdx) - 1);
            float current = lastValue + valueInterval;

            for (int idx = lastIdx + 1; idx < currentIdx; idx++)
            {
                if (current < 0)
                    current = 0;
                if (current > 1)
                    current = 1;
                if (isXRow)
                {
                    if (heightMap[currentColumn][idx].Height != 0)
                    {
                        heightMap[currentColumn][idx].Height = (heightMap[currentColumn][idx].Height + current) / 2.0f;
                    }
                    else
                    {
                        heightMap[currentColumn][idx].Height = current;
                    }
                }
                else
                {
                    if (heightMap[idx][currentColumn].Height != 0)
                    {
                        heightMap[idx][currentColumn].Height = (heightMap[idx][currentColumn].Height + current) / 2.0f;
                    }
                    else
                    {
                        heightMap[idx][currentColumn].Height = current;
                    }
                }

                current += valueInterval;
            }
        }

        public static float normalDistribution(float distanceFromCenter, float sd, float amp)
        {
            return (float)(amp * Math.Exp(-0.5f * Math.Pow(distanceFromCenter / sd, 2)));
        }

        public static float? getTopLeft(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x - 1;
            nY = y - 1;
            if (x > 0 && y > 0)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getLeft(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x - 1;
            nY = y;
            if (x > 0)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getBottomLeft(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x - 1;
            nY = y + 1;
            if (x > 0 && (y + 1) < heightMap[x - 1].Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getTop(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x;
            nY = y - 1;
            if (y > 0)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getBottom(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x;
            nY = y + 1;
            if ((y + 1) < heightMap[x].Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getTopRight(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x + 1;
            nY = y - 1;
            if ((x + 1) < heightMap.Length && y > 0)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getRight(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x + 1;
            nY = y;
            if ((x + 1) < heightMap.Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getBottomRight(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x + 1;
            nY = y + 1;
            if ((x + 1) < heightMap.Length && (y + 1) < heightMap[x + 1].Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }








        public static float? getTopTopLeft(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x - 1;
            nY = y - 2;
            if (x > 0 && y > 1)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getLeftLeftTop(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x - 2;
            nY = y - 1;
            if (x > 1 && y > 0)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getTopTopRight(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x + 1;
            nY = y - 2;
            if ((x + 1) < heightMap.Length && y > 1)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getRightRightTop(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x + 2;
            nY = y - 1;
            if ((x + 2) < heightMap.Length && y > 0)
            {
                return heightMap[nX][nY];
            }
            return null;
        }




        public static float? getBottomBottomLeft(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x - 1;
            nY = y + 2;
            if (x > 0 && (y + 2) < heightMap.Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getLeftLeftBottom(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x - 2;
            nY = y + 1;
            if (x > 1 && (y + 1) < heightMap.Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getBottomBottomRight(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x + 1;
            nY = y + 2;
            if ((x + 1) < heightMap.Length && (y + 2) < heightMap.Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }

        public static float? getRightRightBottom(float?[][] heightMap, int x, int y, out int nX, out int nY)
        {
            nX = x + 2;
            nY = y + 1;
            if ((x + 2) < heightMap.Length && (y + 1) < heightMap.Length)
            {
                return heightMap[nX][nY];
            }
            return null;
        }
    }
}
