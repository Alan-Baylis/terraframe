using System;
using System.Collections.Generic;
using TerrainGeneration_PluginBase;
using TerrainGeneration_PluginBase.WorkflowData;

namespace TerrainGeneration_HeightMap
{
    class Plugin : TerrainPlugin
    {
        public string Author
        {
            get
            {
                return "Hugo P.";
            }
        }

        public string Description
        {
            get
            {
                return "Creates an eroded heightmap with ridges (Belhadj & Audibert 2005, Olsen 2004)";
            }
        }

        public HashSet<LayerType> In
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                return terrainTypes;
            }
        }

        public string Name
        {
            get
            {
                return "Heightmap: Diam. Sqr+Ridge+MDI";
            }
        }

        public HashSet<LayerType> Not
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                terrainTypes.Add(LayerType.WaterBodies);
                terrainTypes.Add(LayerType.Vegetation);
                terrainTypes.Add(LayerType.Roads);
                terrainTypes.Add(LayerType.Urban);
                return terrainTypes;
            }
        }

        public HashSet<LayerType> Out
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                terrainTypes.Add(LayerType.HeightMap);
                return terrainTypes;
            }
        }

        public int Progress
        {
            get
            {
                return 0;
            }
        }

        public List<TerrainInput> TerrainInputs
        {
            get
            {
                TerrainInputForm terrainParametersForm = new TerrainInputForm();
                terrainParametersForm.Title = "Parameters";
                terrainParametersForm.Key = "parameters";

                FormFieldInteger terrainSeed = new FormFieldInteger();
                terrainSeed.Title = "Seed";
                terrainSeed.Key = "seed";
                terrainSeed.Value = 4;

                FormFieldOptions terrainSizeOptions = new FormFieldOptions();
                terrainSizeOptions.Title = "Grid size";
                terrainSizeOptions.Key = "gridSize";
                terrainSizeOptions.Options.Add("65*65");
                terrainSizeOptions.Options.Add("129*129");
                terrainSizeOptions.Options.Add("257*257");
                terrainSizeOptions.Options.Add("513*513");
                terrainSizeOptions.Options.Add("1025*1025");
                terrainSizeOptions.Value = 0;

                FormFieldInteger terrainCellSize = new FormFieldInteger();
                terrainCellSize.Title = "Cell size (m)";
                terrainCellSize.Key = "cellSize";
                terrainCellSize.Value = 1000;

                FormFieldInteger maxHeight = new FormFieldInteger();
                maxHeight.Title = "Max height (m)";
                maxHeight.Key = "maxHeight";
                maxHeight.Value = 2000;

                FormFieldInteger thermalErosionPass = new FormFieldInteger();
                thermalErosionPass.Title = "Thermal erosion pass";
                thermalErosionPass.Key = "thermalErosionPass";
                thermalErosionPass.Value = 20;

                FormFieldInteger hydraulicErosionPass = new FormFieldInteger();
                hydraulicErosionPass.Title = "Hydraulic erosion pass";
                hydraulicErosionPass.Key = "hydraulicErosionPass";
                hydraulicErosionPass.Value = 20;

                FormFieldCheck useRidgeSketch = new FormFieldCheck();
                useRidgeSketch.Title = "Use ridge sketch";
                useRidgeSketch.Key = "useRidgeSketch";
                useRidgeSketch.Value = false;

                FormFieldInteger ridgeParticles = new FormFieldInteger();
                ridgeParticles.Title = "Ridge particles";
                ridgeParticles.Key = "ridgeParticles";
                ridgeParticles.Value = 5;

                FormFieldCheck exportGaussHeightMap = new FormFieldCheck();
                exportGaussHeightMap.Title = "Export gauss map";
                exportGaussHeightMap.Key = "exportGaussMap";
                exportGaussHeightMap.Value = false;

                FormFieldCheck exportMDIMap = new FormFieldCheck();
                exportMDIMap.Title = "Export MDI map";
                exportMDIMap.Key = "exportMDIMap";
                exportMDIMap.Value = false;

                terrainParametersForm.FormFields.Add(terrainSeed);
                terrainParametersForm.FormFields.Add(terrainSizeOptions);
                terrainParametersForm.FormFields.Add(terrainCellSize);
                terrainParametersForm.FormFields.Add(maxHeight);
                terrainParametersForm.FormFields.Add(thermalErosionPass);
                terrainParametersForm.FormFields.Add(hydraulicErosionPass);
                terrainParametersForm.FormFields.Add(useRidgeSketch);
                terrainParametersForm.FormFields.Add(ridgeParticles);
                //terrainParametersForm.FormFields.Add(exportGaussHeightMap);
                //terrainParametersForm.FormFields.Add(exportMDIMap);

                TerrainInputSketch ridgeSketch = new TerrainInputSketch();
                ridgeSketch.Title = "Ridge sketch";
                ridgeSketch.Key = "ridgeSketch";
                ridgeSketch.BaseColor = "#000000ff";
                ridgeSketch.Width = 500;
                ridgeSketch.Height = 500;

                SketchToolBrush sketchToolRidge = new SketchToolBrush();
                sketchToolRidge.Title = "Ridge";
                sketchToolRidge.Color = "#ffffffff";
                sketchToolRidge.VelocityBasedOpacity = true;
                sketchToolRidge.Width = 1;

                SketchToolEraser ridgeEraser = new SketchToolEraser();
                ridgeEraser.Title = "Ridge eraser";
                ridgeEraser.Size = 20;

                ridgeSketch.SketchTools.Add(sketchToolRidge);
                ridgeSketch.SketchTools.Add(ridgeEraser);

                TerrainInputSketch terrainRoughnessSketch = new TerrainInputSketch();
                terrainRoughnessSketch.Title = "Roughness sketch";
                terrainRoughnessSketch.Key = "roughnessSketch";
                terrainRoughnessSketch.BaseColor = "#00ff00ff";
                terrainRoughnessSketch.Width = 500;
                terrainRoughnessSketch.Height = 500;


                SketchToolBrush sketchToolRoughnessLessHalf = new SketchToolBrush();
                sketchToolRoughnessLessHalf.Title = "Roughness -1/2";
                sketchToolRoughnessLessHalf.Color = "#0000ffff";
                sketchToolRoughnessLessHalf.VelocityBasedOpacity = false;
                sketchToolRoughnessLessHalf.Width = 20;

                SketchToolBrush sketchToolRoughnessLessQuarter = new SketchToolBrush();
                sketchToolRoughnessLessQuarter.Title = "Roughness -1/4";
                sketchToolRoughnessLessQuarter.Color = "#00ffffff";
                sketchToolRoughnessLessQuarter.VelocityBasedOpacity = false;
                sketchToolRoughnessLessQuarter.Width = 20;

                SketchToolBrush sketchToolRoughnessMoreQuarter = new SketchToolBrush();
                sketchToolRoughnessMoreQuarter.Title = "Roughness 1/4";
                sketchToolRoughnessMoreQuarter.Color = "#ffff00ff";
                sketchToolRoughnessMoreQuarter.VelocityBasedOpacity = false;
                sketchToolRoughnessMoreQuarter.Width = 20;

                SketchToolBrush sketchToolRoughnessMoreHalf = new SketchToolBrush();
                sketchToolRoughnessMoreHalf.Title = "Roughness 1/2";
                sketchToolRoughnessMoreHalf.Color = "#ff0000ff";
                sketchToolRoughnessMoreHalf.VelocityBasedOpacity = false;
                sketchToolRoughnessMoreHalf.Width = 20;

                SketchToolEraser roughnessEraser = new SketchToolEraser();
                roughnessEraser.Title = "Roughness eraser";
                roughnessEraser.Size = 20;

                terrainRoughnessSketch.SketchTools.Add(sketchToolRoughnessLessHalf);
                terrainRoughnessSketch.SketchTools.Add(sketchToolRoughnessLessQuarter);
                terrainRoughnessSketch.SketchTools.Add(sketchToolRoughnessMoreQuarter);
                terrainRoughnessSketch.SketchTools.Add(sketchToolRoughnessMoreHalf);
                terrainRoughnessSketch.SketchTools.Add(roughnessEraser);

                List<TerrainInput> terrainInputs = new List<TerrainInput>();
                terrainInputs.Add(terrainParametersForm);
                terrainInputs.Add(ridgeSketch);
                terrainInputs.Add(terrainRoughnessSketch);

                return terrainInputs;
            }
        }

        public List<TerrainOutput> StartProcess(List<TerrainInput> terrainInputs, Dictionary<LayerType, WorkflowData> workflowData)
        {
            List<TerrainOutput> terrainOutputs = new List<TerrainOutput>();
            RidgeHeightMap.curSample = 0;
            RidgeHeightMap.targetSamples = 0;

            HeightmapWorkflowData heightmapWorkflowData = new HeightmapWorkflowData();

            RidgeHeightMap.startWork(terrainInputs, terrainOutputs, heightmapWorkflowData);

            workflowData[LayerType.HeightMap] = heightmapWorkflowData;

            return terrainOutputs;

            /*float?[][] heightMap = GenerationInterface.startWork();

            List<TerrainOutput> terrainOutputs = new List<TerrainOutput>();


            TerrainOutputMesh terrainOutputMesh = new TerrainOutputMesh();
            terrainOutputMesh.Title = "Height map mesh";

            for (int x = 0; x < heightMap.Length; x++)
            {
                for(int y = 0; y < heightMap[x].Length; y++)
                {
                    terrainOutputMesh.VertexData.Add(new float[] { x, (float)(heightMap[x][y] * 2f), y });
                }
            }

            for (int x = 0; x < heightMap.Length-1; x++)
            {
                for (int y = 1; y < heightMap[x].Length; y++)
                {
                    terrainOutputMesh.FacesData.Add(new int[] {y+x*heightMap[x].Length+1,
                    y+x*heightMap[x].Length,
                    y+(x+1)*heightMap[x].Length});

                    terrainOutputMesh.FacesData.Add(new int[] {y+x*heightMap[x].Length+1,
                    y+(x+1)*heightMap[x].Length,
                    y+(x+1)*heightMap[x].Length+1});
                }
            }

            terrainOutputs.Add(terrainOutputMesh);
            return terrainOutputs;*/

            /*TerrainOutputMesh terrainOutputMesh = new TerrainOutputMesh();
            terrainOutputMesh.Title = "Height map mesh";
            terrainOutputMesh.Key = "hMap";


            float?[][] heightMap = GenerationInterface.startWork();*/
            /*float[][] heightMap = new float[64][];
            Console.WriteLine("A");
            for(int x = 0; x < 64; x++)
            {
                heightMap[x] = new float[64];
                for (int y = 0; y < 64; y++)
                {
                    heightMap[x][y] = (float)x/(float)2 +  (float)y/(float)2;
                }
            }*/
            /*Console.WriteLine("B");

            for (int x = 0; x < heightMap.Length; x++)
            {
                for (int y = 0; y < heightMap[x].Length; y++)
                {
                    terrainOutputMesh.VertexData.Add(new float[] { x, (float)(heightMap[x][y] * 4f), y });
                }
            }

            Console.WriteLine("C");

            for (int x = 0; x < heightMap.Length - 1; x++)
            {
                for (int y = 1; y < heightMap[x].Length; y++)
                {
                    terrainOutputMesh.FacesData.Add(new int[] {y+x*heightMap[x].Length+1,
                    y+(x+1)*heightMap[x].Length,
                    y +x*heightMap[x].Length });

                    terrainOutputMesh.FacesData.Add(new int[] {y+x*heightMap[x].Length+1,
                    y+(x+1)*heightMap[x].Length+1,
                    y+(x+1)*heightMap[x].Length});
                }
            }

            Console.WriteLine("D");




            TerrainOutputImage terrainOutputImage1 = new TerrainOutputImage();
            terrainOutputImage1.Title = "Output Image 1";
            terrainOutputImage1.Key = "oImage1";

            terrainOutputImage1.ImageData = new int[300][];
            for (int x = 0; x < 300; x++)
            {
                terrainOutputImage1.ImageData[x] = new int[300];
                for (int y = 0; y < 300; y++)
                {
                    terrainOutputImage1.ImageData[x][y] = int.Parse("FF0000FF", System.Globalization.NumberStyles.HexNumber);
                }
            }

            Console.WriteLine("E");

            TerrainOutputImage terrainOutputImage2 = new TerrainOutputImage();
            terrainOutputImage2.Title = "Output Image 2";
            terrainOutputImage2.Key = "oImage2";

            terrainOutputImage2.ImageData = new int[300][];
            for (int x = 0; x < 300; x++)
            {
                terrainOutputImage2.ImageData[x] = new int[300];
                for (int y = 0; y < 300; y++)
                {
                    terrainOutputImage2.ImageData[x][y] = int.Parse("00FF00FF", System.Globalization.NumberStyles.HexNumber);
                }
            }

            Console.WriteLine("G");

            TerrainOutputGroup terrainOutputGroup = new TerrainOutputGroup();
            terrainOutputGroup.Title = "Image group";
            terrainOutputGroup.Key = "iGroup";
            terrainOutputGroup.ChildOutputs.Add(terrainOutputImage1);
            terrainOutputGroup.ChildOutputs.Add(terrainOutputImage2);


            TerrainOutputValues terrainOutputValues = new TerrainOutputValues();
            terrainOutputValues.Title = "Values";
            terrainOutputValues.Key = "values";
            terrainOutputValues.AddValue("Value 1", "Something1", "val1");
            terrainOutputValues.AddValue("Value 2", "Something2", "val2");
            terrainOutputValues.AddValue("Value 3", "Something3", "val3");
            terrainOutputValues.AddValue("Value 4", "Something4", "val4");
            terrainOutputValues.AddValue("Value 5", "Something5", "val5");
            terrainOutputValues.AddValue("Value 6", "Something6", "val6");

            Console.WriteLine("G");

            List<TerrainOutput> terrainOutputs = new List<TerrainOutput>();
            terrainOutputs.Add(terrainOutputMesh);
            terrainOutputs.Add(terrainOutputGroup);
            terrainOutputs.Add(terrainOutputValues);

            Console.WriteLine("H");

            return terrainOutputs;*/
        }
    }
}
