
using System;
using System.Collections.Generic;
using TerrainGeneration_Core;
using TerrainGeneration_PluginBase;
using TerrainGeneration_PluginBase.Helpers;
using TerrainGeneration_PluginBase.WorkflowData;

namespace TerrainGeneration_HeightMap
{
    public class RidgeHeightMap
    {
        private static List<float> meanHeight = new List<float>();
        private static List<float> meanVariance = new List<float>();
        public static int curSample = 0;
        public static int targetSamples = 100;

        public static void startWork(List<TerrainInput> terrainInputs, List<TerrainOutput> terrainOutputs, HeightmapWorkflowData heightmapWorkflowData)
        {
            int size = 513;
            Dictionary<string, TerrainInput> inputMap = new Dictionary<string, TerrainInput>();
            Dictionary<string, FormField> formFieldsMap = new Dictionary<string, FormField>();
            createInputMap(terrainInputs, inputMap, formFieldsMap);

            System.Console.Out.WriteLine("Starting");

            switch (((FormFieldOptions)formFieldsMap["parameters_gridSize"]).Value)
            {
                case 0:
                    size = 65;
                    break;
                case 1:
                    size = 129;
                    break;
                case 2:
                    size = 257;
                    break;
                case 3:
                    size = 513;
                    break;
                case 4:
                    size = 1025;
                    break;
            }

            Random random;
            if (targetSamples > 0)
            {
                random = new Random();
                if(curSample == 0)
                {
                    meanHeight = new List<float>();
                    meanVariance = new List<float>();
                }
            }
            else
            {
                random = new Random(((FormFieldInteger)formFieldsMap["parameters_seed"]).Value);
            }
            //Random random = new Random(((FormFieldInteger)formFieldsMap["parameters_seed"]).Value);

            float heightFactor = (float)((FormFieldInteger)formFieldsMap["parameters_maxHeight"]).Value/(float)((FormFieldInteger)formFieldsMap["parameters_cellSize"]).Value;

            float?[][] heightMap = new float?[size][];
		
		    for ( int x = 0; x<heightMap.Length; x++ ) {
                heightMap[x] = new float?[size];
                for ( int y = 0; y<heightMap.Length; y++ ) {
				    heightMap[x][y] = null;  
			    }
            }
            System.Console.Out.WriteLine("Data created");

            TerrainOutputImage terrainOutputRidges = new TerrainOutputImage();
            terrainOutputRidges.Title = "Ridges";
            terrainOutputRidges.Key = "ridges";

            List<List<GeoPoint>> ridges = new List<List<GeoPoint>>();

            if (((FormFieldCheck)formFieldsMap["parameters_useRidgeSketch"]).Value)
            {
                int[][] resizedPixels = Helpers.Instance.ResizePixels(((TerrainInputSketch)inputMap["ridgeSketch"]).Value, ((TerrainInputSketch)inputMap["ridgeSketch"]).Width, ((TerrainInputSketch)inputMap["ridgeSketch"]).Height, size, size);

                terrainOutputRidges.ImageData = resizedPixels;

                for (int x = 0; x < heightMap.Length; x++)
                {
                    for (int y = 0; y < heightMap.Length; y++)
                    {
                        float height = (float)(((resizedPixels[x][y] & 0xff000000)>>24)/256.0);
                        if (height != 0)
                        heightMap[x][y] = height;
                    }
                }
            }
            else
            {
                int ridgeParticles = ((FormFieldInteger)formFieldsMap["parameters_ridgeParticles"]).Value;

                RidgeHelpers.createRidges(heightMap, ridges, random, ridgeParticles);

                terrainOutputRidges.ImageData = new int[heightMap.Length][];
                for (int x = 0; x < heightMap.Length; x++)
                {
                    terrainOutputRidges.ImageData[x] = new int[heightMap.Length];
                    for (int y = 0; y < heightMap[x].Length; y++)
                    {
                        byte color = 0;
                        if (heightMap[x][y] != null)
                            color = (byte)(heightMap[x][y] * 256);

                        terrainOutputRidges.ImageData[x][y] = (color << 24) | (color << 16) | (color << 8) | 0xff;
                    }
                }
            }


            GeoPoint[][] gaussGeoPoints = TerrainHelpers.calculateDistanceToRidges(heightMap);
            TerrainOutputImage gaussTerrainOutputHeightMap = new TerrainOutputImage();
            gaussTerrainOutputHeightMap.Title = "Gauss Height map";
            gaussTerrainOutputHeightMap.Key = "gaussHeightMap";

            gaussTerrainOutputHeightMap.ImageData = new int[gaussGeoPoints.Length][];
            for (int x = 0; x < gaussGeoPoints.Length; x++)
            {
                gaussTerrainOutputHeightMap.ImageData[x] = new int[gaussGeoPoints.Length];
                for (int y = 0; y < gaussGeoPoints[x].Length; y++)
                {
                    byte color = (byte)(gaussGeoPoints[x][y].Height * 256);
                    gaussTerrainOutputHeightMap.ImageData[x][y] = (color << 24) | (color << 16) | (color << 8) | 0xff;
                }
            }

            float?[][] gaussHeightMap = new float?[gaussGeoPoints.Length][];
            for (int x = 0; x < gaussGeoPoints.Length; x++)
            {
                gaussHeightMap[x] = new float?[gaussGeoPoints.Length];
                for (int y = 0; y < gaussGeoPoints[x].Length; y++)
                {
                    gaussHeightMap[x][y] = gaussGeoPoints[x][y].Height;
                }
            }

            int[][] roughnessSketch = Helpers.Instance.ResizePixels(((TerrainInputSketch)inputMap["roughnessSketch"]).Value, ((TerrainInputSketch)inputMap["roughnessSketch"]).Width, ((TerrainInputSketch)inputMap["roughnessSketch"]).Height, size, size);
            float[][] roughness = new float[size][];

            int veryLowColor = ColorToInt(stringToColor("#0000ffff"));
            int lowColor = ColorToInt(stringToColor("#00ffffff"));
            int highColor = ColorToInt(stringToColor("#ffff00ff"));
            int veryHighColor = ColorToInt(stringToColor("#ff0000ff"));

            for (int x = 0; x < size; x++)
            {
                roughness[x] = new float[size];
                for(int y = 0; y < size; y++)
                {
                    if (roughnessSketch[x][y] == veryLowColor)
                    {
                        roughness[x][y] = 0.5f;
                    }
                    else if (roughnessSketch[x][y] == lowColor)
                    {
                        roughness[x][y] = 0.75f;
                    }
                    else if (roughnessSketch[x][y] == highColor)
                    {
                        roughness[x][y] = 1.25f;
                    }
                    else if (roughnessSketch[x][y] == veryHighColor)
                    {
                        roughness[x][y] = 1.5f;
                    }
                    else
                    {
                        roughness[x][y] = 1f;
                    }

                }
            }



            TerrainOutputImage terrainOutputRoughness = new TerrainOutputImage();
            terrainOutputRoughness.Title = "Roughness";
            terrainOutputRoughness.Key = "roughness";
            terrainOutputRoughness.ImageData = roughnessSketch;


            //List<GeoPoint> riverPoints = RiverHelpers.traceRivers(gaussHeightMap, ridges, 8, random);
            /*foreach(GeoPoint geoPoint in riverPoints)
            {
                heightMap[geoPoint.X][geoPoint.Y] = geoPoint.Height;
            }*/
            List<GeoPoint> riverPoints = new List<GeoPoint>();

            System.Console.Out.WriteLine("Ridges created");
            TerrainHelpers.generateTerrain(heightMap, random, roughness, riverPoints, gaussHeightMap);
            System.Console.Out.WriteLine("Terrain created");

            int termalErosionPass = ((FormFieldInteger)formFieldsMap["parameters_thermalErosionPass"]).Value;
            if (termalErosionPass != 0) {
                ErosionHelpers.applyThermalErosion(heightMap, termalErosionPass);
                System.Console.Out.WriteLine("ThermalErosion applied");
            }

            int hydraulicErosionPass = ((FormFieldInteger)formFieldsMap["parameters_hydraulicErosionPass"]).Value;
            if (hydraulicErosionPass != 0)
            {
                HydraulicErosionCell[][] hydraulicErosionCells = ErosionHelpers.applyHydraulicErosion(heightMap, hydraulicErosionPass);
                System.Console.Out.WriteLine("HydraulicErosion applied");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("###Pass "+ hydraulicErosionPass + "######");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");
                System.Console.Out.WriteLine("########################");

            }

            System.Console.Out.WriteLine("##From terrain changed!!!");





            String meanHeightStr = "Mean samples: ";
            String meanVarianceStr = "Mean variance: ";


            Boolean doContinue = true;
            if (targetSamples > 0)
            {
                //meanHeight = new List<float>();
                //meanVariance = new List<float>();

                int count = 0;
                double sum = 0;
                double diffSum = 0;
                for (int x = 0; x < heightMap.Length; x++)
                {
                    for (int y = 0; y < heightMap[x].Length; y++)
                    {
                        count++;
                        sum += heightMap[x][y].Value;

                        diffSum += Math.Abs(getAdjacentCellsMean(heightMap, x, y).Value - heightMap[x][y].Value);
                    }
                }

                float mean = (float)(sum / count);
                float diffMean = (float)(diffSum / count);

                meanHeight.Add(mean);
                meanVariance.Add(diffMean);

                foreach (float meanValue in meanHeight)
                    meanHeightStr += "#" + meanValue;

                foreach (float varianceValue in meanVariance)
                    meanVarianceStr += "#" + varianceValue;

                System.Console.Out.WriteLine(meanHeightStr);
                System.Console.Out.WriteLine(meanVarianceStr);

                curSample++;

                if (curSample < targetSamples)
                {
                    terrainOutputs.Clear();

                    System.Console.Out.WriteLine("Starting sample:" + curSample);
                    doContinue = false;
                    startWork(terrainInputs, terrainOutputs, heightmapWorkflowData);
                }
                else
                {
                    System.Console.Out.WriteLine("All samples done");
                }
            }



            if (!doContinue)
                return;





            TerrainOutputMesh terrainOutputMesh = new TerrainOutputMesh();
            terrainOutputMesh.Title = "Height map mesh";
            terrainOutputMesh.Key = "heightMap";
            terrainOutputMesh.CameraPosition = new float[]{0,size/4,0};
            terrainOutputMesh.CameraRotation = new float[] {22.5f,45,0};

            exportHeightMap(heightMap, terrainOutputMesh, heightFactor);


            TerrainOutputImage terrainOutputHeightMap = new TerrainOutputImage();
            terrainOutputHeightMap.Title = "Height map bmp";
            terrainOutputHeightMap.Key = "heightMapBmp";

            GeoPoint[][] heightMapGeoPoints = new GeoPoint[heightMap.Length][];

            terrainOutputHeightMap.ImageData = new int[heightMap.Length][];
            heightmapWorkflowData.HeightmapCells = new float[heightMap.Length][];
            float maxHeight = 0;
            float minHeight = 1;
            for (int x = 0; x < heightMap.Length; x++)
            {
                heightMapGeoPoints[x] = new GeoPoint[heightMap[x].Length];
                terrainOutputHeightMap.ImageData[x] = new int[heightMap.Length];
                heightmapWorkflowData.HeightmapCells[x] = new float[heightMap.Length];
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    GeoPoint geoPoint = new GeoPoint();
                    geoPoint.Height = (float)heightMap[x][y];
                    geoPoint.X = x;
                    geoPoint.Y = y;
                    heightMapGeoPoints[x][y] = geoPoint;
                    byte color = (byte)(heightMap[x][y] * 256);
                    terrainOutputHeightMap.ImageData[x][y] = (color<<24)|(color<<16)|(color<<8)|0xff;
                    heightmapWorkflowData.HeightmapCells[x][y] = (float)heightMap[x][y];

                    if (maxHeight < (float)heightMap[x][y])
                        maxHeight = (float)heightMap[x][y];

                    if (minHeight > (float)heightMap[x][y])
                        minHeight = (float)heightMap[x][y];
                }
            }

