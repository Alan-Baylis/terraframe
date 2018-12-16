using System;
using System.Collections.Generic;
using System.IO;
using TerrainGeneration_PluginBase;

namespace TerrainGeneration_Core
{
    public class Configurations
    {
        private static string CONFIGURATIONS_DEFAULT_PATH = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Data";
        private static string CONFIGURATIONS_FILE = "Configurations.tgc";
        private static string currentPath;


        private bool protectData = false;

        private List<TerrainPipelineLayer> layers = new List<TerrainPipelineLayer>();
        public List<TerrainPipelineLayer> Layers
        {
            get
            {
                if (protectData)
                {
                    List<TerrainPipelineLayer> layersCopy = new List<TerrainPipelineLayer>();
                    foreach (TerrainPipelineLayer terrainPipelineLayer in layers)
                    {
                        layersCopy.Add(terrainPipelineLayer.Clone());
                    }
                    return layersCopy;
                }
                else
                {
                    return layers;
                }
            }

            set
            {
                if (protectData)
                {
                    List<TerrainPipelineLayer> layersCopy = new List<TerrainPipelineLayer>();
                    HashSet<LayerType> configuredLayerTypes = new HashSet<LayerType>();

                    foreach (TerrainPipelineLayer layer in value)
                    {
                        bool ignoreLayer = false;
                        foreach (LayerType layerType in layer.LayerTypes)
                        {
                            if (configuredLayerTypes.Contains(layerType))
                            {
                                ignoreLayer = true;
                            }
                        }

                        if (!ignoreLayer)
                        {
                            layersCopy.Add(layer.Clone());
                            foreach (LayerType layerType in layer.LayerTypes)
                                configuredLayerTypes.Add(layerType);
                        }


                    }

                    foreach (LayerType terrainPipelineLayerType in Enum.GetValues(typeof(LayerType)))
                    {
                        if (!configuredLayerTypes.Contains(terrainPipelineLayerType))
                        {
                            TerrainPipelineLayer terrainPipelineLayer = new TerrainPipelineLayer();
                            terrainPipelineLayer.LayerCode = (byte)terrainPipelineLayerType;

                            layersCopy.Add(terrainPipelineLayer);
                        }
                    }

                    layers = layersCopy;
                }
                else
                {
                    layers = value;
                }
            }
        }

        private Configurations(bool protectData)
        {
            this.protectData = protectData;
            Layers = new List<TerrainPipelineLayer>();
        }
        private Configurations() {
            Layers = new List<TerrainPipelineLayer>();
        }

        internal static Configurations LoadConfigurations(TerrainProject terrainProject)
        {
            Configurations configurations = loadConfigurations(terrainProject.Path);

            currentPath = terrainProject.Path;

            return configurations;
        }

        internal static Configurations LoadConfigurations()
        {
            return loadConfigurations(CONFIGURATIONS_DEFAULT_PATH);
        }

        private static Configurations loadConfigurations(String path)
        {
            currentPath = path;

            string fileContent = FileInterface.ReadString(path + Path.DirectorySeparatorChar + CONFIGURATIONS_FILE);

            Configurations configurations = null;

            if (fileContent != null)
            {
                Object obj = ObjectSerializer.DeSerialize(fileContent, typeof(Configurations));
                if (obj != null)
                {
                    Console.WriteLine("Object is not null. Path: " + currentPath);
                    configurations = (Configurations)obj;
                }
            }
            else
            {
                Console.WriteLine("File content null");
                if (!currentPath.Equals(CONFIGURATIONS_DEFAULT_PATH))
                {
                    return LoadConfigurations();
                }
            }


            if (configurations == null)
            {
                Console.WriteLine("Finally configurations are null");
                configurations = new Configurations(true);
            }

            configurations.protectData = true;
            return configurations;
        }

        internal void Commit()
        {
            string data = ObjectSerializer.Serialize(this);
            FileInterface.WriteString(currentPath + Path.DirectorySeparatorChar + CONFIGURATIONS_FILE, data);
        }
    }
}
