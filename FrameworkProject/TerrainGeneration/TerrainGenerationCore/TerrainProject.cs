using System;
using System.Collections.Generic;
using System.IO;
using TerrainGeneration_PluginBase;
using TerrainGeneration_PluginBase.WorkflowData;

namespace TerrainGeneration_Core
{
    public class TerrainProject
    {
        private const string PROJECT_INFO_FILE = "ProjectInfo.tgi";
        private string INPUT_SUBDIRECTORY = System.IO.Path.DirectorySeparatorChar + "Input" + System.IO.Path.DirectorySeparatorChar;
        private string OUTPUT_SUBDIRECTORY = System.IO.Path.DirectorySeparatorChar + "Output" + System.IO.Path.DirectorySeparatorChar;

        public TerrainProjectInfo Info { get; }
        private Configurations configurations;
        public Configurations Configurations{ get { return configurations; }}
        internal List<TerrainPlugin> TerrainPipeline { get; set; }
        private Dictionary<LayerType, WorkflowData> workflowData;
        public Dictionary<LayerType, WorkflowData> WorkflowData
        {
            get
            {
                if (workflowData == null)
                    loadWorkflowData();
                return workflowData;
            }
        }
        internal Dictionary<TerrainPipelineLayer, TerrainPlugin> PipelinePluginMap
        {
            get
            {
                Dictionary<TerrainPipelineLayer, TerrainPlugin> pipelinePluginMap = new Dictionary<TerrainPipelineLayer, TerrainPlugin>(new TerrainPipelineLayerComparator());
                foreach (TerrainPlugin terrainPlugin in TerrainPipeline)
                {
                    if (terrainPlugin != null)
                    {
                        TerrainPipelineLayer pipelineLayer = new TerrainPipelineLayer();
                        TerrainPluginInfo pluginInfo = new TerrainPluginInfo(terrainPlugin);
                        pipelineLayer.PluginId = pluginInfo.Id;
                        pipelineLayer.LayerCode = TerrainPipelineLayer.ToPipelineLayerCode(terrainPlugin.Out, terrainPlugin.In, terrainPlugin.Not);

                        pipelinePluginMap.Add(pipelineLayer, terrainPlugin);
                    }
                }

                return pipelinePluginMap;
            }
        }
        internal string Path { get; }
        private Dictionary<TerrainPipelineLayer, List<TerrainOutput>> terrainOutputs;
        public Dictionary<TerrainPipelineLayer, List<TerrainOutput>> TerrainOutputs
        {
            get
            {
                if(terrainOutputs == null)
                    loadTerrainOutputs();
                return terrainOutputs;
            }
        }

        private Dictionary<TerrainPipelineLayer, List<TerrainInput>> terrainInputs;
        public Dictionary<TerrainPipelineLayer, List<TerrainInput>> TerrainInputs
        {
            get
            {
                if (terrainInputs == null)
                {
                    terrainInputs = new Dictionary<TerrainPipelineLayer, List<TerrainInput>>(new TerrainPipelineLayerComparator());
                    foreach (TerrainPlugin terrainPlugin in TerrainPipeline)
                    {
                        if (terrainPlugin != null)
                        {
                            Console.WriteLine("Inputs: " + terrainPlugin.Name + ", " + terrainPlugin.TerrainInputs.Count);

                            List<TerrainInput> terrainInputs_ = terrainPlugin.TerrainInputs;

                            TerrainPipelineLayer pipelineLayer = new TerrainPipelineLayer();
                            TerrainPluginInfo pluginInfo = new TerrainPluginInfo(terrainPlugin);
                            pipelineLayer.PluginId = pluginInfo.Id;
                            pipelineLayer.LayerCode = TerrainPipelineLayer.ToPipelineLayerCode(terrainPlugin.Out, terrainPlugin.In, terrainPlugin.Not);

                            terrainInputs_ = loadTerrainInputs(pipelineLayer, terrainInputs_);
                            terrainInputs.Add(pipelineLayer, terrainInputs_);
                        }
                    }

                    cleanUnusedInputs();
                }

                return terrainInputs;
            }
        }