            heightmapWorkflowData.MaxHeight = maxHeight * (float)((FormFieldInteger)formFieldsMap["parameters_maxHeight"]).Value;
            heightmapWorkflowData.MinHeight = minHeight * (float)((FormFieldInteger)formFieldsMap["parameters_maxHeight"]).Value;
            heightmapWorkflowData.CellSize = (float)((FormFieldInteger)formFieldsMap["parameters_cellSize"]).Value;

            foreach (GeoPoint geoPoint in riverPoints)
            {
                byte color = (byte)(geoPoint.Height * 256);
                terrainOutputHeightMap.ImageData[geoPoint.X][geoPoint.Y] = (0x00 << 24) | (0x00 << 16) | (color << 8) | 0xff;
            }




            int chunk = 0;
            TerrainOutputMesh splittedMesh = new TerrainOutputMesh();
            splittedMesh.Title = "Terrain";
            splittedMesh.Key = "terrainHeightMap";

            List<float[]> grassLand = new List<float[]>();

            MeshGeneratorHelpers.FillHeightMapMesh(splittedMesh, heightMapGeoPoints, heightmapWorkflowData.CellSize, heightmapWorkflowData.MinHeight, heightmapWorkflowData.MaxHeight, grassLand);


            /*splittedMesh.VertexData.Add(new List<float[]>());
            splittedMesh.FacesData.Add(new List<int[]>());
            splittedMesh.MaterialColor.Add("#ffffffff");
            splittedMesh.MaterialTexture.Add("HeightMapMaterials\\transitionTexture.png");
            splittedMesh.TexureCoordData.Add(new List<float[]>());
            exportTransitionHeightMap(heightMapGeoPoints, splittedMesh, chunk, heightFactor);*/



