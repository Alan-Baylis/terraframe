using System;
using System.Collections.Generic;
using TerrainGeneration_PluginBase;
using TerrainGeneration_PluginBase.Helpers;
using TerrainGeneration_PluginBase.WorkflowData;

namespace TerrainGeneration_Vegetation
{
    class Plugin : TerrainPlugin
    {
        public string Name
        {
            get
            {
                return "Trees with space colonization";
            }
        }

        public string Description
        {
            get
            {
                return "Generates trees with an algorithm based in particle systems (Runions et al. 2007)";
            }
        }

        public string Author
        {
            get
            {
                return "Hugo P.";
            }
        }

        public int Progress
        {
            get
            {
                return 0;
            }
        }

        public HashSet<LayerType> Out
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                terrainTypes.Add(LayerType.Vegetation);
                return terrainTypes;
            }
        }

        public HashSet<LayerType> In
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                //terrainTypes.Add(TerrainPipelineLayerType.HeightMap);
                return terrainTypes;
            }
        }

        public HashSet<LayerType> Not
        {
            get
            {
                HashSet<LayerType> terrainTypes = new HashSet<LayerType>();
                terrainTypes.Add(LayerType.Roads);
                terrainTypes.Add(LayerType.Urban);
                terrainTypes.Add(LayerType.WaterBodies);
                return terrainTypes;
            }
        }

        public List<TerrainInput> TerrainInputs
        {
            get
            {
                List<TerrainInput> inputs = new List<TerrainInput>();

                TerrainInputForm terrainInputForm = new TerrainInputForm();
                terrainInputForm.Key = "treeParameters";
                terrainInputForm.Title = "Tree Parameters";

                FormFieldInteger formFieldSpace = new FormFieldInteger();
                formFieldSpace.Key = "formFieldStartSpace";
                formFieldSpace.Title = "Space";
                formFieldSpace.Value = 500;

                FormFieldInteger formFieldMinLeafDistance = new FormFieldInteger();
                formFieldMinLeafDistance.Key = "minLeafDistance";
                formFieldMinLeafDistance.Title = "Min Leaf Disance";
                formFieldMinLeafDistance.Value = 20;

                FormFieldInteger formFieldMaxLeafDistance = new FormFieldInteger();
                formFieldMaxLeafDistance.Key = "maxLeafDistance";
                formFieldMaxLeafDistance.Title = "Max Leaf Disance";
                formFieldMaxLeafDistance.Value = 100;

                FormFieldInteger formFieldPointCount = new FormFieldInteger();
                formFieldPointCount.Key = "pointCount";
                formFieldPointCount.Title = "Point Count";
                formFieldPointCount.Value = 300;

                FormFieldInteger formFieldBranchLength = new FormFieldInteger();
                formFieldBranchLength.Key = "branchLength";
                formFieldBranchLength.Title = "Branch Length";
                formFieldBranchLength.Value = 15;

                FormFieldInteger formFieldLeafCount = new FormFieldInteger();
                formFieldLeafCount.Key = "leafCount";
                formFieldLeafCount.Title = "Leaf Count";
                formFieldLeafCount.Value = 300;

                FormFieldInteger formFieldLeafSize = new FormFieldInteger();
                formFieldLeafSize.Key = "leafSize";
                formFieldLeafSize.Title = "Leaf Size";
                formFieldLeafSize.Value = 5;

                FormFieldInteger formFieldLeafOffset = new FormFieldInteger();
                formFieldLeafOffset.Key = "leafOffset";
                formFieldLeafOffset.Title = "Leaf Branch Offset";
                formFieldLeafOffset.Value = 10;

                FormFieldInteger formFieldNumOfTrees = new FormFieldInteger();
                formFieldNumOfTrees.Key = "numOfTrees";
                formFieldNumOfTrees.Title = "Tree count";
                formFieldNumOfTrees.Value = 100;

                TerrainInputSketch terrainInputSketch = new TerrainInputSketch();
                terrainInputSketch.Title = "Tree Top Sketch";
                terrainInputSketch.Key = "treeTopSketch";
                terrainInputSketch.Width = 500;
                terrainInputSketch.Height = 500;
                terrainInputSketch.BaseColor = "#e9e9e9ff";

                SketchToolBrush lineSketchTool = new SketchToolBrush();
                lineSketchTool.Title = "Tree Top";
                lineSketchTool.Color = "#00ff33ff";
                lineSketchTool.Width = 30;

                SketchToolEraser eraserSketchTool = new SketchToolEraser();
                eraserSketchTool.Title = "Clear";
                eraserSketchTool.Size = 30;

                terrainInputSketch.SketchTools.Add(lineSketchTool);
                terrainInputSketch.SketchTools.Add(eraserSketchTool);

                terrainInputForm.FormFields.Add(formFieldSpace);
                terrainInputForm.FormFields.Add(formFieldMinLeafDistance);
                terrainInputForm.FormFields.Add(formFieldMaxLeafDistance);
                terrainInputForm.FormFields.Add(formFieldBranchLength);
                terrainInputForm.FormFields.Add(formFieldPointCount);
                terrainInputForm.FormFields.Add(formFieldLeafCount);
                terrainInputForm.FormFields.Add(formFieldLeafSize);
                terrainInputForm.FormFields.Add(formFieldLeafOffset);
                terrainInputForm.FormFields.Add(formFieldNumOfTrees);

                inputs.Add(terrainInputForm);
                inputs.Add(terrainInputSketch);

                return inputs;
            }
        }

        public List<TerrainOutput> StartProcess(List<TerrainInput> terrainInputs, Dictionary<LayerType, WorkflowData> workflowData)
        {
            System.Console.WriteLine("Will begin vegetation");
            List<TerrainOutput> terrainOutputs = new List<TerrainOutput>();
            /*System.Console.WriteLine("A");
            Dictionary<string, TerrainInput> inputMap = new Dictionary<string, TerrainInput>();
            Dictionary<string, FormField> formFieldsMap = new Dictionary<string, FormField>();
            createInputMap(terrainInputs, inputMap, formFieldsMap);
            System.Console.WriteLine("B");
            string rules = ((FormFieldText)formFieldsMap["lSystemInput_formFieldRules"]).Value;
            System.Console.WriteLine("C");
            Dictionary<char, string> originDestinationMap = new Dictionary<char, string>();
            System.Console.WriteLine("D");
            foreach (string rule in rules.Split(','))
            {
                originDestinationMap.Add(rule.Split('>')[0][0], rule.Split('>')[1]);
            }
            System.Console.WriteLine("E");
            string production = Vegetation.createProducton("", "", ((FormFieldText)formFieldsMap["lSystemInput_formFieldAxiom"]).Value, originDestinationMap, ((FormFieldInteger)formFieldsMap["lSystemInput_formFieldIterations"]).Value);
            System.Console.WriteLine("F");
            terrainOutputs.Add(Turtle.drawProduction(production,
                ((FormFieldInteger)formFieldsMap["lSystemInput_formFieldStartWidth"]).Value,
                ((FormFieldInteger)formFieldsMap["lSystemInput_formFieldStartHeight"]).Value,
                ((FormFieldInteger)formFieldsMap["lSystemInput_formFieldStartX"]).Value,
                ((FormFieldInteger)formFieldsMap["lSystemInput_formFieldStartY"]).Value,
                (float)((FormFieldNumber)formFieldsMap["lSystemInput_formFieldStrokeX"]).Value,
                (float)((FormFieldNumber)formFieldsMap["lSystemInput_formFieldStrokeY"]).Value,
                ((FormFieldInteger)formFieldsMap["lSystemInput_formFieldRotation"]).Value));

            System.Console.WriteLine(production);

            TerrainOutputValues terrainOutputValues = new TerrainOutputValues();
            terrainOutputValues.Key = "terrainOutputValues";
            terrainOutputValues.Title = "Result values";

            terrainOutputValues.ValueKeys.Add("production");
            terrainOutputValues.ValueTitles.Add("Production");
            terrainOutputValues.Values.Add(production);

            System.Console.WriteLine("Will end vegetation");

            terrainOutputs.Add(terrainOutputValues);*/
            try
            {
                Dictionary<string, TerrainInput> inputMap = new Dictionary<string, TerrainInput>();
                Dictionary<string, FormField> formFieldsMap = new Dictionary<string, FormField>();
                createInputMap(terrainInputs, inputMap, formFieldsMap);

                terrainOutputs.AddRange(SpaceColonization.createTree(
                    ((FormFieldInteger)formFieldsMap["treeParameters_formFieldStartSpace"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_formFieldStartSpace"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_formFieldStartSpace"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_minLeafDistance"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_maxLeafDistance"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_pointCount"]).Value,
                    ((TerrainInputSketch)inputMap["treeTopSketch"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_leafCount"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_leafSize"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_leafOffset"]).Value,
                    ((FormFieldInteger)formFieldsMap["treeParameters_branchLength"]).Value));

                if (workflowData.ContainsKey(LayerType.HeightMap))
                {
                    HeightmapWorkflowData heightmapWorkflowData = (HeightmapWorkflowData)workflowData[LayerType.HeightMap];
                    TerrainOutputMesh terrainOutputMesh = new TerrainOutputMesh();
                    terrainOutputMesh.Title = "Terrain";
                    terrainOutputMesh.Key = "terrainHeightMap";

                    List<float[]> grassLand = new List<float[]>();

                    MeshGeneratorHelpers.FillHeightMapMesh(terrainOutputMesh, heightmapWorkflowData.HeightmapCells, heightmapWorkflowData.CellSize, heightmapWorkflowData.MinHeight, heightmapWorkflowData.MaxHeight, grassLand);


                    

                    int numOfRepeats = ((FormFieldInteger)formFieldsMap["treeParameters_numOfTrees"]).Value;

                    TerrainOutputMesh treeMesh = (TerrainOutputMesh)terrainOutputs[0];
                    treeMesh.Title = "Tree";
                    treeMesh.Key = "Tree";

                    terrainOutputMesh.SubMeshes.Add((TerrainOutputMesh)terrainOutputs[0]);
                    terrainOutputMesh.Positions.Add(new List<float[]>());
                    terrainOutputMesh.Scales.Add(new List<float[]>());
                    terrainOutputMesh.Rotations.Add(new List<float[]>());




                    for (int i = 0; i < numOfRepeats; i++)
                    {
                        if (grassLand.Count > 0)
                        {
                            int idx = SpaceColonization.random.Next(grassLand.Count);
                            


                            terrainOutputMesh.Positions[0].Add(new float[] { grassLand[idx][0]-1f, grassLand[idx][2], grassLand[idx][1]-1f });
                            terrainOutputMesh.Scales[0].Add(new float[] { 0.005f, 0.005f, 0.005f });
                            terrainOutputMesh.Rotations[0].Add(new float[] { 0f, 0f, 0f });

                            grassLand.RemoveAt(idx);
                        }
                    }

                    terrainOutputs.Insert(0,terrainOutputMesh);
                }
                







            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw ex;
            }


            return terrainOutputs;
        }

        public static void createInputMap(List<TerrainInput> terrainInputs, Dictionary<string, TerrainInput> inputMap, Dictionary<string, FormField> formFieldsMap)
        {
            foreach (TerrainInput terrainInput in terrainInputs)
            {
                switch (terrainInput.Type)
                {
                    case InputType.Group:
                        createInputMap(((TerrainInputGroup)terrainInput).ChildInputs, inputMap, formFieldsMap);
                        break;
                    case InputType.Form:
                        inputMap.Add(terrainInput.Key, terrainInput);
                        foreach (FormField formField in ((TerrainInputForm)terrainInput).FormFields)
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
    }
}
