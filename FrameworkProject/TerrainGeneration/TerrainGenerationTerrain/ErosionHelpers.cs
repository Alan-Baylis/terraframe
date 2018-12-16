using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGeneration_HeightMap
{
    class ErosionHelpers
    {
        // Hydraulic erosion
        private static float Kr = 0.03f;
        private static float Ks = 0.03f;
        private static float Ke = 1.5f;
        private static float Kc = 0.03f;

        public static float T = 4.0f / 513.0f;
        public static float C = 0.5f;

        public static HydraulicErosionCell[][] applyHydraulicErosion(float?[][] heightMap, int iterations)
        {
            HydraulicErosionCell[][] hydraulicErosionCells = createHydraulicErosionCells(heightMap);


            System.Console.Out.WriteLine("###Pass " + iterations + "######");
            System.Console.Out.WriteLine("###Pass " + heightMap.Length + "######");
            System.Console.Out.WriteLine("###Pass " + hydraulicErosionCells.Length + "######");

            for (int i = 0; i < iterations; i++)
            {
                hydraulicErosionCells = hydraulicErosionIteration(hydraulicErosionCells);
            }

            updateHydraulicHeightMap(hydraulicErosionCells, heightMap);

            return hydraulicErosionCells;
        }

        private static HydraulicErosionCell[][] createHydraulicErosionCells(float?[][] heightMap)
        {
            int side = heightMap.Length;

            HydraulicErosionCell[][] hydraulicErosionCells = new HydraulicErosionCell[side][];
            for(int i = 0; i < side; i++)
                hydraulicErosionCells[i] = new HydraulicErosionCell[side];

            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {
                    HydraulicErosionCell cell = new HydraulicErosionCell();
                    cell.Altitude = (float)heightMap[x][y];
                    hydraulicErosionCells[x][y] = cell;
                }
            }

            return hydraulicErosionCells;
        }

        private static void updateHydraulicHeightMap(HydraulicErosionCell[][] hydraulicErosionCells, float?[][] heightMap)
        {
            int side = heightMap.Length;

            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {
                    //if (heightMap[x][y] != hydraulicErosionCells[x][y].Altitude)
                        //System.Console.Out.WriteLine("###Diff " + heightMap[x][y] + " : " + hydraulicErosionCells[x][y].Altitude + "######");

                    if (hydraulicErosionCells[x][y].Altitude < 0)
                        heightMap[x][y] = 0.0f;
                    else if (hydraulicErosionCells[x][y].Altitude > 1)
                        heightMap[x][y] = 1.0f;
                    else
                        heightMap[x][y] = hydraulicErosionCells[x][y].Altitude;
                }
            }
        }

        private static HydraulicErosionCell[][] hydraulicErosionIteration(HydraulicErosionCell[][] hydraulicErosionCells)
        {
            int side = hydraulicErosionCells.Length;

            //HydraulicErosionCell[][] resultHydraulicErosionCells = new HydraulicErosionCell[side][side]; 

            // Step1 Rain water added
            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {
                    hydraulicErosionCells[x][y].Water = hydraulicErosionCells[x][y].Water + Kr;
                }
            }

            // Step2 Create sediment
            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {
                    hydraulicErosionCells[x][y].Altitude = hydraulicErosionCells[x][y].Altitude - Ks * hydraulicErosionCells[x][y].Water;
                    hydraulicErosionCells[x][y].Sediment = hydraulicErosionCells[x][y].Sediment + Ks * hydraulicErosionCells[x][y].Water;
                }
            }

            // Step3 water and sediment transport
            HydraulicErosionCell[][] resultHydraulicErosionCells = new HydraulicErosionCell[side][];
            for (int i = 0; i < side; i++)
                resultHydraulicErosionCells[i] = new HydraulicErosionCell[side];
            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {
                    resultHydraulicErosionCells[x][y] = hydraulicErosionCells[x][y].getCopy();
                }
            }

            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {

                    HydraulicErosionCell current = hydraulicErosionCells[x][y];
                    HydraulicErosionCell resultCurrent = resultHydraulicErosionCells[x][y];
                    List<HydraulicErosionCell> neighbours = new List<HydraulicErosionCell>();
                    List<HydraulicErosionCell> resultNeighbours = new List<HydraulicErosionCell>();


                    if (x > 0 && y > 0)
                        neighbours.Add(hydraulicErosionCells[x - 1][y - 1]);
                    if (y > 0)
                        neighbours.Add(hydraulicErosionCells[x][y - 1]);
                    if (x < side - 1 && y > 0)
                        neighbours.Add(hydraulicErosionCells[x + 1][y - 1]);
                    if (x < side - 1)
                        neighbours.Add(hydraulicErosionCells[x + 1][y]);
                    if (x < side - 1 && y < side - 1)
                        neighbours.Add(hydraulicErosionCells[x + 1][y + 1]);
                    if (y < side - 1)
                        neighbours.Add(hydraulicErosionCells[x][y + 1]);
                    if (x > 0 && y < side - 1)
                        neighbours.Add(hydraulicErosionCells[x - 1][y + 1]);
                    if (x > 0)
                        neighbours.Add(hydraulicErosionCells[x - 1][y]);

                    if (x > 0 && y > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x - 1][y - 1]);
                    if (y > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x][y - 1]);
                    if (x < side - 1 && y > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x + 1][y - 1]);
                    if (x < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x + 1][y]);
                    if (x < side - 1 && y < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x + 1][y + 1]);
                    if (y < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x][y + 1]);
                    if (x > 0 && y < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x - 1][y + 1]);
                    if (x > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x - 1][y]);

                    transportWater(neighbours, resultNeighbours, current, resultCurrent);
                }
            }

            hydraulicErosionCells = resultHydraulicErosionCells;

            // Step4 Evaporation and deposition
            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {

                    hydraulicErosionCells[x][y].Water = hydraulicErosionCells[x][y].Water * (1 - Ke);
                    float mmax = Kc * hydraulicErosionCells[x][y].Water;
                    float dm = Math.Max(0, hydraulicErosionCells[x][y].Sediment - mmax);
                    hydraulicErosionCells[x][y].Sediment = hydraulicErosionCells[x][y].Sediment - dm;
                    hydraulicErosionCells[x][y].Altitude = hydraulicErosionCells[x][y].Altitude + dm;
                }
            }

            return hydraulicErosionCells;
        }

        private static void transportWater(List<HydraulicErosionCell> neighbours, List<HydraulicErosionCell> resultNeighbours, HydraulicErosionCell current, HydraulicErosionCell resultCurrent)
        {

            float a = current.Altitude + current.Water;
            float aa = 0;
            float dtotal = 0;
            int validCount = 0;

            for (int i = 0; i < neighbours.Count; i++)
            {
                float ai = neighbours[i].Altitude + neighbours[i].Water;
                if (ai < a)
                {
                    aa += ai;
                    dtotal += (a - ai);
                    validCount++;
                }
            }

            aa /= (float)validCount;
            float da = a - aa;

            for (int i = 0; i < neighbours.Count; i++)
            {
                float ai = neighbours[i].Altitude + neighbours[i].Water;
                if (ai < a)
                {
                    float di = a - ai;
                    float dw = Math.Min(current.Water, da) * (di / dtotal);
                    float dm = current.Sediment * (dw / current.Water);

                    resultNeighbours[i].Water = resultNeighbours[i].Water + dw;
                    resultCurrent.Water = resultCurrent.Water - dw;
                    resultNeighbours[i].Sediment = resultNeighbours[i].Sediment + dm;
                    resultCurrent.Sediment = resultCurrent.Sediment - dm;
                }
            }
        }

        public static void applyThermalErosion(float?[][] heightMap, int iterations)
        {
            HydraulicErosionCell[][] hydraulicErosionCells = createHydraulicErosionCells(heightMap);

            T = 4.0f / (float)heightMap.Length;

            for (int i = 0; i < iterations; i++)
            {
                hydraulicErosionCells = thermalErosionIteration(hydraulicErosionCells);
            }

            updateHydraulicHeightMap(hydraulicErosionCells, heightMap);
        }

        private static HydraulicErosionCell[][] thermalErosionIteration(HydraulicErosionCell[][] hydraulicErosionCells)
        {
            int side = hydraulicErosionCells.Length;

            HydraulicErosionCell[][] resultHydraulicErosionCells = new HydraulicErosionCell[side][];
            for(int i = 0; i < side; i++)
                resultHydraulicErosionCells[i] = new HydraulicErosionCell[side];

            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {
                    resultHydraulicErosionCells[x][y] = hydraulicErosionCells[x][y].getCopy();
                }
            }

            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
                {
                    HydraulicErosionCell current = hydraulicErosionCells[x][y];
                    HydraulicErosionCell resultCurrent = resultHydraulicErosionCells[x][y];
                    List<HydraulicErosionCell> neighbours = new List<HydraulicErosionCell>();
                    List<HydraulicErosionCell> resultNeighbours = new List<HydraulicErosionCell>();


                    if (x > 0 && y > 0)
                        neighbours.Add(hydraulicErosionCells[x - 1][y - 1]);
                    if (y > 0)
                        neighbours.Add(hydraulicErosionCells[x][y - 1]);
                    if (x < side - 1 && y > 0)
                        neighbours.Add(hydraulicErosionCells[x + 1][y - 1]);
                    if (x < side - 1)
                        neighbours.Add(hydraulicErosionCells[x + 1][y]);
                    if (x < side - 1 && y < side - 1)
                        neighbours.Add(hydraulicErosionCells[x + 1][y + 1]);
                    if (y < side - 1)
                        neighbours.Add(hydraulicErosionCells[x][y + 1]);
                    if (x > 0 && y < side - 1)
                        neighbours.Add(hydraulicErosionCells[x - 1][y + 1]);
                    if (x > 0)
                        neighbours.Add(hydraulicErosionCells[x - 1][y]);

                    if (x > 0 && y > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x - 1][y - 1]);
                    if (y > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x][y - 1]);
                    if (x < side - 1 && y > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x + 1][y - 1]);
                    if (x < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x + 1][y]);
                    if (x < side - 1 && y < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x + 1][y + 1]);
                    if (y < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x][y + 1]);
                    if (x > 0 && y < side - 1)
                        resultNeighbours.Add(resultHydraulicErosionCells[x - 1][y + 1]);
                    if (x > 0)
                        resultNeighbours.Add(resultHydraulicErosionCells[x - 1][y]);

                    transportSediment(neighbours, resultNeighbours, current, resultCurrent);
                }
            }
            return resultHydraulicErosionCells;
        }

        private static void transportSediment(List<HydraulicErosionCell> neighbours, List<HydraulicErosionCell> resultNeighbours, HydraulicErosionCell current, HydraulicErosionCell resultCurrent)
        {

            float dtotal = 0;
            float dmax = 0;
            for (int i = 0; i < neighbours.Count; i++)
            {
                float di = current.Altitude - neighbours[i].Altitude;
                if (di > T)
                {
                    dtotal += di;
                    if (di > dmax)
                        dmax = di;
                }
            }

            if (dtotal > 0)
            {
                for (int i = 0; i < neighbours.Count; i++)
                {
                    float di = current.Altitude - neighbours[i].Altitude;
                    if (di > T)
                    {
                        float dh = C * (dmax - T) * (di / dtotal);

                        resultNeighbours[i].Altitude = resultNeighbours[i].Altitude + dh;
                        resultCurrent.Altitude = resultCurrent.Altitude - dh;
                    }
                }
            }
        }
    }
}