        internal static string PROJECT_BASE_PATH
        {
            get
            {
                string terraFrameFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + System.IO.Path.DirectorySeparatorChar + "TerraFrame";
                Console.WriteLine(terraFrameFolder);

                if (!Directory.Exists(terraFrameFolder))
                    Directory.CreateDirectory(terraFrameFolder);

                string projectFolder = terraFrameFolder + System.IO.Path.DirectorySeparatorChar + "Projects";

                if (!Directory.Exists(projectFolder))
                    Directory.CreateDirectory(projectFolder);

                return projectFolder + System.IO.Path.DirectorySeparatorChar;
            }
        }

        public static Dictionary<TerrainPipelineLayer, List<TerrainPluginInfo>> AvailableTerrainPlugins
        {
            get
            {
                Dictionary<TerrainPipelineLayer, List<TerrainPluginInfo>> pluginInfos = new Dictionary<TerrainPipelineLayer, List<TerrainPluginInfo>>(new TerrainPipelineLayerComparator());
                Dictionary<TerrainPipelineLayer, List<TerrainPlugin>> availablePlugins = PluginManager.AvailablePlugins;

                foreach (TerrainPipelineLayer terrainPipelineLayer in availablePlugins.Keys)
                {
                    pluginInfos.Add(terrainPipelineLayer, new List<TerrainPluginInfo>());
                    foreach(TerrainPlugin terrainPlugin in availablePlugins[terrainPipelineLayer])
                    {
                        pluginInfos[terrainPipelineLayer].Add(new TerrainPluginInfo(terrainPlugin));
                    }
                }

                return pluginInfos;
            }
        }

        public TerrainProject(string name, string description)
        {
            Info = new TerrainProjectInfo();
            Info.Name = name;
            Info.CreationDate = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Info.ConfiguredLayers = new List<LayerType>();
            Info.ProcessedLayers = new List<LayerType>();

            Path = getNewPath();
            saveInfo();
        }

        public TerrainProject(string existingProjectPath)
        {
            Path = existingProjectPath;

            string infoFileContent = FileInterface.ReadString(Path + System.IO.Path.DirectorySeparatorChar + PROJECT_INFO_FILE);
            object obj = ObjectSerializer.DeSerialize(infoFileContent, typeof(TerrainProjectInfo));
            Info = (TerrainProjectInfo)obj;
        }

        public void Load()
        {
            configurations = Configurations.LoadConfigurations(this);
            TerrainPipeline = PluginManager.GetTerrainPipeline(this);

            for(int i = 0; i < TerrainPipeline.Count; i++)
            {
                if (TerrainPipeline[i] == null)
                    configurations.Layers[i].PluginId = 0;
            }

            Console.WriteLine("Pipeline created: " + TerrainPipeline.Count);
            terrainInputs = null;
        }

        public void Save()
        {
            Info.ConfiguredLayers.Clear();
            Info.ProcessedLayers.Clear();
            if (terrainInputs != null)
            {
                foreach (TerrainPipelineLayer terrainPipelineLayer in terrainInputs.Keys)
                {
                    Info.ConfiguredLayers.AddRange(terrainPipelineLayer.LayerTypes);
                }
            }

            if (TerrainOutputs != null)
            {
                foreach (TerrainPipelineLayer terrainPipelineLayer in TerrainOutputs.Keys)
                {
                    if(TerrainOutputs[terrainPipelineLayer].Count > 0)
                        Info.ProcessedLayers.AddRange(terrainPipelineLayer.LayerTypes);
                }
            }

            saveInfo();
            configurations.Commit();

            HashSet<string> inputFilesCreated = new HashSet<string>();
            if(terrainInputs != null)
            {
                foreach(TerrainPipelineLayer terrainPipelineLayer in terrainInputs.Keys)
                {
                    saveTerrainInputs(terrainPipelineLayer, terrainInputs[terrainPipelineLayer], inputFilesCreated);
                }
            }

            if (FileInterface.DoesFolderExists(Path + INPUT_SUBDIRECTORY))
            {
                string[] files = FileInterface.GetChildFiles(Path + INPUT_SUBDIRECTORY);

                foreach (string file in files)
                {
                    if (file.EndsWith(".tin"))
                    {
                        if (!inputFilesCreated.Contains(file))
                        {
                            FileInterface.DeleteFile(file);
                        }
                    }
                }
            }

            if (terrainOutputs != null)
                saveTerrainOutputs();

            if (workflowData != null)
                saveWorkflowData();
        }