            terrainOutputs.Add(splittedMesh);
            //terrainOutputs.Add(terrainOutputMesh);
            terrainOutputs.Add(terrainOutputHeightMap);
            terrainOutputs.Add(gaussTerrainOutputHeightMap);
            terrainOutputs.Add(terrainOutputRidges);
            //terrainOutputs.Add(terrainOutputObject);
            //terrainOutputs.Add(terrainOutputRoughness);
            //terrainOutputs.Add(terrainOutputSplitted);

            //Vector[][] waterVelocityField = RiverHelpers.getVelocityField(gaussHeightMap);
            //terrainOutputs.Add(RiverHelpers.getVectorFieldImage(waterVelocityField));

        }

        public static float? getAdjacentCellsMean(float?[][] heightMap, int x, int y)
        {
            List<float?> adjCells = new List<float?>();

            if (x > 0 && y > 0)
                adjCells.Add(heightMap[x - 1][y - 1]);
            if (y > 0)
                adjCells.Add(heightMap[x][y - 1]);
            if (x < heightMap.Length - 1 && y > 0)
                adjCells.Add(heightMap[x + 1][y - 1]);
            if (x < heightMap.Length - 1)
                adjCells.Add(heightMap[x + 1][y]);
            if (x < heightMap.Length - 1 && y < heightMap.Length - 1)
                adjCells.Add(heightMap[x + 1][y + 1]);
            if (y < heightMap.Length - 1)
                adjCells.Add(heightMap[x][y + 1]);
            if (x > 0 && y < heightMap.Length - 1)
                adjCells.Add(heightMap[x - 1][y + 1]);
            if (x > 0)
                adjCells.Add(heightMap[x - 1][y]);


            float? sum = 0;
            foreach(float? height in adjCells)
            {
                sum += height;
            }

            return sum / adjCells.Count;
        }

