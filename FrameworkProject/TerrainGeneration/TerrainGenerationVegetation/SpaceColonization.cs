using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGeneration_Core;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Vegetation
{
    class SpaceColonization
    {
        public static Random random = new Random();
        public static int branchSize = 15;

        public static List<TerrainOutput> createTree(int width, int height, int depth, int minDist, int maxDist, int pointCount, int[][] treeTopSketch, int leafCount, int leafSize, int maxCenterOffset, int branchSize)
        {
            SpaceColonization.branchSize = branchSize;
            treeTopSketch = Helpers.Instance.ResizePixels(treeTopSketch, treeTopSketch.Length, treeTopSketch.Length, width, height);
            List <TreeSection> treeSections = new List<TreeSection>();
            createTreeTopCircunferenceLookUp(treeTopSketch, treeSections);

            List<TreeSection> treeSectionOdds = new List<TreeSection>();
            for(int i = 0; i < treeSections.Count; i++)
            {
                for(int j = 0; j < treeSections[i].EndX- treeSections[i].StartX; j++)
                {
                    treeSectionOdds.Add(treeSections[i]);
                }
            }

            Tree tree = new Tree(pointCount, width, height, depth, minDist, maxDist, treeSectionOdds);

            TerrainOutputMesh terrainOutputMesh = new TerrainOutputMesh();
            terrainOutputMesh.Key = "TreeMesh";
            terrainOutputMesh.Title = "Tree Mesh";
            terrainOutputMesh.CameraPosition = new float[] { width/2, height/2, -depth };
            terrainOutputMesh.CameraRotation = new float[] { 0, 0, 0 };
            terrainOutputMesh.MaterialFile = "VegetationMaterials"+ System.IO.Path.DirectorySeparatorChar + "material.mtl";

            System.Console.WriteLine("Gen A");

            terrainOutputMesh.VertexData.Add(new List<float[]>());
            terrainOutputMesh.FacesData.Add(new List<int[]>());
            terrainOutputMesh.TexureCoordData.Add(new List<float[]>());
            terrainOutputMesh.FacesTextureCoordData.Add(new List<int[]>());
            terrainOutputMesh.MaterialName.Add("Branches");
            terrainOutputMesh.MaterialColor.Add("#ffffffff");
            terrainOutputMesh.MaterialMode.Add(TerrainOutputMesh.RenderingMode.opaque);

            terrainOutputMesh.MaterialTexture.Add("VegetationMaterials" + System.IO.Path.DirectorySeparatorChar + "wood.png");

            System.Console.WriteLine("Gen B");

            tree.fill3DMesh(terrainOutputMesh);

            System.Console.WriteLine("Gen C");

            terrainOutputMesh.VertexData.Add(new List<float[]>());
            terrainOutputMesh.FacesData.Add(new List<int[]>());
            terrainOutputMesh.TexureCoordData.Add(new List<float[]>());
            terrainOutputMesh.FacesTextureCoordData.Add(new List<int[]>());
            terrainOutputMesh.MaterialName.Add("Leaves");
            terrainOutputMesh.MaterialColor.Add("#ffffffff");
            terrainOutputMesh.MaterialMode.Add(TerrainOutputMesh.RenderingMode.fade);
            terrainOutputMesh.MaterialTexture.Add("VegetationMaterials" + System.IO.Path.DirectorySeparatorChar + "leafTexture.png");

            System.Console.WriteLine("Gen D");

            tree.fill3DLeafMesh(terrainOutputMesh, leafCount, leafSize, maxCenterOffset);

            System.Console.WriteLine("Gen E");

            List<TerrainOutput> outputs = new List<TerrainOutput>();
            outputs.Add(terrainOutputMesh);
            /*outputs.Add(createImage(tree, "Tree XY", "TreeXY", "xy", width, height));
            outputs.Add(createImage(tree, "Tree ZY", "TreeZY", "zy", depth, height));
            outputs.Add(createImage(tree, "Tree XZ", "TreeXZ", "xz", width, depth));*/

            return outputs;
        }

        //public static int ColorToInt(Color color)
        //{
        //    string rgbColor = ((int)(color.R)).ToString("X2") + ((int)(color.G)).ToString("X2") + ((int)(color.B)).ToString("X2") + ((int)(color.A)).ToString("X2");
        //    return int.Parse(rgbColor, System.Globalization.NumberStyles.HexNumber);
        //}

        private static int RGBAtoARGB(int rgba)
        {
            int argb = (int)(rgba & 0xFFFFFF00) >> 8 | (rgba & 0x000000FF) << 24;
            //argb += (rgba & 0x000000FF) << 24;
            return argb;
        }

        private static void createTreeTopCircunferenceLookUp(int[][] treeTopSketch, List<TreeSection> treeSections)
        {
            int[][] treeTopSketch_ = new int[treeTopSketch.Length][];

            for (int x = 0; x < treeTopSketch.Length; x++)
            {
                treeTopSketch_[x] = new int[treeTopSketch.Length];
                for (int y = 0; y < treeTopSketch[x].Length; y++)
                {
                    treeTopSketch_[x][y] = treeTopSketch[y][x];
                }
            }

            treeTopSketch = treeTopSketch_;

            for (int x = 0; x < treeTopSketch.Length; x++)
            {
                int currentStart = 0;
                int currentEnd = 0;
                bool treeFound = false;
                for(int y = 0; y < treeTopSketch[x].Length; y++)
                {
                    
                    if(treeTopSketch[x][y] == 0x00ff33ff)
                    {
                        if (!treeFound)
                        {
                            currentStart = y;
                        }
                        treeFound = true;
                        currentEnd = y;
                    }
                }

                if (currentStart != currentEnd)
                {

                    treeSections.Add(new TreeSection(x, currentStart, currentEnd));
                }
            }

            if(treeSections.Count == 0)
            {
                for (int x = 0; x < treeTopSketch.Length; x++)
                {
                    treeSections.Add(new TreeSection(x, 0, treeTopSketch.Length));
                }
            }
        }
        
        //public static TerrainOutputImage createImage(Tree tree, string title, string key, string type, int width, int height)
        //{
        //    Bitmap bitmap = new Bitmap(width, height);
        //    Graphics graphics = Graphics.FromImage(bitmap);
        //    graphics.TranslateTransform(0, height);
        //    graphics.ScaleTransform(1, -1);
        //    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        //    graphics.Clear(Color.Gray);

        //    if(type.Equals("xy"))
        //        tree.drawXY(graphics);
        //    else if (type.Equals("zy"))
        //        tree.drawZY(graphics);
        //    else if (type.Equals("xz"))
        //        tree.drawXZ(graphics);


        //    TerrainOutputImage terrainOutputHeightMap = new TerrainOutputImage();
        //    terrainOutputHeightMap.Title = title;
        //    terrainOutputHeightMap.Key = key;

        //    terrainOutputHeightMap.ImageData = new int[bitmap.Width][];

        //    for (int i = 0; i < bitmap.Width; i++)
        //    {
        //        terrainOutputHeightMap.ImageData[i] = new int[bitmap.Height];
        //        for (int j = 0; j < bitmap.Height; j++)
        //        {
        //            Color color = bitmap.GetPixel(i, j);
        //            terrainOutputHeightMap.ImageData[i][j] = (color.R << 24) | (color.G << 16) | (color.B << 8) | 0xff;
        //        }
        //    }

        //    return terrainOutputHeightMap;
        //} 
    }

    class Tree
    {
        List<Leaf> leaves;
        List<Branch> branches;
        HashSet<Branch> branchesWithoutSuccessor;
        private int activeLeafs = 0;
        public int branchSides = 3;

        public static int MAX_LEVEL = 1;

        public Tree(int leafCount, int width, int height, int depth, int minDist, int maxDist, List<TreeSection> treeSections)
        {
            activeLeafs = leafCount;
            leaves = new List<Leaf>();
            branches = new List<Branch>();
            for (int i = 0; i < leafCount; i++)
            {
                leaves.Add(new Leaf(width, height, depth, treeSections));
            }

            branches.Add(new Branch(null, width / 2, 0, depth / 2, 0, 1, 0, 1));

            bool found = false;
            Branch currentBranch = branches[0];

            System.Console.WriteLine("Tree A");

            while (!found 
                && branches[branches.Count - 1].Pos.Vx >= 0 && branches[branches.Count - 1].Pos.Vx <= width
                && branches[branches.Count - 1].Pos.Vy >= 0 && branches[branches.Count - 1].Pos.Vy <= height
                && branches[branches.Count - 1].Pos.Vz >= 0 && branches[branches.Count - 1].Pos.Vz <= depth)
            {
                foreach(Leaf leaf in leaves)
                {
                    float d = (float)Math.Sqrt(Math.Pow(leaf.Pos.Vx - currentBranch.Pos.Vx, 2) + Math.Pow(leaf.Pos.Vy - currentBranch.Pos.Vy, 2) + Math.Pow(leaf.Pos.Vz - currentBranch.Pos.Vz, 2));
                    if (d < maxDist)
                    {
                        found = true;
                    }

                    if (!found)
                    {
                        Branch nextBranch = currentBranch.next();
                        branches.Add(nextBranch);
                        currentBranch = nextBranch;
                    }
                }
            }

            System.Console.WriteLine("Tree B");

            int previousActiveLeafs = activeLeafs;
            int previousBranchCount = branches.Count;
            int maxNonLeafTries = 10;
            do {
                if(previousActiveLeafs == activeLeafs)
                {
                    maxNonLeafTries--;
                }
                else
                {
                    maxNonLeafTries = 10;
                }


                previousActiveLeafs = activeLeafs;
                previousBranchCount = branches.Count;
                traceTree(minDist, maxDist);
            } while ((previousActiveLeafs != activeLeafs || previousBranchCount != branches.Count) && maxNonLeafTries > 0 );

            System.Console.WriteLine("Tree CC");

            branchesWithoutSuccessor = new HashSet<Branch>(branches);

            foreach (Branch branch in branches)
            {
                if(branch.Parent != null && branchesWithoutSuccessor.Contains(branch.Parent))
                    branchesWithoutSuccessor.Remove(branch.Parent);
            }

            System.Console.WriteLine("Tree D");

            foreach (Branch branch in branchesWithoutSuccessor)
            {
                processThickness(branch, 0);
            }

            System.Console.WriteLine("Tree E");
        }

        private void traceTree(int minDist, int maxDist)
        {
            System.Console.WriteLine("Trace- activeLeafs: " + activeLeafs + " branches: " + branches.Count );
            foreach (Leaf leaf in leaves)
            {
                if (!leaf.Reached)
                {
                    Branch closestBranch = null;
                    float minDistance = 99999999;

                    foreach (Branch branch in branches)
                    {
                        float d = (float)Math.Sqrt(Math.Pow(leaf.Pos.Vx - branch.Pos.Vx, 2) + Math.Pow(leaf.Pos.Vy - branch.Pos.Vy, 2) + Math.Pow(leaf.Pos.Vz - branch.Pos.Vz, 2));
                        if (d < minDist)
                        {
                            leaf.Reached = true;
                            activeLeafs--;
                        }
                        else if ((closestBranch == null || d < minDistance) && d < maxDist)
                        {
                            closestBranch = branch;
                            minDistance = d;
                        }
                    }

                    if(closestBranch != null)
                    {
                        float dX = leaf.Pos.Vx - closestBranch.Pos.Vx;
                        float dY = leaf.Pos.Vy - closestBranch.Pos.Vy;
                        float dZ = leaf.Pos.Vz - closestBranch.Pos.Vz;

                        Vector newDir = new Vector(dX, dY, dZ);
                        newDir.normalize();

                        closestBranch.Dir.Vx += newDir.Vx;
                        closestBranch.Dir.Vy += newDir.Vy;
                        closestBranch.Dir.Vz += newDir.Vz;

                        closestBranch.Dir.normalize();

                        closestBranch.count++;
                    }
                }
            }

            List<Branch> newBranches = new List<Branch>();

            foreach(Branch branch in branches)
            {
                if(branch.count > 0)
                {
                    newBranches.Add(branch.next());
                    branch.reset();
                }
            }

            branches.AddRange(newBranches);
        }

        public void processThickness(Branch branch, int width)
        {
            if (branch.WidthFactor < width)
                branch.WidthFactor = width;

            if (branch.Parent != null)
            {
                processThickness(branch.Parent, width + 1);
            }
        }

        //public void drawXY(Graphics graphics)
        //{
        //    foreach (Branch branch in branches)
        //    {
        //        if (branch.Parent != null)
        //        {
        //            float thickness = (((float)Tree.MAX_LEVEL - (float)branch.Level)/(float)Tree.MAX_LEVEL)*30.0f;
        //            Pen pen = new Pen(Color.Brown, thickness);
        //            graphics.DrawLine(pen, branch.Parent.Pos.Vx, branch.Parent.Pos.Vy, branch.Pos.Vx, branch.Pos.Vy);
        //        }
        //    }

        //    foreach (Leaf leaf in leaves)
        //    {
        //        graphics.FillEllipse(Brushes.Blue, leaf.Pos.Vx - 2, leaf.Pos.Vy - 2, 5, 5);
        //    }
        //}

        //public void drawZY(Graphics graphics)
        //{
        //    foreach (Branch branch in branches)
        //    {
        //        if (branch.Parent != null)
        //        {
        //            float thickness = (((float)Tree.MAX_LEVEL - (float)branch.Level) / (float)Tree.MAX_LEVEL) * 30.0f;
        //            Pen pen = new Pen(Color.Brown, thickness);
        //            graphics.DrawLine(pen, branch.Parent.Pos.Vz, branch.Parent.Pos.Vy, branch.Pos.Vz, branch.Pos.Vy);

        //        }
        //    }

        //    foreach (Leaf leaf in leaves)
        //    {
        //        graphics.FillEllipse(Brushes.Blue, leaf.Pos.Vz - 2, leaf.Pos.Vy - 2, 5, 5);
        //    }
        //}

        //public void drawXZ(Graphics graphics)
        //{
        //    foreach (Branch branch in branches)
        //    {
        //        if (branch.Parent != null)
        //        {
        //            float thickness = (((float)Tree.MAX_LEVEL - (float)branch.Level) / (float)Tree.MAX_LEVEL) * 30.0f;
        //            Pen pen = new Pen(Color.Brown, thickness);
        //            graphics.DrawLine(pen, branch.Parent.Pos.Vx, branch.Parent.Pos.Vz, branch.Pos.Vx, branch.Pos.Vz);
        //        }
        //    }

        //    foreach(Leaf leaf in leaves)
        //    {
        //        graphics.FillEllipse(Brushes.Blue, leaf.Pos.Vx - 2, leaf.Pos.Vz - 2, 5, 5);
        //    }
        //}

        float textureStepJump = 0.2f;
        public void fill3DMesh(TerrainOutputMesh terrainOutputMesh)
        {
            int maxWidthFactor = 20;
            if (branches[0].WidthFactor > 0)
            {
                maxWidthFactor = branches[0].WidthFactor;
            }

            float textureSideStep = 1.0f / branchSides;

            foreach (Branch branch in branches)
            {
                float textureStep = ((float)(maxWidthFactor - branch.WidthFactor)) * textureStepJump;
                float thickness = ((float)branch.WidthFactor / (float)maxWidthFactor) * 30.0f;
                if (branch.Parent != null)
                {
                    if (branch.WidthFactor == 0)
                    {
                        terrainOutputMesh.VertexData[0].Add(new float[] {
                            branch.Pos.Vx,
                            branch.Pos.Vy,
                            branch.Pos.Vz
                        });

                        terrainOutputMesh.TexureCoordData[0].Add(new float[] {
                            0.5f,
                            textureStep
                        });

                        int pointVertex = terrainOutputMesh.VertexData[0].Count;
                        for (int i = 0; i < branchSides - 1; i++)
                        {
                            terrainOutputMesh.FacesData[0].Add(new int[] {
                            branch.Parent.vertexes[i],
                            branch.Parent.vertexes[i+1],
                            pointVertex});
                        }

                        terrainOutputMesh.FacesData[0].Add(new int[] {
                        branch.Parent.vertexes[branchSides-1],
                        branch.Parent.vertexes[0],
                        pointVertex});
                    }
                    else
                    {
                        branch.vertexes = fill3DBranch(branch.Parent.Pos, branch.Pos, thickness, branchSides, terrainOutputMesh, branch.Parent.vertexes);

                        for (int i = 0; i < branchSides; i++)
                        {
                            terrainOutputMesh.TexureCoordData[0].Add(new float[] {
                                textureSideStep*i,
                                textureStep
                            });
                        }
                    }
                }
                else
                {
                    Vector oldPos = new Vector(
                        branch.Pos.Vx - branch.Dir.Vx,
                        branch.Pos.Vy - branch.Dir.Vy,
                        branch.Pos.Vz - branch.Dir.Vz
                        );
                    fillCircle(branch.Pos, oldPos, branchSides, thickness, terrainOutputMesh.VertexData[0], terrainOutputMesh.FacesData[0], (float)Math.PI, false);

                    for(int i = 0; i < branchSides; i++)
                    {
                        terrainOutputMesh.TexureCoordData[0].Add(new float[] {
                            textureSideStep*i,
                            textureStep
                        });
                    }


                    List<int> oldVertex = new List<int>();

                    for(int i = 1; i <= branchSides; i++)
                        oldVertex.Add(i);

                    branch.vertexes = oldVertex;
                }
            }

        }


        public void fill3DLeafMesh(TerrainOutputMesh terrainOutputMesh, int leafCount, int leafSize, int maxCenterOffset)
        {
            int currentVertexAndFaceIdx = 1;

            List<Branch> terminalBranches = new List<Branch>(branchesWithoutSuccessor);

            for(int i = 0; i < leafCount; i++)
            {

                System.Console.WriteLine("Leaf A");

                Branch branch = terminalBranches[SpaceColonization.random.Next(terminalBranches.Count)];
                addLeaf(branch.Pos, leafSize, maxCenterOffset, terrainOutputMesh, currentVertexAndFaceIdx);

                if (terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count > 4000)
                {
                    terrainOutputMesh.VertexData.Add(new List<float[]>());
                    terrainOutputMesh.FacesData.Add(new List<int[]>());
                    terrainOutputMesh.TexureCoordData.Add(new List<float[]>());
                    terrainOutputMesh.FacesTextureCoordData.Add(new List<int[]>());
                    terrainOutputMesh.MaterialName.Add("Leaves");
                    terrainOutputMesh.MaterialColor.Add("#ffffffff");
                    terrainOutputMesh.MaterialMode.Add(TerrainOutputMesh.RenderingMode.transparent);
                    terrainOutputMesh.MaterialTexture.Add("VegetationMaterials" + System.IO.Path.DirectorySeparatorChar + "leafTexture.png");
                    currentVertexAndFaceIdx++;
                }

            }
        }

        public void addLeaf(Vector position, float size, float maxCenterOffset, TerrainOutputMesh terrainOutputMesh, int currentVertexAndFaceIdx)
        {
            Vector center = new Vector(
                (float)(position.Vx - (maxCenterOffset / 2.0f) + maxCenterOffset * SpaceColonization.random.NextDouble()),
                (float)(position.Vy - (maxCenterOffset / 2.0f) + maxCenterOffset * SpaceColonization.random.NextDouble()),
                (float)(position.Vz - (maxCenterOffset / 2.0f) + maxCenterOffset * SpaceColonization.random.NextDouble()));

            Vector direction = new Vector(
                   center.Vx + (1.0f / 2.0f - (float)SpaceColonization.random.NextDouble()),
                   center.Vy + (1.0f / 2.0f - (float)SpaceColonization.random.NextDouble()),
                   center.Vz + (1.0f / 2.0f - (float)SpaceColonization.random.NextDouble())
                );

            Vector center2 = new Vector(
            (float)(position.Vx - (maxCenterOffset / 2.0f) + maxCenterOffset * SpaceColonization.random.NextDouble())+0.0001f,
            (float)(position.Vy - (maxCenterOffset / 2.0f) + maxCenterOffset * SpaceColonization.random.NextDouble()),
            (float)(position.Vz - (maxCenterOffset / 2.0f) + maxCenterOffset * SpaceColonization.random.NextDouble()));

            fillCircle(center, direction, 4, size, terrainOutputMesh.VertexData[currentVertexAndFaceIdx], terrainOutputMesh.FacesData[currentVertexAndFaceIdx], 0, false);


            terrainOutputMesh.FacesData[currentVertexAndFaceIdx].Add(new int[] {
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-3,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-2,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count
            });

            terrainOutputMesh.FacesData[currentVertexAndFaceIdx].Add(new int[] {
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-2,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-1,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count
            });

            fillCircle(center2, direction, 4, size, terrainOutputMesh.VertexData[currentVertexAndFaceIdx], terrainOutputMesh.FacesData[currentVertexAndFaceIdx], 0, false);

            terrainOutputMesh.FacesData[currentVertexAndFaceIdx].Add(new int[] {
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-2,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-3,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count
            });

            terrainOutputMesh.FacesData[currentVertexAndFaceIdx].Add(new int[] {
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-1,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count-2,
                terrainOutputMesh.VertexData[currentVertexAndFaceIdx].Count
            });


            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 0.0f, 0.0f });
            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 1.0f, 0.0f });
            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 1.0f, 1.0f });
            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 0.0f, 1.0f });

            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 0.0f, 0.0f });
            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 1.0f, 0.0f });
            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 1.0f, 1.0f });
            terrainOutputMesh.TexureCoordData[currentVertexAndFaceIdx].Add(new float[] { 0.0f, 1.0f });
        }

        public List<int> fill3DBranch(Vector v1, Vector v2, float width, int sides, TerrainOutputMesh terrainOutputMesh, List<int> previousVertex)
        {
            int initialIdx = terrainOutputMesh.VertexData[0].Count;
            if (previousVertex == null)
            {
                fillCircle(v1, v2, sides, width, terrainOutputMesh.VertexData[0], terrainOutputMesh.FacesData[0], 0, true);
                fillCircle(v2, v1, sides, width, terrainOutputMesh.VertexData[0], terrainOutputMesh.FacesData[0], (float)Math.PI, true);


                int i;


                i = initialIdx + 2;
                terrainOutputMesh.FacesData[0].Add(new int[] {
                    i,
                    i+sides+1,
                    i+1 });
                terrainOutputMesh.FacesData[0].Add(new int[] {
                    i+sides+1,
                    i+sides+sides,
                    i+1 });


                for (i = 2; i < sides; i++)
                {
                    terrainOutputMesh.FacesData[0].Add(new int[] {
                    i+initialIdx+1,
                    initialIdx+2+sides+sides-(i-2),
                    i+initialIdx+2 });
                    terrainOutputMesh.FacesData[0].Add(new int[] {
                    initialIdx+2+sides+sides-(i-2),
                    initialIdx+2+sides+sides-(i-2)-1,
                    i+initialIdx+2 });
                }

                i = initialIdx + sides + 1;
                terrainOutputMesh.FacesData[0].Add(new int[] {
                    i,
                    initialIdx+2+sides+2,
                    initialIdx+2 });
                terrainOutputMesh.FacesData[0].Add(new int[] {
                    initialIdx+2+sides+2,
                    initialIdx+2+sides+1,
                    initialIdx+2 });
            }
            else
            {
                int initialVertices = terrainOutputMesh.VertexData[0].Count;
                fillCircle(v2, v1, sides, width, terrainOutputMesh.VertexData[0], terrainOutputMesh.FacesData[0], (float)Math.PI, false);


                terrainOutputMesh.FacesData[0].Add(new int[] {
                    previousVertex[0],
                    initialVertices + 1,
                    previousVertex[sides-1]});

                terrainOutputMesh.FacesData[0].Add(new int[] {
                    previousVertex[sides-1],
                    initialVertices + 1,
                    initialVertices + sides});


                for (int i = 1; i < sides; i++)
                {
                    terrainOutputMesh.FacesData[0].Add(new int[] {
                    previousVertex[i],
                    initialVertices + i + 1,
                    previousVertex[i-1]});

                    terrainOutputMesh.FacesData[0].Add(new int[] {
                    previousVertex[i-1],
                    initialVertices + i + 1,
                    initialVertices + i});
                }

                List<int> newPreviousVertex = new List<int>();
                for (int i = 1; i <= sides; i++)
                {
                    newPreviousVertex.Add(initialVertices + i);
                }

                return newPreviousVertex;
            }

            return null;
        }

        public void fillCircle(Vector center, Vector destination, int sides, float width, List<float[]> vertexData, List<int[]> facesData, float initialRotation, bool fill)
        {
            Vector normal = new Vector(center.Vx-destination.Vx, center.Vy - destination.Vy, center.Vz - destination.Vz);
            normal.normalize();
            Vector normalPerpendicular = normal.getPerpendicular();
            normalPerpendicular.normalize();
            float radius = width / 2.0f;

            //P(angle) = radius*Math.cos(angle)*normalPerpendicular + radius*Math.sin(angle)*normal x normalPerpendicular + center

            if(fill)
                vertexData.Add(new float[] { center.Vx, center.Vy, center.Vz });

            int centerIdx = vertexData.Count;

            for (int i = 0; i < sides; i++)
            {

                float angle = (float)((float)i * (2.0f * Math.PI / (float)sides)) + initialRotation;

                Vector temp1 = new Vector((float)(radius * Math.Cos(angle) * normalPerpendicular.Vx), (float)(radius * Math.Cos(angle) * normalPerpendicular.Vy), (float)(radius * Math.Cos(angle) * normalPerpendicular.Vz));

                Vector cross = new Vector(normal.Vy * normalPerpendicular.Vz - normal.Vz * normalPerpendicular.Vy,
                                    normal.Vz * normalPerpendicular.Vx - normal.Vx * normalPerpendicular.Vz,
                                    normal.Vx * normalPerpendicular.Vy - normal.Vy * normalPerpendicular.Vx);

                Vector temp2 = new Vector(  (float)(radius * Math.Sin(angle) * cross.Vx), 
                                            (float)(radius * Math.Sin(angle) * cross.Vy), 
                                            (float)(radius * Math.Sin(angle) * cross.Vz));

                /*Vector temp2 = new Vector((float)(radius * Math.Sin(angle) * normal.Vx), (float)(radius * Math.Sin(angle) * normal.Vy), (float)(radius * Math.Sin(angle) * normal.Vz));

                System.Console.WriteLine("Temp2: " + temp2.Vx + ", " + temp2.Vy + ", " + temp2.Vz);

                temp2 = new Vector( temp2.Vy * normalPerpendicular.Vz - temp2.Vz * normalPerpendicular.Vy,
                                    temp2.Vz * normalPerpendicular.Vx - temp2.Vx * normalPerpendicular.Vz,
                                    temp2.Vx * normalPerpendicular.Vy - temp2.Vy * normalPerpendicular.Vx);*/

                Vector point = new Vector(temp1.Vx + temp2.Vx + center.Vx,
                    temp1.Vy + temp2.Vy + center.Vy,
                    temp1.Vz + temp2.Vz + center.Vz);

                vertexData.Add(new float[] { point.Vx, point.Vy, point.Vz });

                if (fill)
                {
                    if (i > 0)
                    {
                        facesData.Add(new int[] { centerIdx, vertexData.Count() - 1, vertexData.Count() });
                    }
                }
            }

            if (fill)
            {
                facesData.Add(new int[] { centerIdx, vertexData.Count(), centerIdx + 1 });
            }
        }
    }

    class Branch
    {
        public Vector Pos { get; set; }
        public Vector Dir { get; set; }
        private Vector originalDir;
        public Branch Parent { get; set; }
        public int count { get; set; } = 0;
        public int Level { get; set; }
        public List<int> vertexes { get; set; }
        public int WidthFactor { get; set; } = 0;

        public Branch(Branch parent, float startX, float startY, float startZ, float dX, float dY, float dZ, int level)
        {
            Pos = new Vector(startX, startY, startZ);
            Dir = new Vector(dX, dY, dZ);
            Dir.normalize();
            Level = level;

            if (level > Tree.MAX_LEVEL)
                Tree.MAX_LEVEL = level;

            originalDir = Dir.copy();

            Parent = parent;
        }

        public void reset()
        {
            Dir = originalDir.copy();
            count = 0;
        }

        public Branch next()
        {
            return new Branch(this, Pos.Vx + (Dir.Vx)* SpaceColonization.branchSize, Pos.Vy + (Dir.Vy)* SpaceColonization.branchSize, Pos.Vz + (Dir.Vz)* SpaceColonization.branchSize, Dir.Vx, Dir.Vy, Dir.Vz, Level +1);
        }
    }

    class Leaf
    {
        public Vector Pos {get; set; }
        public bool Reached { get; set; } = false;
        public Leaf(int width, int height, int depth, List<TreeSection> treeSections)
        {
            Pos = treeSections[SpaceColonization.random.Next(treeSections.Count)].Point;
        }
    }

    class Vector
    {
        public float Vx { get; set; }
        public float Vy { get; set; }
        public float Vz { get; set; }
        public Vector(float vx, float vy, float vz)
        {
            Vx = vx;
            Vy = vy;
            Vz = vz;
        }

        public void normalize()
        {
            float length = (float)Math.Sqrt(Math.Pow(Vx, 2) + Math.Pow(Vy, 2) + Math.Pow(Vz, 2));

            if (length != 0)
            {
                Vx = Vx / length;
                Vy = Vy / length;
                Vz = Vz / length;
            }
        }

        public Vector copy()
        {
            return new Vector(Vx, Vy, Vz);
        }
        
        public Vector getPerpendicular()
        {
            if (Vx != 0)
                return new Vector(Vy, -Vx, 0);
            else if(Vy != 0)
                return new Vector(-Vy, Vx, 0);
            else
                return new Vector(0, -Vz, Vy);
        }
    }

    class TreeSection
    {
        private int Y { get; set; }
        public int StartX { get; set; }
        public int EndX { get; set; }

        float maxRadius;
        List<int> radiusLookup;

        public Vector Point
        {
            get
            {

                if (radiusLookup == null)
                {
                    maxRadius = ((EndX - StartX) / 2.0f);
                    radiusLookup = new List<int>();
                    for (int i = 0; i < maxRadius; i++)
                    {
                        int odds = (int)(2.0f * Math.PI * (float)i);
                        for (int j = 0; j < odds; j++)
                        {
                            radiusLookup.Add(i);
                        }
                    }
                }

                int currentRadius = radiusLookup[SpaceColonization.random.Next(radiusLookup.Count)];
                float angle = (float)(SpaceColonization.random.NextDouble()*(Math.PI*2.0f));

                float x = (float)(currentRadius * Math.Cos(angle));
                float z = (float)(currentRadius * Math.Sin(angle));

                return new Vector(StartX + maxRadius + x, Y, StartX + maxRadius + z);
            }
        }

        public TreeSection(int y, int startX, int endX)
        {
            Y = y;
            StartX = startX;
            EndX = endX;
        }
    }
}
