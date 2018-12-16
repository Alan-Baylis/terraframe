using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGeneration_PluginBase.Helpers
{
    public class MeshGeneratorHelpers
    {
        public static void FillHeightMapMesh(TerrainOutputMesh splittedMesh, float[][] heightMap, float cellSize, float minHeight, float maxHeight, List<float[]> grassLand)
        {
            GeoPoint[][] heightMapGeoPoints = new GeoPoint[heightMap.Length][];
            for (int x = 0; x < heightMap.Length; x++)
            {
                heightMapGeoPoints[x] = new GeoPoint[heightMap[x].Length];
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    GeoPoint geoPoint = new GeoPoint();
                    geoPoint.Height = (float)heightMap[x][y];
                    geoPoint.X = x;
                    geoPoint.Y = y;
                    heightMapGeoPoints[x][y] = geoPoint;
                }
            }

            FillHeightMapMesh(splittedMesh, heightMapGeoPoints, cellSize, minHeight, maxHeight, grassLand);
        }

        public static void FillHeightMapMesh(TerrainOutputMesh splittedMesh, GeoPoint[][] heightMapGeoPoints, float cellSize, float minHeight, float maxHeight, List<float[]> grassLand)
        {
            float maxHeightValue = 0;

            for (int x = 0; x < heightMapGeoPoints.Length; x++)
            {
                for (int y = 0; y < heightMapGeoPoints[x].Length; y++)
                {
                    if (maxHeightValue < heightMapGeoPoints[x][y].Height)
                        maxHeightValue = heightMapGeoPoints[x][y].Height;
                }
            }


            int chunk = 0;
            float heightFactor = (maxHeight / maxHeightValue) / cellSize;
            TerrainOutput terrainOutputSplitted = splitTerrainSegments(heightMapGeoPoints);

            for (int x = 0; x < heightMapGeoPoints.Length; x++)
            {
                for (int y = 0; y < heightMapGeoPoints[x].Length; y++)
                {
                    if (heightMapGeoPoints[x][y].Type.Equals(CellType.Grass))
                        grassLand.Add(new float[] { heightMapGeoPoints[x][y].X, heightMapGeoPoints[x][y].Y, heightMapGeoPoints[x][y].Height * heightFactor });
                }
            }



            splittedMesh.CameraPosition = new float[] { 0, heightMapGeoPoints.Length / 4, 0 };
            splittedMesh.CameraRotation = new float[] { 22.5f, 45, 0 };

            foreach (CellType type in Enum.GetValues(typeof(CellType)))
            {
                splittedMesh.VertexData.Add(new List<float[]>());
                splittedMesh.FacesData.Add(new List<int[]>());
                splittedMesh.MaterialColor.Add("");
                splittedMesh.MaterialTexture.Add("");
                splittedMesh.TexureCoordData.Add(new List<float[]>());
                exportSplittedHeightMap(heightMapGeoPoints, splittedMesh, chunk, heightFactor, type);
                chunk++;
            }

            exportTransitionHeightMap_(heightMapGeoPoints, splittedMesh, heightFactor);
        }

        private static void exportTransitionHeightMap_(GeoPoint[][] heightMap, TerrainOutputMesh terrainOutputMesh, float heightFactor)
        {
            Dictionary<string, List<GeoPoint[]>> tempFaces = new Dictionary<string, List<GeoPoint[]>>();
            GeoPoint[] tempFace;

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    //heightMap[x][y].VertexIdx = -1;
                    //heightMap[x][y].uv = null;
                    heightMap[x][y].uvMap = new Dictionary<string, List<float[]>>();
                    heightMap[x][y].currentUv = 0;
                }
            }

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 1; y < heightMap[x].Length; y++)
                {
                    //terrainOutputMesh.VertexData[chunk].Add(new float[] { x, (float)(heightMap[x][y].Height * heightFactor), y });
                    //tempVertices.Add(heightMap[x][y]);

                    if (x < (heightMap[x].Length - 1)
                        && (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        || heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        || heightMap[x][y].Type != heightMap[x + 1][y].Type))
                    {
                        if (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // A
                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);

                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);
                        }

                        if (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // B
                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);

                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);
                        }

                        if (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y].Type)
                        {
                            // C
                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);
                        }

                        if (heightMap[x][y].Type == heightMap[x][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // D
                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);
                        }

                        if (heightMap[x][y].Type == heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y].Type)
                        {
                            // E
                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);

                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);
                        }

                        if (heightMap[x][y].Type != heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y].Type)
                        {
                            // F
                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);

                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);
                        }

                        if (heightMap[x][y].Type == heightMap[x][y - 1].Type
                        && heightMap[x][y].Type != heightMap[x + 1][y - 1].Type
                        && heightMap[x][y].Type == heightMap[x + 1][y].Type)
                        {
                            // G
                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);

                            tempFace = new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                };

                            addTempFaceList(tempFaces, tempFace);
                        }
                    }
                }
            }

            foreach (string signature in tempFaces.Keys)
            {
                System.Console.Out.WriteLine("# Signature: " + signature);
                int vertexIdx = 1;

                /*foreach (GeoPoint[] geoPoints in tempFaces[signature])
                {
                    foreach (GeoPoint geoPoint in geoPoints)
                    {
                        geoPoint.VertexIdx = -1;
                    }
                }*/

                terrainOutputMesh.VertexData.Add(new List<float[]>());
                terrainOutputMesh.FacesData.Add(new List<int[]>());
                terrainOutputMesh.MaterialColor.Add("#ffffffff");
                terrainOutputMesh.MaterialTexture.Add("HeightMapMaterials" + System.IO.Path.DirectorySeparatorChar + "transitionTexture.png");
                terrainOutputMesh.TexureCoordData.Add(new List<float[]>());
                int chunk = terrainOutputMesh.VertexData.Count - 1;

                foreach (GeoPoint[] geoPoints in tempFaces[signature])
                {
                    foreach (GeoPoint geoPoint in geoPoints)
                    {
                        terrainOutputMesh.VertexData[chunk].Add(new float[] { geoPoint.X, (float)(geoPoint.Height * heightFactor), geoPoint.Y });
                        terrainOutputMesh.TexureCoordData[chunk].Add(geoPoint.uvMap[signature][0]);
                        geoPoint.uvMap[signature].RemoveAt(0);
                    }

                    terrainOutputMesh.FacesData[chunk].Add(new int[] {
                    vertexIdx,
                    vertexIdx+1,
                    vertexIdx+2,
                    });

                    vertexIdx += 3;
                }
            }
        }

        private static void addTempFaceList(Dictionary<string, List<GeoPoint[]>> tempFaces, GeoPoint[] tempFace)
        {
            string signature = getFaceSignature(tempFace);
            if (!tempFaces.ContainsKey(signature))
                tempFaces.Add(signature, new List<GeoPoint[]>());
            tempFaces[signature].Add(tempFace);

            setTransitionUv_(signature, tempFace);
        }

        private static void setTransitionUv_(string signature, GeoPoint[] tempFace)
        {
            List<float[]> faceUvs = new List<float[]>();

            switch (signature)
            {
                case "#Grass#Grass#Grass":
                    faceUvs.Add(new float[] { 0, 0.666f });
                    faceUvs.Add(new float[] { 0.333f, 0.666f });
                    faceUvs.Add(new float[] { 0, 1 });
                    break;
                case "#Grass#Rock#Grass":
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 0, 0 });
                    faceUvs.Add(new float[] { 0.333f, 1 });
                    break;
                case "#Grass#Snow#Grass":
                    faceUvs.Add(new float[] { 0, 0.666f });
                    faceUvs.Add(new float[] { 1, 0.666f });
                    faceUvs.Add(new float[] { 0, 1 });
                    break;
                case "#Grass#Grass#Rock":
                    faceUvs.Add(new float[] { 0.333f, 1 });
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 0, 0 });
                    break;
                case "#Grass#Grass#Snow":
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 0, 0.666f });
                    faceUvs.Add(new float[] { 1, 1 });
                    break;
                case "#Grass#Rock#Rock":
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 0, 0 });
                    faceUvs.Add(new float[] { 0.333f, 0 });
                    break;
                case "#Grass#Snow#Snow":
                    faceUvs.Add(new float[] { 0, 0.666f });
                    faceUvs.Add(new float[] { 1, 0.666f });
                    faceUvs.Add(new float[] { 1, 1 });
                    break;

                case "#Rock#Rock#Rock":
                    faceUvs.Add(new float[] { 0, 0 });
                    faceUvs.Add(new float[] { 0.333f, 0 });
                    faceUvs.Add(new float[] { 0, 0.333f });
                    break;
                case "#Rock#Grass#Rock":
                    faceUvs.Add(new float[] { 0.333f, 0 });
                    faceUvs.Add(new float[] { 0.333f, 1 });
                    faceUvs.Add(new float[] { 0, 0 });
                    break;
                case "#Rock#Snow#Rock":
                    faceUvs.Add(new float[] { 1, 0 });
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0.666f, 0 });
                    break;
                case "#Rock#Rock#Grass":
                    faceUvs.Add(new float[] { 0, 0 });
                    faceUvs.Add(new float[] { 0.333f, 0 });
                    faceUvs.Add(new float[] { 0.333f, 1 });
                    break;
                case "#Rock#Rock#Snow":
                    faceUvs.Add(new float[] { 0.666f, 0 });
                    faceUvs.Add(new float[] { 1, 0 });
                    faceUvs.Add(new float[] { 1, 1 });
                    break;
                case "#Rock#Grass#Grass":
                    faceUvs.Add(new float[] { 0.333f, 0 });
                    faceUvs.Add(new float[] { 0.333f, 1 });
                    faceUvs.Add(new float[] { 0, 1 });
                    break;
                case "#Rock#Snow#Snow":
                    faceUvs.Add(new float[] { 1, 0 });
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0.666f, 1 });
                    break;

                case "#Snow#Snow#Snow":
                    faceUvs.Add(new float[] { 1, 0.666f });
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0.666f, 1 });
                    break;
                case "#Snow#Grass#Snow":
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 1, 0.666f });
                    break;
                case "#Snow#Rock#Snow":
                    faceUvs.Add(new float[] { 0.666f, 1 });
                    faceUvs.Add(new float[] { 0.666f, 0 });
                    faceUvs.Add(new float[] { 1, 1 });
                    break;
                case "#Snow#Snow#Grass":
                    faceUvs.Add(new float[] { 1, 0.666f });
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0, 1 });
                    break;
                case "#Snow#Snow#Rock":
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0.666f, 1 });
                    faceUvs.Add(new float[] { 0.666f, 0 });
                    break;
                case "#Snow#Grass#Grass":
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 0, 0.666f });
                    break;
                case "#Snow#Rock#Rock":
                    faceUvs.Add(new float[] { 0.666f, 1 });
                    faceUvs.Add(new float[] { 0.666f, 0 });
                    faceUvs.Add(new float[] { 1, 0 });
                    break;

                case "#Grass#Rock#Snow":
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 0.5f, 0 });
                    faceUvs.Add(new float[] { 1, 1 });
                    break;
                case "#Grass#Snow#Rock":
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0.5f, 0 });
                    break;
                case "#Rock#Grass#Snow":
                    faceUvs.Add(new float[] { 0.5f, 0 });
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 1, 1 });
                    break;
                case "#Rock#Snow#Grass":
                    faceUvs.Add(new float[] { 0.5f, 0 });
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0, 1 });
                    break;
                case "#Snow#Grass#Rock":
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0, 1 });
                    faceUvs.Add(new float[] { 0.5f, 0 });
                    break;
                case "#Snow#Rock#Grass":
                    faceUvs.Add(new float[] { 1, 1 });
                    faceUvs.Add(new float[] { 0.5f, 0 });
                    faceUvs.Add(new float[] { 0, 1 });
                    break;
            }

            for (int i = 0; i < tempFace.Length; i++)
            {
                if (!tempFace[i].uvMap.ContainsKey(signature))
                    tempFace[i].uvMap.Add(signature, new List<float[]>());
                tempFace[i].uvMap[signature].Add(faceUvs[i]);
            }
        }

        private static string getFaceSignature(GeoPoint[] tempFace)
        {
            string signature = "";
            foreach (GeoPoint gp in tempFace)
            {
                signature += ("#" + gp.Type);
            }

            return signature;
        }

        private static TerrainOutputImage splitTerrainSegments(GeoPoint[][] heightMap)
        {
            TerrainOutputImage terrainOutputSegments = new TerrainOutputImage();
            terrainOutputSegments.Title = "Segments";
            terrainOutputSegments.Key = "segments";


            int[][] splittedTerrainSegments = new int[heightMap.Length][];
            float maxHeight = 0;
            for (int x = 0; x < heightMap.Length; x++)
            {
                splittedTerrainSegments[x] = new int[heightMap[x].Length];
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    if (heightMap[x][y].Height > maxHeight)
                        maxHeight = heightMap[x][y].Height;
                }
            }

            float snowThreshold = 0.6f * maxHeight;

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    float inclination = 0;
                    List<GeoPoint> neighbours = GeoPoint.getAdjacentCells(heightMap, heightMap[x][y]);

                    float higherNeighbour = 0;
                    int higherCount = 0;
                    foreach (GeoPoint geoPoint in neighbours)
                    {
                        if (geoPoint.Height > heightMap[x][y].Height)
                        {
                            higherNeighbour += geoPoint.Height;
                            higherCount++;
                        }
                    }
                    higherNeighbour = higherNeighbour / (float)higherCount;

                    if (higherNeighbour != 0)
                        inclination = Math.Abs(heightMap[x][y].Height - higherNeighbour);

                    if (inclination > 0.02f)
                    {
                        //is rock
                        splittedTerrainSegments[x][y] = (0xff << 24) | (0x00 << 16) | (0x00 << 8) | 0xff;
                        heightMap[x][y].Type = CellType.Rock;
                    }
                    else if (heightMap[x][y].Height > snowThreshold)
                    {
                        //is snow
                        splittedTerrainSegments[x][y] = (0xff << 24) | (0xff << 16) | (0xff << 8) | 0xff;
                        heightMap[x][y].Type = CellType.Snow;
                    }
                    else
                    {
                        //is grass
                        splittedTerrainSegments[x][y] = (0x00 << 24) | (0xff << 16) | (0x00 << 8) | 0xff;
                        heightMap[x][y].Type = CellType.Grass;
                    }
                }
            }

            terrainOutputSegments.ImageData = splittedTerrainSegments;
            return terrainOutputSegments;
        }

        private static void exportSplittedHeightMap(GeoPoint[][] heightMap, TerrainOutputMesh terrainOutputMesh, int chunk, float heightFactor, CellType type)
        {
            //string color = "#00ff00ff";
            string texture = "grassTexture.png";
            switch (type)
            {
                case CellType.Snow:
                    //color = "#ffffffff";
                    texture = "snowTexture.png";
                    break;
                case CellType.Rock:
                    //color = "#999999ff";
                    texture = "rockTexture.png";
                    break;
                case CellType.Grass:
                    //color = "#00ff00ff";
                    texture = "grassTexture.png";
                    break;
            }
            string color = "#ffffffff";

            terrainOutputMesh.MaterialColor[chunk] = color;
            terrainOutputMesh.MaterialTexture[chunk] = "HeightMapMaterials" + System.IO.Path.DirectorySeparatorChar + texture;

            List<GeoPoint> tempVertices = new List<GeoPoint>();
            List<GeoPoint[]> tempFaces = new List<GeoPoint[]>();

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    heightMap[x][y].VertexIdx = -1;
                }
            }

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 1; y < heightMap[x].Length; y++)
                {
                    if (heightMap[x][y].Type == type)
                    {
                        //terrainOutputMesh.VertexData[chunk].Add(new float[] { x, (float)(heightMap[x][y].Height * heightFactor), y });
                        tempVertices.Add(heightMap[x][y]);

                        if (x < (heightMap[x].Length - 1))
                        {
                            if (heightMap[x][y - 1].Type == type && heightMap[x + 1][y - 1].Type == type && heightMap[x + 1][y].Type == type)
                            {
                                // A
                                tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });

                                tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });
                            }
                            if (heightMap[x][y - 1].Type == type && heightMap[x + 1][y - 1].Type != type && heightMap[x + 1][y].Type == type)
                            {
                                // B
                                tempFaces.Add(new GeoPoint[] {
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x][y-1]
                                });
                            }

                            // C
                            // D

                            if (heightMap[x][y - 1].Type != type && heightMap[x + 1][y - 1].Type == type && heightMap[x + 1][y].Type == type)
                            {
                                // E
                                tempFaces.Add(new GeoPoint[]{
                                    heightMap[x][y],
                                    heightMap[x+1][y],
                                    heightMap[x+1][y-1]
                                });
                            }
                            if (heightMap[x][y - 1].Type == type && heightMap[x + 1][y - 1].Type == type && heightMap[x + 1][y].Type != type)
                            {
                                // F
                                tempFaces.Add(new GeoPoint[]{
                                    heightMap[x][y],
                                    heightMap[x+1][y-1],
                                    heightMap[x][y-1]
                                });
                            }
                        }

                        if (x > 0 && heightMap[x - 1][y].Type != type && heightMap[x][y - 1].Type == type && heightMap[x - 1][y - 1].Type == type)
                        {
                            // G
                            tempFaces.Add(new GeoPoint[]{
                                    heightMap[x][y],
                                    heightMap[x][y - 1],
                                    heightMap[x - 1][y-1]
                                });
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

                        terrainOutputMesh.TexureCoordData[chunk].Add(new float[] {
                            (geoPoint.X % 2 == 0?0:1),
                            (geoPoint.Y % 2 == 0?0:1)
                        });
                    }
                }

                terrainOutputMesh.FacesData[chunk].Add(new int[] {
                    geoPoints[0].VertexIdx,
                    geoPoints[1].VertexIdx,
                    geoPoints[2].VertexIdx,
                });
            }
        }
    }
}