        private static void exportHeightMap(float?[][] heightMap, TerrainOutputMesh terrainOutputMesh, float heightFactor)
        {
            int chunk = 65;

            int xStart = 0;
            int xEnd = chunk;
            System.Console.WriteLine("xEnd: " + xEnd + "  " + heightMap.Length);

            int currentChunk = 0;
            while (xEnd <= heightMap.Length)
            {
                int yStart = 0;
                int yEnd = chunk;
                System.Console.WriteLine("yEnd: " + yEnd + "  " + heightMap.Length);
                while (yEnd <= heightMap.Length)
                {

                    terrainOutputMesh.VertexData.Add(new List<float[]>());
                    terrainOutputMesh.FacesData.Add(new List<int[]>());

                    for (int x = xStart; x < xEnd; x++)
                    {
                        for (int y = yStart; y < yEnd; y++)
                        {
                            terrainOutputMesh.VertexData[currentChunk].Add(new float[] { x, (float)(heightMap[x][y] * heightFactor), y });
                        }
                    }

                    for (int x = 0; x < xEnd-xStart - 1; x++)
                    {
                        for (int y =  1; y < yEnd-yStart; y++)
                        {
                            terrainOutputMesh.FacesData[currentChunk].Add(new int[] {y+x*(yEnd-yStart)+1,
                    y+(x+1)*(yEnd-yStart),
                    y +x*(yEnd-yStart) });

                            terrainOutputMesh.FacesData[currentChunk].Add(new int[] {y+x*(yEnd-yStart)+1,
                    y+(x+1)*(yEnd-yStart)+1,
                    y+(x+1)*(yEnd-yStart)});
                        }
                    }

                    yStart = yEnd-1;
                    yEnd = yStart + chunk;

                    currentChunk++;
                    System.Console.WriteLine("yEnd: " + yEnd + "  " + heightMap.Length);
                }

                xStart = xEnd-1;
                xEnd = xStart + chunk;
                System.Console.WriteLine("xEnd: " + xEnd + "  " + heightMap.Length);
            }
            
        }




        