        private void saveInfo()
        {
            string infoFileContent = ObjectSerializer.Serialize(Info);
            FileInterface.WriteString(Path + System.IO.Path.DirectorySeparatorChar + PROJECT_INFO_FILE, infoFileContent);
        }

        private string getNewPath()
        {
            string basePath = PROJECT_BASE_PATH + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string path = basePath;
            int i = 1;
            while (FileInterface.DoesFolderExists(path))
            {
                path = basePath + "_" + i;
                i++;
            }

            FileInterface.CreateFolder(path);

            return path;
        }

        public static List<TerrainProject> TerrainProjects
        {
            get
            {
                string[] childFolders = FileInterface.GetChildFolders(PROJECT_BASE_PATH);

                List<TerrainProject> terrainProjects = new List<TerrainProject>();
                foreach (string childFolder in childFolders)
                {
                    terrainProjects.Add(new TerrainProject(childFolder));
                }

                return terrainProjects;
            }
        }

        private List<TerrainInput> loadTerrainInputs(TerrainPipelineLayer terrainPipelineLayer, List<TerrainInput> terrainInputs)
        {
            string inputData = FileInterface.ReadString(Path + INPUT_SUBDIRECTORY + terrainPipelineLayer.LayerCode + "_" + terrainPipelineLayer.PluginId + ".tin");
            if (inputData != null)
            {
                try
                {
                    List<TerrainInput> allTerrainInputs = new List<TerrainInput>();
                    mergeInnerTerrainInputs(allTerrainInputs, terrainInputs);
                    List<TerrainInputValue> terrainInputValues = (List<TerrainInputValue>)ObjectSerializer.DeSerialize(inputData, typeof(List<TerrainInputValue>));

                    int i = 0;
                    foreach (TerrainInput terrainInput in allTerrainInputs)
                    {
                        switch (terrainInput.Type)
                        {
                            case InputType.Form:
                            case InputType.Sketch:
                                terrainInputValues[i].FillValue(terrainInput);
                                i++;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Saved input data does not match: " + ex.Message);
                }
            }

            return terrainInputs;
        }

        private void saveTerrainInputs(TerrainPipelineLayer terrainPipelineLayer, List<TerrainInput> terrainInputs, HashSet<string> inputFilesCreated)
        {
            if (terrainPipelineLayer == null || terrainInputs == null)
                return;

            List<TerrainInput> allTerrainInputs = new List<TerrainInput>();
            mergeInnerTerrainInputs(allTerrainInputs, terrainInputs);
            terrainInputs = allTerrainInputs;

            List<TerrainInputValue> terrainInputValues = new List<TerrainInputValue>();

            foreach(TerrainInput terrainInput in terrainInputs)
            {
                TerrainInputValue terrainInputValue = null;

                switch (terrainInput.Type)
                {
                    case InputType.Form:
                        terrainInputValue = new TerrainInputValueForm();
                        break;
                    case InputType.Sketch:
                        terrainInputValue = new TerrainInputValueSketch();
                        break;
                }

                if (terrainInputValue != null)
                {
                    terrainInputValue.SetValue(terrainInput);
                    terrainInputValues.Add(terrainInputValue);
                }
            }

            if(terrainInputValues.Count > 0)
            {
                if (!FileInterface.DoesFolderExists(Path + INPUT_SUBDIRECTORY))
                    FileInterface.CreateFolder(Path + INPUT_SUBDIRECTORY);
                string inputData = ObjectSerializer.Serialize(terrainInputValues);
                FileInterface.WriteString(Path + INPUT_SUBDIRECTORY + terrainPipelineLayer.LayerCode + "_" + terrainPipelineLayer.PluginId + ".tin", inputData);
                inputFilesCreated.Add(Path + INPUT_SUBDIRECTORY + terrainPipelineLayer.LayerCode + "_" + terrainPipelineLayer.PluginId + ".tin");
            }
        }

        private void mergeInnerTerrainInputs(List<TerrainInput> allTerrainInputs, List<TerrainInput> terrainInputs)
        {
            foreach(TerrainInput terrainInput in terrainInputs)
            {
                switch (terrainInput.Type)
                {
                    case InputType.Group:
                        mergeInnerTerrainInputs(allTerrainInputs, ((TerrainInputGroup)terrainInput).ChildInputs);
                        break;
                    default:
                        allTerrainInputs.Add(terrainInput);
                        break;
                }
            }
        }

        public void StartTerrainGenerationTask(TerrainGenerationTask terrainGenerationTask)
        {
            terrainGenerationTask.StartTask(this);
        }

        private void saveWorkflowData()
        {
            if (!FileInterface.DoesFolderExists(Path + OUTPUT_SUBDIRECTORY))
                FileInterface.CreateFolder(Path + OUTPUT_SUBDIRECTORY);

            foreach (LayerType layerType in workflowData.Keys)
            {
                List<WorkflowData> workflowData_ = new List<WorkflowData>();
                workflowData_.Add(workflowData[layerType]);
                string outputData = ObjectSerializer.Serialize(workflowData_);
                FileInterface.WriteString(Path + System.IO.Path.DirectorySeparatorChar + "workflow_" + layerType + ".twd", outputData);
            }
        }

        private void saveTerrainOutputs()
        {
            HashSet<string> createdFiles = new HashSet<string>();
            Console.WriteLine("Will save terrain output");
            if (!FileInterface.DoesFolderExists(Path + OUTPUT_SUBDIRECTORY))
                FileInterface.CreateFolder(Path + OUTPUT_SUBDIRECTORY);

            foreach (TerrainPipelineLayer terrainPipelineLayer in TerrainOutputs.Keys)
            {
                List<TerrainOutput> allTerrainOutputs = new List<TerrainOutput>();
                mergeInnerTerrainOutputs(allTerrainOutputs, TerrainOutputs[terrainPipelineLayer]);

                int i = 0;
                foreach(TerrainOutput terrainOutput in allTerrainOutputs)
                {
                    switch (terrainOutput.Type)
                    {
                        case OutputType.Mesh:
                            Console.WriteLine("Will save terrain output");
                            if (((TerrainOutputMesh)terrainOutput).MeshPath == null || ((TerrainOutputMesh)terrainOutput).MeshPath.Count == 0)
                            {
                                FileInterface.SaveMeshFile(Path + OUTPUT_SUBDIRECTORY + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + "_" + i + ".obj", (TerrainOutputMesh)terrainOutput);
                            }
                            break;
                        case OutputType.Image:
                            if(((TerrainOutputImage)terrainOutput).ImagePath == null || ((TerrainOutputImage)terrainOutput).ImagePath.Length == 0)
                            {
                                FileInterface.SaveImageFile(Path + OUTPUT_SUBDIRECTORY + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + "_" + i + ".bmp", (TerrainOutputImage)terrainOutput);
                            }
                            break;
                    }

                    i++;
                }

                if (TerrainOutputs[terrainPipelineLayer].Count > 0)
                {
                    Console.WriteLine("Saving terrain output");
                    string outputData = ObjectSerializer.Serialize(TerrainOutputs[terrainPipelineLayer]);
                    FileInterface.WriteString(Path + OUTPUT_SUBDIRECTORY + terrainPipelineLayer.LayerCode + "_" + terrainPipelineLayer.PluginId + ".tou", outputData);
                    createdFiles.Add(Path + OUTPUT_SUBDIRECTORY + terrainPipelineLayer.LayerCode + "_" + terrainPipelineLayer.PluginId + ".tou");
                }
            }

            if (FileInterface.DoesFolderExists(Path + OUTPUT_SUBDIRECTORY))
            {
                string[] files = FileInterface.GetChildFiles(Path + OUTPUT_SUBDIRECTORY);

                foreach (string file in files)
                {
                    if (file.EndsWith(".tou"))
                    {
                        if (!createdFiles.Contains(file))
                        {
                            FileInterface.DeleteFile(file);
                        }
                    }
                }
            }
        }

        private void loadWorkflowData()
        {
            workflowData = new Dictionary<LayerType, WorkflowData>();

            foreach(LayerType layerType in (LayerType[]) Enum.GetValues(typeof(LayerType)))
            {
                string filename = Path + System.IO.Path.DirectorySeparatorChar + "workflow_" + layerType + ".twd";
                string inputData = FileInterface.ReadString(filename);
                if (inputData != null)
                {
                    List<WorkflowData> workflowData_ = (List<WorkflowData>)ObjectSerializer.DeSerialize(inputData, typeof(List<WorkflowData>));
                    workflowData.Add(layerType, workflowData_[0]);
                }
            }
        }

        private void loadTerrainOutputs()
        {
            terrainOutputs = new Dictionary<TerrainPipelineLayer, List<TerrainOutput>>(new TerrainPipelineLayerComparator());
            foreach (TerrainPlugin terrainPlugin in TerrainPipeline)
            {
                if (terrainPlugin != null)
                {
                    TerrainPipelineLayer pipelineLayer = new TerrainPipelineLayer();
                    TerrainPluginInfo pluginInfo = new TerrainPluginInfo(terrainPlugin);
                    pipelineLayer.PluginId = pluginInfo.Id;
                    pipelineLayer.LayerCode = TerrainPipelineLayer.ToPipelineLayerCode(terrainPlugin.Out, terrainPlugin.In, terrainPlugin.Not);

                    string filename = Path + OUTPUT_SUBDIRECTORY + pipelineLayer.LayerCode + "_" + pipelineLayer.PluginId + ".tou";

                    string inputData = FileInterface.ReadString(filename);
                    if (inputData != null)
                    {
                        List<TerrainOutput> terrainOutputsList = (List<TerrainOutput>)ObjectSerializer.DeSerialize(inputData, typeof(List<TerrainOutput>));
                        if(terrainOutputsList != null)
                            terrainOutputs.Add(pipelineLayer, terrainOutputsList);
                    }
                }
            }

            cleanUnusedOutputs();
        }

        private void mergeInnerTerrainOutputs(List<TerrainOutput> allTerrainOutputs, List<TerrainOutput> terrainOutputs)
        {
            foreach (TerrainOutput terrainOutput in terrainOutputs)
            {
                switch (terrainOutput.Type)
                {
                    case OutputType.Group:
                        mergeInnerTerrainOutputs(allTerrainOutputs, ((TerrainOutputGroup)terrainOutput).ChildOutputs);
                        break;
                    default:
                        allTerrainOutputs.Add(terrainOutput);
                        break;
                }
            }
        }

        private void cleanUnusedOutputs()
        {
            HashSet<string> activePaths = new HashSet<string>();

            foreach(List<TerrainOutput> terrainOutputList_ in terrainOutputs.Values)
            {
                List<TerrainOutput> terrainOutputList = new List<TerrainOutput>();
                mergeInnerTerrainOutputs(terrainOutputList, terrainOutputList_);
                foreach (TerrainOutput terrainOutput in terrainOutputList)
                {
                    switch (terrainOutput.Type)
                    {
                        case OutputType.Image:
                            activePaths.Add(((TerrainOutputImage)terrainOutput).ImagePath);
                            break;
                        case OutputType.Mesh:
                            foreach(string path in ((TerrainOutputMesh)terrainOutput).MeshPath)
                                activePaths.Add(path);
                            break;
                    }
                }
            }

            if (FileInterface.DoesFolderExists(Path + OUTPUT_SUBDIRECTORY))
            {
                string[] files = FileInterface.GetChildFiles(Path + OUTPUT_SUBDIRECTORY);

                foreach (string file in files)
                {
                    if (!file.EndsWith(".tou"))
                    {
                        if (!activePaths.Contains(file))
                        {
                            FileInterface.DeleteFile(file);
                        }
                    }
                }
            }
        }

        private void cleanUnusedInputs()
        {

        }

        public static void DeleteProject(TerrainProject terrainProject) {
            FileInterface.DeleteFolder(terrainProject.Path);
        }

        public int[][] ThumbnailData
        {
            set
            {
                Helpers.Instance.SaveImageFile(Path + System.IO.Path.DirectorySeparatorChar + "thumbnail.bmp", value);
                Info.ThumbnailPath = Path + System.IO.Path.DirectorySeparatorChar + "thumbnail.bmp";
                saveInfo();
            }
        }
    }
}
