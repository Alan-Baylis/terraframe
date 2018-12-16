using System;
using System.Collections.Generic;
using System.IO;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    class FileInterface
    {
        public static string ReadString(string fileName)
        {
            try
            {
                return File.ReadAllText(fileName);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in File: " + fileName + "      Reason: " + ex.Message);
                return null;        
            }
        }

        public static void WriteString(string fileName, string data)
        {
            File.WriteAllText(fileName, data);
        }

        public static bool DoesFolderExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        public static bool DoesFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void CreateFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
        }

        public static string[] GetChildFolders(string path)
        {
            return Directory.GetDirectories(path);
        }

        public static string[] GetChildFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public static void DeleteFolder(string path)
        {
            Directory.Delete(path, true);
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static void SaveMeshFile_(string filename_, TerrainOutputMesh terrainOutputMesh)
        {
            List<string> fileNames = new List<string>();



            Console.WriteLine("Mesh chunks count: " + terrainOutputMesh.VertexData.Count);
            Console.WriteLine("Color count: " + terrainOutputMesh.MaterialColor.Count);

            for (int chunk = 0; chunk < terrainOutputMesh.VertexData.Count; chunk++)
            {
                string filename = filename_.Replace(".obj", "_" + chunk + ".obj");
                fileNames.Add(filename);
                StreamWriter sw = File.CreateText(filename);

                if(terrainOutputMesh.MaterialFile != null && terrainOutputMesh.MaterialFile.Length != 0)
                {
                    sw.WriteLine("mtllib " + PluginManager.PLUGINS_PATH + Path.DirectorySeparatorChar + terrainOutputMesh.MaterialFile);
                }

                Console.WriteLine("Mesh vertex count: " + terrainOutputMesh.VertexData[chunk].Count);
                Console.WriteLine("Mesh faces count: " + terrainOutputMesh.FacesData[chunk].Count);

                foreach (float[] vertex in terrainOutputMesh.VertexData[chunk])
                {
                    string line = "v ";
                    for (var i = 0; i < vertex.Length; i++)
                        line += vertex[i] + " ";
                    sw.WriteLine(line);
                }

                if (terrainOutputMesh.TexureCoordData.Count > 0 && terrainOutputMesh.TexureCoordData[chunk].Count > 0)
                {
                    foreach (float[] textureVertices in terrainOutputMesh.TexureCoordData[chunk])
                    {
                        string line = "vt ";
                        for (var i = 0; i < textureVertices.Length; i++)
                            line += textureVertices[i] + " ";
                        sw.WriteLine(line);
                    }
                }

                Dictionary<int, List<float[]>> vertexNormals = new Dictionary<int, List<float[]>>();
                Dictionary<int, int> vertexNormal = new Dictionary<int, int>();

                foreach (int[] face in terrainOutputMesh.FacesData[chunk])
                {
                    float[] n = triangleNormal(terrainOutputMesh.VertexData[chunk][face[0] - 1], terrainOutputMesh.VertexData[chunk][face[1] - 1], terrainOutputMesh.VertexData[chunk][face[2] - 1]);

                    for (var i = 0; i < face.Length; i++)
                    {
                        if (!vertexNormals.ContainsKey(face[i]))
                            vertexNormals.Add(face[i], new List<float[]>());
                        vertexNormals[face[i]].Add(n);
                    }
                }


                int vn = 1;
                foreach (int v in vertexNormals.Keys)
                {
                    float[] vnormal = meanNormal(vertexNormals[v]);

                    string line = "vn ";
                    for (var i = 0; i < vnormal.Length; i++)
                        line += vnormal[i] + " ";
                    sw.WriteLine(line);
                    vertexNormal.Add(v, vn);
                    vn++;
                }

                if(terrainOutputMesh.MaterialName.Count > 0 && terrainOutputMesh.MaterialName[chunk].Length > 0)
                    sw.WriteLine("usemtl " + terrainOutputMesh.MaterialName[chunk]);


                int j = 0;
                foreach (int[] face in terrainOutputMesh.FacesData[chunk])
                {
                    int[] textureVertex = null;
                    if (terrainOutputMesh.FacesTextureCoordData.Count > 0 && terrainOutputMesh.FacesTextureCoordData[chunk].Count > 0)
                    {
                        textureVertex = terrainOutputMesh.FacesTextureCoordData[chunk][j];
                    }

                    string line = "f ";
                    for (var i = 0; i < face.Length; i++)
                        line += face[i] + "/"+ (textureVertex==null?"":(""+textureVertex[i])) + "/" + vertexNormal[face[i]] + " ";
                    sw.WriteLine(line);

                    j++;
                }



                sw.Close();
            }

            terrainOutputMesh.VertexData.Clear();
            terrainOutputMesh.FacesData.Clear();
            terrainOutputMesh.MeshPath = fileNames;
        }

        public static void SaveMeshFile(string filename_, TerrainOutputMesh terrainOutputMesh)
        {
            if (terrainOutputMesh.NormalData != null && terrainOutputMesh.NormalData.Count > 0)
                return;

            List<string> fileNames = new List<string>();

            Console.WriteLine("Mesh chunks count: " + terrainOutputMesh.VertexData.Count);
            Console.WriteLine("Color count: " + terrainOutputMesh.MaterialColor.Count);

            Dictionary<string, List<float[]>> vertexNormals = new Dictionary<string, List<float[]>>();
            Dictionary<string, float[]> finalNormals = new Dictionary<string, float[]>();

            for (int chunk = 0; chunk < terrainOutputMesh.VertexData.Count; chunk++)
            {
                foreach (int[] face in terrainOutputMesh.FacesData[chunk])
                {
                    float[] n = triangleNormal(terrainOutputMesh.VertexData[chunk][face[0] - 1], terrainOutputMesh.VertexData[chunk][face[1] - 1], terrainOutputMesh.VertexData[chunk][face[2] - 1]);

                    for (var i = 0; i < face.Length; i++)
                    {
                        string vertexSignature = getVertexSignature(terrainOutputMesh.VertexData[chunk][face[i] - 1]);
                        if (!vertexNormals.ContainsKey(vertexSignature))
                            vertexNormals.Add(vertexSignature, new List<float[]>());
                        vertexNormals[vertexSignature].Add(n);
                    }
                }
            }

            foreach (string vertexSignature in vertexNormals.Keys)
            {
                float[] vnormal = meanNormal(vertexNormals[vertexSignature]);
                finalNormals.Add(vertexSignature, vnormal);
            }

            for (int chunk = 0; chunk < terrainOutputMesh.VertexData.Count; chunk++)
            {
                if (terrainOutputMesh.MaterialTexture.Count > 0 && terrainOutputMesh.MaterialTexture[chunk].Length > 0)
                {
                    terrainOutputMesh.MaterialTexture[chunk] = PluginManager.PLUGINS_PATH + Path.DirectorySeparatorChar + terrainOutputMesh.MaterialTexture[chunk];
                }


                terrainOutputMesh.NormalData.Add(new List<float[]>());

                foreach (float[] vertex in terrainOutputMesh.VertexData[chunk])
                {
                    string vertexSignature = getVertexSignature(vertex);
                    terrainOutputMesh.NormalData[chunk].Add(finalNormals[vertexSignature]);
                }

                terrainOutputMesh.FaceNormalData.Add(new List<int[]>());
                foreach (int[] face in terrainOutputMesh.FacesData[chunk])
                {
                    int[] faceNormals = new int[3];
                    faceNormals[0] = face[0];
                    faceNormals[1] = face[1];
                    faceNormals[2] = face[2];
                    terrainOutputMesh.FaceNormalData[chunk].Add(faceNormals);
                }
            }

            foreach (TerrainOutputMesh subMesh in terrainOutputMesh.SubMeshes)
                SaveMeshFile(filename_, subMesh);
        }

        private static string getVertexSignature(float[] vertex)
        {
            return vertex[0] + "#" + vertex[1] + "#" + vertex[2];
        }

        internal static void SaveImageFile(string filename, TerrainOutputImage terrainOutput)
        {
            Helpers.Instance.SaveImageFile(filename, terrainOutput.ImageData);

            terrainOutput.ImageData = null;
            terrainOutput.ImagePath = filename;
        }

        //internal static void SaveImageFile(string filename, int[][] imageData)
        //{
        //    Bitmap bmp = new Bitmap(imageData.Length, imageData[0].Length);

        //    for (int x = 0; x < imageData.Length; x++)
        //    {
        //        for (int y = 0; y < imageData[x].Length; y++)
        //        {
        //            bmp.SetPixel(x, y, Color.FromArgb(RGBAtoARGB(imageData[x][y])));
        //        }
        //    }

        //    bmp.Save(filename);
        //}

        private static float[] meanNormal(List<float[]> normals)
        {
            float x = 0;
            float y = 0;
            float z = 0;
            foreach (float[] normal in normals)
            {
                x += normal[0];
                y += normal[1];
                z += normal[2];
            }

            x /= normals.Count;
            y /= normals.Count;
            z /= normals.Count;

            float[] N = new float[] {x,y,z};

            float q = (Math.Abs(N[0]) + Math.Abs(N[1]) + Math.Abs(N[2]));
            if(q == 0)
            {
                return new float[] { 0, 0, 0 };
            }
            return new float[] { N[0] / q, N[1] / q, N[2] / q };
        }

        private static float[] triangleNormal(float[] v1, float[] v2, float[] v3)
        {
            float[] V = new float[] { v2[0] - v1[0], v2[1] - v1[1], v2[2] - v1[2] };
            float[] W = new float[] { v3[0] - v1[0], v3[1] - v1[1], v3[2] - v1[2] };
            float[] N = new float[] { V[1]*W[2]-V[2]*W[1], V[2]*W[0]-V[0]*W[2], V[0]*W[1]-V[1]*W[0] };

            float q = (Math.Abs(N[0]) + Math.Abs(N[1]) + Math.Abs(N[2]));

            if(q == 0)
            {
                return new float[] {0,0,0};
            }

            return new float[] { N[0] / q, N[1] / q, N[2] / q };
        }

        private static int RGBAtoARGB(int rgba)
        {
            int argb = (int)(rgba & 0xFFFFFF00)>>8 | (rgba & 0x000000FF) << 24;
            //argb += (rgba & 0x000000FF) << 24;
            return argb;
        }
    }
}