        private static void exportTransitionHeightMap(GeoPoint[][] heightMap, TerrainOutputMesh terrainOutputMesh, int chunk, float heightFactor)
        {

            Dictionary<CellType, float[]> typeUvsStart = new Dictionary<CellType, float[]>();
            typeUvsStart.Add(CellType.Grass, new float[] { 0, 1 });
            typeUvsStart.Add(CellType.Snow, new float[] { 1, 1 });
            typeUvsStart.Add(CellType.Rock, new float[] { 0, 0 });

            Dictionary<CellType, float[]> typeUvsEnd = new Dictionary<CellType, float[]>();
            typeUvsEnd.Add(CellType.Grass, new float[] { 0, 0.33f });
            typeUvsEnd.Add(CellType.Snow, new float[] { 1, 0.33f });
            typeUvsEnd.Add(CellType.Rock, new float[] { 0.66f, 1 });


            List<GeoPoint> tempVertices = new List<GeoPoint>();
            List<GeoPoint[]> tempFaces = new List<GeoPoint[]>();

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    heightMap[x][y].VertexIdx = -1;
                    heightMap[x][y].uv = null;
                }
            }

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 1; y < heightMap[x].Length; y++)
                {
                    //terrainOutputMesh.VertexData[chunk].Add(new float[] { x, (float)(heightMap[x][y].Height * heightFactor), y });
                    tempVertices.Add(heightMap[x][y]);

                    if (x < (heightMap[x].Length - 1) 
                        && (heightMap[x][y].Type != heightMap[x][y-1].Type
                        || heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        || heightMap[x][y].Type != heightMap[x + 1][y].Type))
                    {
                        if(heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // A
                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count-1]);

                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);
                        }

                        if (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // B
                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);

                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);
                        }

                        if (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y].Type)
                        {
                            // C
                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);
                        }

                        if (heightMap[x][y].Type == heightMap[x][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // D
                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);
                        }

                        if (heightMap[x][y].Type == heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // E
                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);

                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);
                        }

                        if (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y].Type)
                        {
                            // F
                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);

                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);
                        }

                        if (heightMap[x][y].Type == heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y].Type)
                        {
                            // G
                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);

                            tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });

                            setTransitionUv(tempFaces[tempFaces.Count - 1]);
                        }
                    }
                }
            }

            int vertexIdx = 1;
            foreach (GeoPoint[] geoPoints in tempFaces)
            {
                foreach (GeoPoint geoPoint in geoPoints)
                {
                    if (geoPoint.VertexIdx == -1)
                    {
                        terrainOutputMesh.VertexData[chunk].Add(new float[] { geoPoint.X, (float)(geoPoint.Height * heightFactor), geoPoint.Y });
                        geoPoint.VertexIdx = vertexIdx;
                        vertexIdx++;

                        terrainOutputMesh.TexureCoordData[chunk].Add(geoPoint.uv);
                    }
                }

                terrainOutputMesh.FacesData[chunk].Add(new int[] {
                    geoPoints[0].VertexIdx,
                    geoPoints[1].VertexIdx,
                    geoPoints[2].VertexIdx,
                });
            }
        }


        private static void setTransitionUv(GeoPoint[] triangle)
        {
            Dictionary<CellType, float[]> typeUvsStart = new Dictionary<CellType, float[]>();
            typeUvsStart.Add(CellType.Grass, new float[] { 0, 1 });
            typeUvsStart.Add(CellType.Snow, new float[] { 1, 1 });
            typeUvsStart.Add(CellType.Rock, new float[] { 0, 0 });

            Dictionary<CellType, float[]> typeUvsEnd = new Dictionary<CellType, float[]>();
            typeUvsEnd.Add(CellType.Grass, new float[] { 0, 0.66f });
            typeUvsEnd.Add(CellType.Snow, new float[] { 1, 0.66f });
            typeUvsEnd.Add(CellType.Rock, new float[] { 0.33f, 0.33f });

            HashSet<CellType> types = new HashSet<CellType>();
            foreach(GeoPoint geoPoint in triangle)
            {
                if (geoPoint.uv == null) {
                    if (types.Contains(geoPoint.Type))
                    {
                        geoPoint.uv = typeUvsEnd[geoPoint.Type];
                    }
                    else
                    {
                        geoPoint.uv = typeUvsStart[geoPoint.Type];
                    }
                }
                types.Add(geoPoint.Type);
            }
        }











        public static void createInputMap(List<TerrainInput> terrainInputs, Dictionary<string, TerrainInput> inputMap, Dictionary<string, FormField> formFieldsMap)
        {
            foreach(TerrainInput terrainInput in terrainInputs)
            {
                switch (terrainInput.Type)
                {
                    case InputType.Group:
                        createInputMap(((TerrainInputGroup)terrainInput).ChildInputs, inputMap, formFieldsMap);
                        break;
                    case InputType.Form:
                        inputMap.Add(terrainInput.Key, terrainInput);
                        foreach(FormField formField in ((TerrainInputForm)terrainInput).FormFields)
                        {
                            formFieldsMap.Add(terrainInput.Key + "_" + formField.Key, formField);
                        }
                        break;
                    default:
                        inputMap.Add(terrainInput.Key, terrainInput);
                        break;
                }
            }
        }

        //public static int[][] resizePixels(int[][] pixels, int w1, int h1, int w2, int h2)
        //{
        //    Bitmap original = new Bitmap(w1, h1);
        //    for(int x = 0; x < w1; x++)
        //    {
        //        for(int y = 0; y < h1; y++)
        //        {
        //            original.SetPixel(x, y, Color.FromArgb(RGBAtoARGB(pixels[x][y])));
        //        }
        //    }

        //    var newBmp = new Bitmap(w2, h2);
        //    var graph = Graphics.FromImage(newBmp);
        //    graph.InterpolationMode = InterpolationMode.Low;
        //    graph.CompositingQuality = CompositingQuality.HighSpeed;
        //    graph.SmoothingMode = SmoothingMode.None;

        //    graph.DrawImage(original, new Rectangle(0, 0, w2, h2));

        //    int[][] final = new int[w2][];
        //    for (int x = 0; x < w2; x++)
        //    {
        //        final[x] = new int[h2];
        //        for (int y = 0; y < h2; y++)
        //        {
        //            final[x][y] = ColorToInt(newBmp.GetPixel(x, y));
        //        }
        //    }
        //    graph.Dispose();
        //    return final;

        //    /*int[][] temp = new int[h2][];
        //    // EDIT: added +1 to account for an early rounding problem
        //    int x_ratio = (int)((w1 << 16) / w2) + 1;
        //    int y_ratio = (int)((h1 << 16) / h2) + 1;
        //    //int x_ratio = (int)((w1<<16)/w2) ;
        //    //int y_ratio = (int)((h1<<16)/h2) ;
        //    int x2, y2;
        //    for (int i = 0; i < h2; i++)
        //    {
        //        temp[i] = new int[w2];
        //        for (int j = 0; j < w2; j++)
        //        {
        //            x2 = ((j * x_ratio) >> 16);
        //            y2 = ((i * y_ratio) >> 16);
        //            temp[i][j] = pixels[y2][x2];
        //        }
        //    }
        //    return temp;*/
        //}

        private static byte[] stringToColor(string colorStr)
        {
            colorStr = colorStr.Replace("#", "");
            while (colorStr.Length < 8)
            {
                colorStr += "0";
            }

            string rStr = colorStr.Substring(0, 2);
            string gStr = colorStr.Substring(2, 2);
            string bStr = colorStr.Substring(4, 2);
            string aStr = colorStr.Substring(6, 2);

            byte r = byte.Parse(rStr, System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(gStr, System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(bStr, System.Globalization.NumberStyles.HexNumber);
            byte a = byte.Parse(aStr, System.Globalization.NumberStyles.HexNumber);

            byte[] color = new byte[4];
            color[0] = r;
            color[1] = g;
            color[2] = b;
            color[3] = a;

            return color;
        }

        public static int ColorToInt(byte[] color)
        {
            string rgbColor = ((int)(color[0])).ToString("X2") + ((int)(color[1])).ToString("X2") + ((int)(color[2])).ToString("X2") + ((int)(color[3])).ToString("X2");
            return int.Parse(rgbColor, System.Globalization.NumberStyles.HexNumber);
        }

        private static int RGBAtoARGB(int rgba)
        {
            int argb = (int)(rgba & 0xFFFFFF00) >> 8 | (rgba & 0x000000FF) << 24;
            //argb += (rgba & 0x000000FF) << 24;
            return argb;
        }

        private static int ARGBtoRGBA(int argb)
        {
            int rgba = ((int)(argb & 0x00FFFFFF) << 8 | (int)(argb & 0xFF000000) >> 24);
            //argb += (rgba & 0x000000FF) << 24;
            return rgba;
        }
    }
}